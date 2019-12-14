using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
  [SerializeField] float chunkHeight;
  [SerializeField] float chunkWidth;
  [SerializeField] float chunkDepth;

  [SerializeField] float perlinScale = 0.1f;

  [SerializeField] CubeEditor basicCubePrefab;

  WorldManager worldManager;

  [SerializeField] Vector3 generationOffset;
  float perlinOffsetX;
  float perlinOffsetZ;

  Dictionary<Vector3, CubeEditor> cubeEditors = new Dictionary<Vector3, CubeEditor>();



  public CubeEditor GetCubeEditorByVector(Vector3 cubeEditorName)
  {
    return cubeEditors[cubeEditorName];
  }

  public bool DoesHaveNeighbour(Vector3 neighbourName)
  {
    if (cubeEditors.ContainsKey(neighbourName))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  private void Start()
  {
    worldManager = FindObjectOfType<WorldManager>();
    // if (!worldManager.IsChunkGenerated(this))
    // {
    GenerateChunk();
    // }
  }

  public void AddNewCube(CubeEditor cubeEditor)
  {
    if (!cubeEditors.ContainsKey(cubeEditor.transform.position))
    {
      cubeEditors.Add(cubeEditor.transform.position, cubeEditor);
    }
  }

  public void RemoveCube(CubeEditor cubeEditor)
  {
    if (cubeEditors.ContainsKey(cubeEditor.transform.position))
    {
      cubeEditors.Remove(cubeEditor.transform.position);
    }
  }

  private void GenerateChunk()
  {
    perlinOffsetX = worldManager.GetPerlinOffset(transform.position).chunkOffsetX;
    perlinOffsetZ = worldManager.GetPerlinOffset(transform.position).chunkOffsetZ;
    for (int x = 0; x < chunkHeight; x++)
    {
      for (int z = 0; z < chunkWidth; z++)
      {
        float columnHeight = GenerateHeight(x, z);
        for (int y = 0; y < columnHeight; y++)
        {
          Vector3 newCubeLocation = transform.position + new Vector3(x, y, z) + generationOffset;
          CubeEditor currentCubeEditor = Instantiate(basicCubePrefab, newCubeLocation, Quaternion.identity, transform);
          currentCubeEditor.UpdateName();
          if (!cubeEditors.ContainsKey(currentCubeEditor.transform.position))
          {
            cubeEditors.Add(currentCubeEditor.transform.position, currentCubeEditor);
          }
          else
          {
            Destroy(currentCubeEditor.gameObject);
          }
        }
      }
    }
  }

  private float GenerateHeight(int x, int z)
  {
    float xCoord = (float)x / chunkHeight + perlinOffsetX;
    float zCoord = (float)z / chunkWidth + perlinOffsetZ;
    float columnHeight = Mathf.PerlinNoise(xCoord, zCoord);
    columnHeight = columnHeight / 4 * 100;
    columnHeight = Mathf.RoundToInt(columnHeight);
    return columnHeight;
  }

  public void WelcomeNeighbour(Vector3 neighbourName, bool firstTime)
  {
    if (cubeEditors.ContainsKey(neighbourName))
    {
      cubeEditors[neighbourName].ProcessNeighbours(firstTime);
    }
  }

  private void OnDrawGizmos()
  {
    Gizmos.DrawWireCube(transform.position, new Vector3(chunkHeight, chunkDepth, chunkWidth));
  }

  public void UpdateName()
  {
    gameObject.name = transform.position.x + "," + transform.position.z;
  }
}

