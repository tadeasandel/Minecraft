using UnityEngine;

public class GameData : MonoBehaviour
{
  public WorldData worldData;
  public PlayerData playerData;
  SavingSystem savingSystem;

  private void Start()
  {
    savingSystem = FindObjectOfType<SavingSystem>();
    worldData = FindObjectOfType<WorldData>();
    playerData = FindObjectOfType<PlayerData>();
  }

  public GameData(WorldData worldData, PlayerData playerData)
  {
    this.worldData = worldData;
    this.playerData = playerData;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.S))
    {
      worldData.SaveData();
      playerData.SaveData();
      savingSystem.SaveGame();
    }
  }
}