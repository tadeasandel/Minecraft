using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SavingSystem : MonoBehaviour
{
  string saveFileName = "gamedata.sav";

  public void SaveGame()
  {
    string path = GetDataPath();
    using (FileStream stream = new FileStream(path, FileMode.Create))
    {
      BinaryFormatter formatter = new BinaryFormatter();
      PlayerData playerData = FindObjectOfType<PlayerData>();
      formatter.Serialize(stream, playerData);
    }
  }

  public string GetDataPath()
  {
    return Path.Combine(Application.persistentDataPath, saveFileName); ;
  }
}