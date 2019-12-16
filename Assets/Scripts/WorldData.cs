using System.Collections.Generic;
using UnityEngine;

public class WorldData : MonoBehaviour
{
  public ChunkPerlinOffsets generatedPerlinOffset;
  public float[] chunkXPositions;
  public float[] chunkZPositions;
  public List<ChunkData> chunkData;

  public WorldData(WorldManager worldManager)
  {
    generatedPerlinOffset = worldManager.chunkPerlinOffsets;
    int i = 0;
    foreach (var chunkLocation in worldManager.chunkPositions)
    {
      chunkXPositions[i] = chunkLocation.x;
      chunkZPositions[i] = chunkLocation.z;
      i++;
    }
    chunkData = worldManager.chunksData;
  }

  public bool ContainsChunkByVector(Vector3 chunkPosToCheck)
  {
    for (int i = 0; i < chunkXPositions.Length; i++)
    {
      if (chunkXPositions[i] == chunkPosToCheck.x && chunkZPositions[i] == chunkPosToCheck.z)
      {
        return true;
      }
    }
    return false;
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