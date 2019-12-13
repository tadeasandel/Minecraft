using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
  [SerializeField] float chunkHeight;
  [SerializeField] float chunkWidth;
  [SerializeField] float chunkDepth;

  [SerializeField] CubeEditor basicCubePrefab;

  WorldManager worldManager;

  [SerializeField] Vector3 offset;

  Dictionary<string, CubeEditor> cubeEditors = new Dictionary<string, CubeEditor>();

  struct Cube
  {
    CubeEditor cubeEditor;
    CubeType cubeType;
  }

  public CubeEditor GetCubeEditorByIndex(string cubeEditorName)
  {
    return cubeEditors[cubeEditorName];
  }

  public bool DoesHaveNeighbour(string neighbourName)
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
    if (!worldManager.IsChunkGenerated(this))
    {
      GenerateChunk();
    }
  }

  public void AddNewCube(CubeEditor cubeEditor)
  {
    if (!cubeEditors.ContainsKey(cubeEditor.name))
    {
      print("adding " + cubeEditor.name);
      cubeEditors.Add(cubeEditor.name, cubeEditor);
    }
  }

  private void GenerateChunk()
  {
    for (int x = 0; x < chunkHeight; x++)
    {
      for (int y = 0; y < chunkDepth; y++)
      {
        for (int z = 0; z < chunkHeight; z++)
        {
          Vector3 newCubeLocation = transform.position + new Vector3((float)x, (float)y, (float)z) + offset;
          CubeEditor currentCubeEditor = Instantiate(basicCubePrefab, newCubeLocation, Quaternion.identity, transform);
          currentCubeEditor.UpdateName();
          if (!cubeEditors.ContainsKey(currentCubeEditor.name))
          {
            cubeEditors.Add(currentCubeEditor.name, currentCubeEditor);
          }
        }
      }
    }
  }

  public void WelcomeNeighbour(string neighbourName, bool firstTime)
  {
    if (cubeEditors.ContainsKey(neighbourName))
    {
      cubeEditors[neighbourName].ProcessNeightbours(firstTime);
    }
  }

  private void OnDrawGizmos()
  {
    Gizmos.DrawWireCube(transform.position, new Vector3(chunkHeight - 0.5f, chunkDepth, chunkWidth - 0.5f));
  }

  private void Update()
  {
    UpdateName();
  }

  private void UpdateName()
  {
    gameObject.name = transform.position.x + "," + transform.position.z;
  }
}

