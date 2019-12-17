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

    worldData.chunkPerlinOffsetX = worldManager.chunkPerlinOffsets.chunkOffsetX;
    worldData.chunkPerlinOffsetZ = worldManager.chunkPerlinOffsets.chunkOffsetZ;

    worldData.centralChunkX = worldManager.currentCenterChunkPos.x;
    worldData.centralChunkZ = worldManager.currentCenterChunkPos.z;

    foreach (KeyValuePair<Vector3, ChunkGenerator> chunk in worldManager.chunkTable)
    {
      WorldData.ChunkData chunkData = new WorldData.ChunkData();
      chunkData.chunkXPosition = chunk.Key.x;
      chunkData.chunkZPosition = chunk.Key.z;
      chunkData.isDisabled = chunk.Value.IsDisabled();
      foreach (KeyValuePair<Vector3, CubeEditor> cube in chunk.Value.cubeEditorTable)
      {
        chunkData.cubePositionsX.Add(cube.Key.x);
        chunkData.cubePositionsY.Add(cube.Key.y);
        chunkData.cubePositionsZ.Add(cube.Key.z);
        chunkData.cubeTypeNames.Add(cube.Value.currentCubeType.cubeTypeName);
      }
      worldData.chunkData.Add(chunkData);
    }
  }

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
  public float chunkPerlinOffsetX;
  public float chunkPerlinOffsetZ;
  public float centralChunkX;
  public float centralChunkZ;

  [System.Serializable]
  public class ChunkData
  {
    public float chunkXPosition;
    public float chunkZPosition;
    public bool isDisabled;
    public List<float> cubePositionsX = new List<float>();
    public List<float> cubePositionsY = new List<float>();
    public List<float> cubePositionsZ = new List<float>();
    public List<string> cubeTypeNames = new List<string>();
  }
}

