using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
  [SerializeField] ChunkGenerator chunkGeneratorPrefab;

  [SerializeField] float chunkDistance;

  Dictionary<Vector3, ChunkGenerator> chunkTable = new Dictionary<Vector3, ChunkGenerator>();

  [SerializeField] Vector3[] chunkOffsets;

  public struct ChunkPerlinOffsets
  {
    public float chunkOffsetX;
    public float chunkOffsetZ;
  }

  public ChunkPerlinOffsets chunkPerlinOffsets;

  private void Start()
  {
    chunkPerlinOffsets.chunkOffsetX = UnityEngine.Random.Range(0f, 999f);
    chunkPerlinOffsets.chunkOffsetZ = UnityEngine.Random.Range(0f, 999f);
    CreateChunkGenerators();
  }

  private void CreateChunkGenerators()
  {
    foreach (Vector3 chunkLocation in chunkOffsets)
    {
      Vector3 newChunkLocation = transform.position + chunkLocation * chunkDistance;
      ChunkGenerator newChunk = Instantiate(chunkGeneratorPrefab, newChunkLocation, Quaternion.identity, transform);
      newChunk.UpdateName();
      if (chunkTable.ContainsKey(newChunkLocation))
      {
        continue;
      }
      chunkTable.Add(newChunkLocation, newChunk);
    }
  }

  public bool IsChunkGenerated(ChunkGenerator chunkGenerator)
  {
    if (chunkTable.ContainsKey(chunkGenerator.transform.position))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  public bool GetChunkGeneratorByVector(Vector3 chunkGeneratorName)
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

  public ChunkPerlinOffsets GetPerlinOffset(Vector3 chunkGenerator)
  {
    ChunkPerlinOffsets newPerlinOffsets;
    newPerlinOffsets.chunkOffsetX = chunkPerlinOffsets.chunkOffsetX + chunkGenerator.x;
    newPerlinOffsets.chunkOffsetZ = chunkPerlinOffsets.chunkOffsetZ + chunkGenerator.z;
    return newPerlinOffsets;
  }
}
