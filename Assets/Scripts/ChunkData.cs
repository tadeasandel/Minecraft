using System.Collections.Generic;
using UnityEngine;

public class ChunkData : MonoBehaviour
{
  public float[] cubePositionsX;
  public float[] cubePositionsY;
  public float[] cubePositionsZ;
  List<CubeData> cubeData;

  public ChunkData(ChunkGenerator chunkGenerator)
  {
    int i = 0;
    foreach (var cubePosition in chunkGenerator.cubePositionTable)
    {
      cubePositionsX[i] = cubePosition.x;
      cubePositionsY[i] = cubePosition.y;
      cubePositionsZ[i] = cubePosition.z;
      i++;
    }
  }
}