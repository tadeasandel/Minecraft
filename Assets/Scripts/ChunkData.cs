// using System.Collections.Generic;
// using UnityEngine;

// [System.Serializable]
// public class ChunkData : MonoBehaviour
// {
//   CubeDataBase cubeDataBase = new CubeDataBase();

//   public ChunkData(ChunkGenerator chunkGenerator)
//   {
//     cubeDataBase.cubePositionsX = new List<float>();
//     cubeDataBase.cubePositionsY = new List<float>();
//     cubeDataBase.cubePositionsZ = new List<float>();
//     // cubeDataBase.cubeType = new List<CubeType>();

//     foreach (KeyValuePair<Vector3, CubeEditor> cube in chunkGenerator.cubeEditorTable)
//     {
//       cubeDataBase.cubePositionsX.Add(cube.Key.x);
//       cubeDataBase.cubePositionsY.Add(cube.Key.y);
//       cubeDataBase.cubePositionsZ.Add(cube.Key.z);
//       // cubeDataBase.cubeType.Add(cube.Value.currentCubeType);
//     }
//   }

//   public CubeDataBase FormCubeData()
//   {
//     return cubeDataBase;
//   }

// }
// [System.Serializable]
// public class CubeDataBase
// {
//   public List<float> cubePositionsX;
//   public List<float> cubePositionsY;
//   public List<float> cubePositionsZ;
//   // public List<CubeType> cubeType;
// }
