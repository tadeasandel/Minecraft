using UnityEngine;

public class GameData : MonoBehaviour
{
  WorldData worldData;
  PlayerData playerData;

  public GameData(WorldData worldData, PlayerData playerData)
  {
    this.worldData = worldData;
    this.playerData = playerData;
  }
}