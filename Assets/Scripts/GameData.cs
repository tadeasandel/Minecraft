using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
  public PlayerData playerData = new PlayerData();

  public WorldData worldData = new WorldData();

  public GameData(WorldManager worldManager, ObjectInteractionController objectInteractionController)
  {
    ProcessPlayerData(objectInteractionController);

    ProcessWorldData(worldManager);

  }

  private void ProcessWorldData(WorldManager worldManager)
  {
    // stores the random generated number for Perlin Noise offset for both X and Z value
    worldData.chunkPerlinOffsetX = worldManager.chunkPerlinOffsets.chunkOffsetX;
    worldData.chunkPerlinOffsetZ = worldManager.chunkPerlinOffsets.chunkOffsetZ;

    // stores which chunk was the central one for player
    worldData.centralChunkX = worldManager.currentCenterChunkPos.x;
    worldData.centralChunkZ = worldManager.currentCenterChunkPos.z;

    // stores all information about all chunks and their cubes
    foreach (KeyValuePair<Vector3, ChunkGenerator> chunk in worldManager.chunkTable)
    {
      WorldData.ChunkData chunkData = new WorldData.ChunkData();
      chunkData.chunkXPosition = chunk.Key.x;
      chunkData.chunkZPosition = chunk.Key.z;
      foreach (KeyValuePair<Vector3, CubeEditor> cube in chunk.Value.cubeEditorTable)
      {
        chunkData.cubePositionsX.Add(cube.Key.x);
        chunkData.cubePositionsY.Add(cube.Key.y);
        chunkData.cubePositionsZ.Add(cube.Key.z);
        chunkData.cubeTypeNames.Add(cube.Value.currentCubeType.cubeTypeName);
      }
      worldData.chunkData.Add(chunkData);
    }

    // stores information about all destroyed chunks
    foreach (KeyValuePair<Vector3, WorldData.ChunkData> chunk in worldManager.destroyedChunkData)
    {
      worldData.destroyedChunkData.Add(chunk.Value);
    }
  }

  // stores player's position
  private void ProcessPlayerData(ObjectInteractionController objectInteractionController)
  {
    playerData.playerXPosition = objectInteractionController.transform.position.x;
    playerData.playerYPosition = objectInteractionController.transform.position.y;
    playerData.playerZPosition = objectInteractionController.transform.position.z;
  }
}

[System.Serializable]
public class PlayerData
{
  public float playerXPosition;
  public float playerYPosition;
  public float playerZPosition;
}

[System.Serializable]
public class WorldData
{
  public List<ChunkData> chunkData = new List<ChunkData>();
  public List<ChunkData> destroyedChunkData = new List<ChunkData>();
  public float chunkPerlinOffsetX;
  public float chunkPerlinOffsetZ;
  public float centralChunkX;
  public float centralChunkZ;

  [System.Serializable]
  public class ChunkData
  {
    public float chunkXPosition;
    public float chunkZPosition;
    public List<float> cubePositionsX = new List<float>();
    public List<float> cubePositionsY = new List<float>();
    public List<float> cubePositionsZ = new List<float>();
    public List<string> cubeTypeNames = new List<string>();
  }
}

