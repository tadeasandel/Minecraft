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

  [SerializeField] Vector3[] chunkOffsets;

  public ChunkPerlinOffsets chunkPerlinOffsets;

  private void Start()
  {
    chunkPerlinOffsets.chunkOffsetX = UnityEngine.Random.Range(999f, 99999f);
    chunkPerlinOffsets.chunkOffsetZ = UnityEngine.Random.Range(999f, 99999f);
    CreateChunkGenerators();
  }

  public float GetChunkDistance()
  {
    return chunkDistance;
  }

  private void CreateChunkGenerators()
  {
    int i = 0;
    foreach (Vector3 chunkLocation in chunkOffsets)
    {
      Vector3 newChunkLocation = transform.position + chunkLocation * chunkDistance;
      CreateChunk(newChunkLocation);
      i++;
    }
  }

  public void RefreshChunks(Vector3 centerChunkPos)
  {
    print("refresh called");
    int i = 0;
    foreach (Vector3 chunkOffset in chunkOffsets)
    {
      if (chunkOffset == new Vector3(0, 0, 0)) { continue; }
      Vector3 chunkPos = centerChunkPos + chunkOffset * chunkDistance;
      if (!chunkTable.ContainsKey(chunkPos))
      {
        CreateChunk(chunkPos);
      }
      else
      {
        DisableChunk(chunkPos);
      }
      i++;
    }
  }

  private void CreateChunk(Vector3 chunkPos)
  {
    ChunkGenerator newChunk = Instantiate(chunkGeneratorPrefab, chunkPos, Quaternion.identity, transform);
    newChunk.UpdateName();
    if (!chunkTable.ContainsKey(chunkPos))
    {
      chunkTable.Add(chunkPos, newChunk);
      chunkPositions.Add(chunkPos);
      newChunk.SetChunkSetup();
      newChunk.GenerateChunk(GetPerlinOffset(newChunk.transform.position));
      newChunk.SetBoxCollider();
    }
  }

  private void DestroyChunk(Vector3 chunkPos)
  {
    Destroy(chunkTable[chunkPos].gameObject);
    chunkTable.Remove(chunkPos);
    chunkPositions.Remove(chunkPos);
  }

  public void DisableChunk(Vector3 chunkPos)
  {
    if (chunkTable.ContainsKey(chunkPos))
    {
      chunkTable[chunkPos].gameObject.SetActive(false);
    }
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
public struct ChunkPerlinOffsets
{
  public float chunkOffsetX;
  public float chunkOffsetZ;
}

