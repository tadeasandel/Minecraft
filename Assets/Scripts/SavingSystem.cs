using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SavingSystem : MonoBehaviour
{
  string saveFileName = "gamedata.sav";

  public void SaveGame(WorldData worldData, PlayerData playerData)
  {
    string path = GetDataPath();
    using (FileStream stream = new FileStream(path, FileMode.Create))
    {
      BinaryFormatter formatter = new BinaryFormatter();
      GameData gameData = new GameData(worldData, playerData);
      formatter.Serialize(stream, gameData);
    }
  }

  public string GetDataPath()
  {
    return Path.Combine(Application.persistentDataPath, saveFileName); ;
  }
}