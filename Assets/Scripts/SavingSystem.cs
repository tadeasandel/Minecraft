using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

public class SavingSystem : MonoBehaviour
{
  string saveFileName = "gamedata.sav";

  // saves current GameData into a save file
  public void SaveGame(WorldManager worldManager, ObjectInteractionController objectInteractionController)
  {
    string path = GetDataPath(saveFileName);
    using (FileStream stream = new FileStream(path, FileMode.Create))
    {
      print("saving to " + path);
      BinaryFormatter formatter = new BinaryFormatter();
      GameData gameData = new GameData(worldManager, objectInteractionController);
      formatter.Serialize(stream, gameData);
    }
  }

  // Loads Game from saving file, if it exists
  public GameData LoadGame()
  {
    string path = GetDataPath(saveFileName);
    if (!File.Exists(path))
    {
      return null;
    }
    using (FileStream stream = new FileStream(path, FileMode.Open))
    {
      print("loading to " + path);
      BinaryFormatter formatter = new BinaryFormatter();
      GameData gameData = (GameData)formatter.Deserialize(stream);
      return gameData;
    }
  }

  // Deletes a save file if it exists
  public void DeleteFile()
  {
    string path = GetDataPath(saveFileName);
    if (!File.Exists(path))
    {
      return;
    }
    print("deleting " + path);
    File.Delete(path);
  }

  // Checks if a file of filepath exists
  public bool CanLoadGame()
  {
    string path = GetDataPath(saveFileName);
    if (File.Exists(path))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  // creates data path for saving
  public string GetDataPath(string saveFileName)
  {
    return Path.Combine(Application.persistentDataPath, saveFileName); ;
  }
}