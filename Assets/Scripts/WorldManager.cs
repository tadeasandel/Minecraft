using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
  [SerializeField] ChunkGenerator chunkGeneratorPrefab;

  [SerializeField] float chunkDistance;

  public Dictionary<Vector3, ChunkGenerator> chunkTable = new Dictionary<Vector3, ChunkGenerator>();
  public Dictionary<Vector3, WorldData.ChunkData> destroyedChunkData = new Dictionary<Vector3, WorldData.ChunkData>();

  [SerializeField] int loadedChunkDistance;
  [SerializeField] int disabledChunkDistance;
  [SerializeField] int DestroyedChunkDistance;

  public List<Vector3> loadedChunkArea = new List<Vector3>();
  public List<Vector3> disabledChunkArea = new List<Vector3>();
  public List<Vector3> destroyedChunkArea = new List<Vector3>();

  public ChunkPerlinOffsets chunkPerlinOffsets;

  public Vector3 currentCenterChunkPos = new Vector3(0, 0, 0);

  SavingWrapper savingWrapper;

  Vector3 playerSpawnPos;

  Vector3 centralChunkPos;

  [SerializeField] GameObject playerPrefab;
  bool isGameLoaded = false;
  bool isPlayerSpawned = false;
  [SerializeField] GameObject[] objectsToDisable;
  [SerializeField] GameObject[] objectsToEnable;

  private void Start()
  {
    savingWrapper = FindObjectOfType<SavingWrapper>();
    FillAreas();
    ProcessLoading();
  }

  private void Update()
  {
    if (!isGameLoaded)
    {
      foreach (KeyValuePair<Vector3, ChunkGenerator> chunk in chunkTable)
      {
        if (!chunk.Value.isLoaded)
        {
          return;
        }
      }
      isGameLoaded = true;
    }
    if (!isPlayerSpawned && isGameLoaded)
    {
      SpawnPlayer(playerSpawnPos);
      isPlayerSpawned = true;
      foreach (GameObject objectToDisable in objectsToDisable)
      {
        objectToDisable.SetActive(false);
      }
      foreach (GameObject objectToEnable in objectsToEnable)
      {
        objectToEnable.SetActive(true);
      }
    }
  }

  private void ProcessLoading()
  {
    if (savingWrapper.CanLoadGame())
    {
      savingWrapper.StartLoadingGame();
    }
    else
    {
      chunkPerlinOffsets.chunkOffsetX = UnityEngine.Random.Range(999f, 99999f);
      chunkPerlinOffsets.chunkOffsetZ = UnityEngine.Random.Range(999f, 99999f);
      CreateChunkGenerators();
    }
  }

  private void SpawnPlayer(Vector3 playerPos)
  {
    Instantiate(playerPrefab, playerPos, Quaternion.identity);
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
    foreach (Vector3 loadedArea in loadedChunkArea)
    {
      Vector3 newChunkLocation = transform.position + loadedArea * chunkDistance;
      StartCoroutine(CreateChunk(newChunkLocation));
    }
    playerSpawnPos = new Vector3(0, 20, 0); ;
  }

  public void LoadGame(WorldData worldData, Vector3 playerPos)
  {
    chunkPerlinOffsets.chunkOffsetX = worldData.chunkPerlinOffsetX;
    chunkPerlinOffsets.chunkOffsetZ = worldData.chunkPerlinOffsetZ;
    playerSpawnPos = playerPos;
    objectsToDisable[0].transform.position = new Vector3(playerPos.x, 60, playerPos.z);

    chunkTable.Clear();
    destroyedChunkData.Clear();
    centralChunkPos = new Vector3(worldData.centralChunkX, 0, worldData.centralChunkZ);
    // CreateChunkByData(centralChunkPos);

    foreach (WorldData.ChunkData destroyedChunk in worldData.destroyedChunkData)
    {
      destroyedChunkData.Add(new Vector3(destroyedChunk.chunkXPosition, 0, destroyedChunk.chunkZPosition), destroyedChunk);
    }

    foreach (WorldData.ChunkData chunk in worldData.chunkData)
    {
      Vector3 chunkPos = new Vector3(chunk.chunkXPosition, 0, chunk.chunkZPosition);
      StartCoroutine(CreateChunkByData(chunkPos, chunk));
    }
    RefreshChunks(new Vector3(centralChunkPos.x, 0, centralChunkPos.z));
  }

  public IEnumerator CreateChunkByData(Vector3 chunkPos, WorldData.ChunkData chunkData)
  {
    ChunkGenerator newChunk = new ChunkGenerator();
    newChunk = Instantiate(chunkGeneratorPrefab, chunkPos, Quaternion.identity, transform);
    newChunk.UpdateName();

    if (!chunkTable.ContainsKey(chunkPos))
    {
      chunkTable.Add(chunkPos, newChunk);
      newChunk.SetChunkSetup();
      newChunk.SetBoxCollider();
      yield return newChunk.GenerateLoadedChunk(chunkData);
      yield return null;
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
      if (destroyedChunkData.ContainsKey(neighbourChunkPos) && !chunkTable.ContainsKey(neighbourChunkPos))
      {
        StartCoroutine(RestoreChunk(neighbourChunkPos));
      }
      else if (!chunkTable.ContainsKey(neighbourChunkPos))
      {
        StartCoroutine(CreateChunk(neighbourChunkPos));
      }
      else
      {
        StartCoroutine(EnableChunk(neighbourChunkPos));
      }
    }
    foreach (Vector3 disableArea in disabledChunkArea)
    {
      Vector3 disableChunkPos = centerChunkPos + disableArea * chunkDistance;
      if (chunkTable.ContainsKey(disableChunkPos))
      {
        StartCoroutine(DisableChunk(disableChunkPos));
      }
    }
    foreach (Vector3 destroyArea in destroyedChunkArea)
    {
      Vector3 destroyChunkPos = centerChunkPos + destroyArea * chunkDistance;
      if (chunkTable.ContainsKey(destroyChunkPos))
      {
        StartCoroutine(DestroyChunk(destroyChunkPos));
      }
    }
  }

  private IEnumerator RestoreChunk(Vector3 chunkPos)
  {
    ChunkGenerator restoredChunk = Instantiate(chunkGeneratorPrefab, chunkPos, Quaternion.identity, transform);
    restoredChunk.UpdateName();

    chunkTable.Add(chunkPos, restoredChunk);
    restoredChunk.SetChunkSetup();
    restoredChunk.SetBoxCollider();
    yield return restoredChunk.GenerateLoadedChunk(destroyedChunkData[chunkPos]);
    destroyedChunkData.Remove(chunkPos);
  }

  private IEnumerator CreateChunk(Vector3 chunkPos)
  {
    ChunkGenerator newChunk = Instantiate(chunkGeneratorPrefab, chunkPos, Quaternion.identity, transform);
    newChunk.UpdateName();
    if (!chunkTable.ContainsKey(chunkPos))
    {
      chunkTable.Add(chunkPos, newChunk);
      newChunk.SetChunkSetup();
      newChunk.SetBoxCollider();
      yield return newChunk.GenerateChunk(GetPerlinOffset(newChunk.transform.position));
    }
  }

  private IEnumerator DestroyChunk(Vector3 chunkPos)
  {
    WorldData.ChunkData destroyedChunk = new WorldData.ChunkData();

    destroyedChunk.chunkXPosition = chunkPos.x;
    destroyedChunk.chunkZPosition = chunkPos.z;

    foreach (KeyValuePair<Vector3, CubeEditor> cubeData in chunkTable[chunkPos].cubeEditorTable)
    {
      destroyedChunk.cubePositionsX.Add(cubeData.Key.x);
      destroyedChunk.cubePositionsY.Add(cubeData.Key.y);
      destroyedChunk.cubePositionsZ.Add(cubeData.Key.z);
      string cubeTypeName = chunkTable[chunkPos].ProcessCubeByName(cubeData.Value.GetCubeType().cubeTypeName).cubeTypeName;
      destroyedChunk.cubeTypeNames.Add(cubeTypeName);
    }

    destroyedChunkData.Add(chunkPos, destroyedChunk);

    Destroy(chunkTable[chunkPos].gameObject);
    yield return null;
    chunkTable.Remove(chunkPos);
  }

  public IEnumerator DisableChunk(Vector3 chunkPos)
  {
    chunkTable[chunkPos].gameObject.SetActive(false);
    yield return null;
  }

  private IEnumerator EnableChunk(Vector3 chunkPos)
  {
    chunkTable[chunkPos].gameObject.SetActive(true);
    yield return null;
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
    ChunkPerlinOffsets newPerlinOffsets = new ChunkPerlinOffsets();
    newPerlinOffsets.chunkOffsetX = chunkPerlinOffsets.chunkOffsetX + chunkGenerator.x / chunkDistance;
    newPerlinOffsets.chunkOffsetZ = chunkPerlinOffsets.chunkOffsetZ + chunkGenerator.z / chunkDistance;
    return newPerlinOffsets;
  }
}

[System.Serializable]
public class ChunkPerlinOffsets
{
  public float chunkOffsetX;
  public float chunkOffsetZ;
}

