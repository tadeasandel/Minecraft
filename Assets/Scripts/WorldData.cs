using System.Collections.Generic;
using UnityEngine;

public class WorldData : MonoBehaviour
{
  public Dictionary<Vector3, ChunkGenerator> chunkTable = new Dictionary<Vector3, ChunkGenerator>();
  public ChunkPerlinOffsets chunkPerlinOffsets;

  public WorldData(WorldManager worldManager)
  {
    chunkTable = worldManager.chunkTable;
    chunkPerlinOffsets = worldManager.chunkPerlinOffsets;
  }
}