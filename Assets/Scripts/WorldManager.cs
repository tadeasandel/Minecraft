using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
  [SerializeField] ChunkGenerator chunkGeneratorPrefab;

  [SerializeField] float chunkDistance;

  public Dictionary<Vector3, ChunkGenerator> chunkTable = new Dictionary<Vector3, ChunkGenerator>();
  public List<Vector3> chunkPositions = new List<Vector3>();

  [SerializeField] int loadedChunkDistance;
  [SerializeField] int disabledChunkDistance;
  [SerializeField] int DestroyedChunkDistance;

  public List<Vector3> loadedChunkArea = new List<Vector3>();
  public List<Vector3> disabledChunkArea = new List<Vector3>();
  public List<Vector3> destroyedChunkArea = new List<Vector3>();

  public ChunkPerlinOffsets chunkPerlinOffsets;

  Vector3 currentCenterChunkPos = new Vector3(0, 0, 0);

  public List<ChunkData> chunksData = new List<ChunkData>();
  public WorldData worldData;

  public List<Chunk> chunks = new List<Chunk>();

  private void Start()
  {
    chunkPerlinOffsets.chunkOffsetX = UnityEngine.Random.Range(999f, 99999f);
    chunkPerlinOffsets.chunkOffsetZ = UnityEngine.Random.Range(999f, 99999f);
    FillAreas();
    // chunkTable.Add(new Vector3(0, 0, 0), new ChunkGenerator());
    // chunkTable.Add(new Vector3(1, 0, 0), new ChunkGenerator());
    // chunkTable.Add(new Vector3(2, 0, 0), new ChunkGenerator());
    // chunkTable.Add(new Vector3(3, 0, 0), new ChunkGenerator());
    // chunkTable.Add(new Vector3(4, 0, 0), new ChunkGenerator());
    // chunkTable.Add(new Vector3(5, 0, 0), new ChunkGenerator());
    // chunkTable.Add(new Vector3(6, 0, 0), new ChunkGenerator());
    // chunkTable.Add(new Vector3(7, 0, 0), new ChunkGenerator());
    // foreach (KeyValuePair<Vector3, ChunkGenerator> chunkzz in chunkTable)
    // {
    //   print(chunkzz.Key + " " + chunkzz.Value);
    // }
    CreateChunkGenerators();
    worldData = new WorldData(this);
  }

  private void FillAreas()
  {
    FillArea(loadedChunkArea, loadedChunkDistance);
    FillArea(disabledChunkArea, disabledChunkDistance);
    FillArea(destroyedChunkArea, DestroyedChunkDistance);
    UnfillArea(destroyedChunkArea, disabledChunkArea);
    UnfillArea(disabledChunkArea, loadedChunkArea);
  }

  private void FillArea(List<Vector3> areaToFill, int areaSize)
  {
    for (int x = -areaSize; x < areaSize + 1; x++)
    {
      for (int z = -areaSize; z < areaSize + 1; z++)
      {
        areaToFill.Add(new Vector3(x, 0, z));
      }
    }
  }

  private void UnfillArea(List<Vector3> filledArea, List<Vector3> areaToRemove)
  {
    if (areaToRemove != null)
    {
      foreach (var area in areaToRemove)
      {
        if (filledArea.Contains(area))
        {
          filledArea.Remove(area);
        }
      }
    }
  }

  public float GetChunkDistance()
  {
    return chunkDistance;
  }

  private void CreateChunkGenerators()
  {
    int i = 0;
    foreach (Vector3 loadedArea in loadedChunkArea)
    {
      Vector3 newChunkLocation = transform.position + loadedArea * chunkDistance;
      CreateChunk(newChunkLocation);
      i++;
    }
  }

  public void RefreshChunks(Vector3 centerChunkPos)
  {
    Vector3 previousCenterChunk = currentCenterChunkPos;
    currentCenterChunkPos = centerChunkPos;
    foreach (Vector3 loadedArea in loadedChunkArea)
    {
      if (loadedArea == new Vector3(0, 0, 0)) { continue; }
      Vector3 neighbourChunkPos = centerChunkPos + loadedArea * chunkDistance;
      // if (!ContainsChunkVector(neighbourChunkPos))
      // {
      //   print("no chunk to enable - creating");
      //   CreateChunk(neighbourChunkPos);
      // }
      // else
      // {
      //   print("enabling a chunk at pos " + neighbourChunkPos);
      //   EnableChunk(neighbourChunkPos);
      // }
      if (!chunkTable.ContainsKey(neighbourChunkPos))
      {
        CreateChunk(neighbourChunkPos);
      }
      else
      {
        EnableChunk(neighbourChunkPos);
      }
    }
    foreach (Vector3 disableArea in disabledChunkArea)
    {
      Vector3 disableChunkPos = centerChunkPos + disableArea * chunkDistance;
      // if (ContainsChunkVector(disableChunkPos))
      // {
      //   DisableChunk(disableChunkPos);
      // }
      if (chunkTable.ContainsKey(disableChunkPos))
      {
        DisableChunk(disableChunkPos);
      }
    }
    foreach (Vector3 destroyArea in destroyedChunkArea)
    {
      Vector3 destroyChunkPos = centerChunkPos + destroyArea * chunkDistance;
      // if (ContainsChunkVector(destroyChunkPos))
      // {
      //   DestroyChunk(destroyChunkPos);
      // }
      if (chunkTable.ContainsKey(destroyChunkPos))
      {
        DestroyChunk(destroyChunkPos);
      }
    }
  }

  private void CreateChunk(Vector3 chunkPos)
  {
    // if (worldData.ContainsChunkByVector(chunkPos))
    // {
    //   LoadChunkBydata(chunkPos);
    // }
    print("creating chunk");
    ChunkGenerator newChunk = Instantiate(chunkGeneratorPrefab, chunkPos, Quaternion.identity, transform);
    newChunk.UpdateName();
    // Chunk newChunkData = new Chunk();
    // newChunkData.chunkGenerator = newChunk;
    // newChunkData.key = newChunk.name;
    // newChunkData.position = chunkPos;
    // newChunkData.chunkGenerator.SetChunkSetup();
    // newChunkData.offsets = GetPerlinOffset(chunkPos);
    // newChunkData.chunkGenerator.GenerateChunk(newChunkData.offsets);
    // newChunkData.chunkGenerator.SetBoxCollider();
    // chunks.Add(newChunkData);
    if (!chunkTable.ContainsKey(chunkPos))
    {
      chunkTable.Add(chunkPos, newChunk);
      chunkPositions.Add(chunkPos);
      newChunk.SetChunkSetup();
      newChunk.GenerateChunk(GetPerlinOffset(newChunk.transform.position));
      newChunk.SetBoxCollider();
      ChunkData chunkData = new ChunkData(newChunk);
      chunksData.Add(chunkData);
    }
  }

  public bool ContainsChunkVector(Vector3 chunkPos)
  {
    foreach (Chunk chunk in chunks)
    {
      if (chunk.position == chunkPos)
      {
        return true;
      }
    }
    return false;
  }

  public Chunk GetChunkByVector(Vector3 chunkPos)
  {
    foreach (Chunk chunk in chunks)
    {
      if (chunk.position == chunkPos)
      {
        return chunk;
      }
    }
    print("no chunk found at " + chunkPos);
    return chunks[0];
  }

  private void LoadChunkByVector(Vector3 chunkPos)
  {

  }

  private void DestroyChunk(Vector3 chunkPos)
  {
    // Chunk temporaryChunk = GetChunkByVector(chunkPos);
    // Destroy(temporaryChunk.chunkGenerator.gameObject);
    Destroy(chunkTable[chunkPos].gameObject);
    // chunks.Remove(temporaryChunk);
    chunkTable.Remove(chunkPos);
    chunkPositions.Remove(chunkPos);
  }

  public void DisableChunk(Vector3 chunkPos)
  {
    // GetChunkGeneratorByVector(chunkPos).gameObject.SetActive(false);
    chunkTable[chunkPos].gameObject.SetActive(false);
  }

  private void EnableChunk(Vector3 chunkPos)
  {
    // GetChunkByVector(chunkPos).chunkGenerator.gameObject.SetActive(true);
    chunkTable[chunkPos].gameObject.SetActive(true);
  }

  public bool IsChunkGenerated(ChunkGenerator chunkGenerator)
  {
    if (chunkTable.ContainsKey(chunkGenerator.transform.position))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  public ChunkGenerator GetChunkGeneratorByVector(Vector3 chunkGeneratorName)
  {
    if (chunkTable.ContainsKey(chunkGeneratorName))
    {
      return chunkTable[chunkGeneratorName];
    }
    else
    {
      return null;
    }
  }

  private ChunkPerlinOffsets GetPerlinOffset(Vector3 chunkGenerator)
  {
    ChunkPerlinOffsets newPerlinOffsets;
    newPerlinOffsets.chunkOffsetX = chunkPerlinOffsets.chunkOffsetX + chunkGenerator.x / chunkDistance;
    newPerlinOffsets.chunkOffsetZ = chunkPerlinOffsets.chunkOffsetZ + chunkGenerator.z / chunkDistance;
    return newPerlinOffsets;
  }
}

[System.Serializable]
public struct Chunk
{
  public string key;
  public Vector3 position;
  public ChunkGenerator chunkGenerator;
  public ChunkPerlinOffsets offsets;
}

[System.Serializable]
public struct ChunkPerlinOffsets
{
  public float chunkOffsetX;
  public float chunkOffsetZ;
}

