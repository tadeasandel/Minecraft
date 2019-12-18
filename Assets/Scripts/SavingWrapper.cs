using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavingWrapper : MonoBehaviour
{
  WorldManager worldManager;
  ObjectInteractionController objectInteractionController;
  SavingSystem savingSystem;

  private void Start()
  {
    savingSystem = GetComponent<SavingSystem>();
    WorldManager worldManager = FindObjectOfType<WorldManager>();
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.P))
    {
      if (worldManager == null) { worldManager = FindObjectOfType<WorldManager>(); }
      objectInteractionController = FindObjectOfType<ObjectInteractionController>();
      savingSystem.SaveGame(worldManager, objectInteractionController);
    }
    if (Input.GetKeyDown(KeyCode.L))
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    if (Input.GetKeyDown(KeyCode.Delete))
    {
      savingSystem.DeleteFile();
    }
  }

  // starts loading all data
  public void StartLoadingGame()
  {
    GameData gameData = savingSystem.LoadGame();
    if (gameData == null) { print("no file found"); return; }
    if (worldManager == null) { worldManager = FindObjectOfType<WorldManager>(); }

    Vector3 playerNewPos = new Vector3(gameData.playerData.playerXPosition, gameData.playerData.playerYPosition, gameData.playerData.playerZPosition);
    worldManager.LoadGame(gameData.worldData, playerNewPos);
  }

  // checks if game is loadable
  public bool CanLoadGame()
  {
    if (savingSystem == null) { savingSystem = GetComponent<SavingSystem>(); }
    if (savingSystem.CanLoadGame())
    {
      return true;
    }
    else
    {
      return false;
    }
  }
}