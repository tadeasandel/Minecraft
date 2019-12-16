using System.Collections.Generic;
using UnityEngine;

public class WorldData : MonoBehaviour
{
  public ChunkPerlinOffsets generatedPerlinOffset = new ChunkPerlinOffsets();
  public ChunkDatabase chunkDatabase = new ChunkDatabase();
  public List<ChunkData> chunkData = new List<ChunkData>();

  WorldManager worldManager;

  private void Start()
  {
    worldManager = FindObjectOfType<WorldManager>();
    chunkDatabase.chunkXPositions = new List<float>();
    chunkDatabase.chunkZPositions = new List<float>();
  }

  public void SaveData()
  {
    generatedPerlinOffset = worldManager.chunkPerlinOffsets;
    foreach (KeyValuePair<Vector3, ChunkGenerator> chunk in worldManager.chunkTable)
    {
      chunkDatabase.chunkXPositions.Add(chunk.Key.x);
      chunkDatabase.chunkZPositions.Add(chunk.Key.z);
    }
    chunkData = worldManager.chunksData;
  }

  // public bool ContainsChunkByVector(Vector3 chunkPosToCheck)
  // {
  //   for (int i = 0; i < chunkXPositions.Length; i++)
  //   {
  //     if (chunkXPositions[i] == chunkPosToCheck.x && chunkZPositions[i] == chunkPosToCheck.z)
  //     {
  //       return true;
  //     }
  //   }
  //   return false;
  // }

  [System.Serializable]
  public class ChunkDatabase
  {
    public List<float> chunkXPositions;
    public List<float> chunkZPositions;
  }

  // public ChunkData GetChunkDataByVector(Vector3 chunkPos)
  // {
  //   ChunkData chunkDataToReturn = null;
  //   for (int i = 0; i < chunkXPositions.Length; i++)
  //   {
  //     if (chunkXPositions[i] == chunkPosToCheck.x && chunkZPositions[i] == chunkPosToCheck.z)
  //     {
  //       ChunkGenerator chunkGenerator = new ChunkGenerator();
  //       chunkGenerator.transform.position = new Vector3(chunkXPositions[i], 0, chunkZPositions[i]);
  //       chunkDataToReturn = new ChunkData(chunkGenerator);
  //     }
  //   }
  //   return chunkDataToReturn;
  // }
}