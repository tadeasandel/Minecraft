using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
  [SerializeField] ChunkGenerator chunkGeneratorPrefab;

  [SerializeField] float chunkDistance;

  Dictionary<string, ChunkGenerator> chunkTable = new Dictionary<string, ChunkGenerator>();

  [SerializeField] Vector3[] chunkLocations;

  private void Start()
  {
    CreateChunkGenerators();
  }

  private void CreateChunkGenerators()
  {
    foreach (Vector3 chunkLocation in chunkLocations)
    {
      Vector3 newChunkLocation = transform.position + chunkLocation * chunkDistance;
      string newChunkName = newChunkLocation.x + "," + newChunkLocation.z;
      ChunkGenerator newChunk = Instantiate(chunkGeneratorPrefab, newChunkLocation, Quaternion.identity, transform);
      if (chunkTable.ContainsKey(newChunkName))
      {
        continue;
      }
      chunkTable.Add(newChunkName, newChunk);
    }
  }

  public bool IsChunkGenerated(ChunkGenerator chunkGenerator)
  {
    if (chunkTable.ContainsKey(chunkGenerator.name))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  public bool GetChunkGeneratorByIndex(string chunkGeneratorName)
  {
    if (chunkTable.ContainsKey(chunkGeneratorName))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  
}
