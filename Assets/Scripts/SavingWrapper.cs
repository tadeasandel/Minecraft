using System;
using System.Collections.Generic;
using UnityEngine;

public class SavingWrapper : MonoBehaviour
{
  WorldManager worldManager;
  ObjectInteractionController objectInteractionController;
  SavingSystem savingSystem;

  private void Start()
  {
    savingSystem = GetComponent<SavingSystem>();
    WorldManager worldManager = FindObjectOfType<WorldManager>();
    ObjectInteractionController objectInteractionController = FindObjectOfType<ObjectInteractionController>();
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.P))
    {
      if (worldManager == null) { worldManager = FindObjectOfType<WorldManager>(); }
      if (objectInteractionController == null) { objectInteractionController = FindObjectOfType<ObjectInteractionController>(); }
      savingSystem.SaveGame(worldManager, objectInteractionController);
    }
    if (Input.GetKeyDown(KeyCode.L))
    {
      GameData gameData = savingSystem.LoadGame();
      if (gameData == null) { print("no file found"); return; }
      if (worldManager == null) { worldManager = FindObjectOfType<WorldManager>(); }
      if (objectInteractionController == null) { objectInteractionController = FindObjectOfType<ObjectInteractionController>(); }

      Vector3 playerNewPos = new Vector3(gameData.playerData.playerXPosition, gameData.playerData.playerYPosition, gameData.playerData.playerZPosition);
      objectInteractionController.LoadState(playerNewPos);
    }
  }

  public void StartLoadingGame()
  {
    GameData gameData = savingSystem.LoadGame();
    if (gameData == null) { print("no file found"); return; }
    if (worldManager == null) { worldManager = FindObjectOfType<WorldManager>(); }
    if (objectInteractionController == null) { objectInteractionController = FindObjectOfType<ObjectInteractionController>(); }

    Vector3 playerNewPos = new Vector3(gameData.playerData.playerXPosition, gameData.playerData.playerYPosition, gameData.playerData.playerZPosition);
    objectInteractionController.LoadState(playerNewPos);
    worldManager.LoadGame(gameData.worldData);
  }

  public bool CanLoadGame()
  {
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