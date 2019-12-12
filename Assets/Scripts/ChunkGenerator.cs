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

  private void Start()
  {
    worldManager = FindObjectOfType<WorldManager>();
    if (!worldManager.IsChunkGenerated(this))
    {
      GenerateChunk();
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
          if (cubeEditors.ContainsKey(currentCubeEditor.name))
          {
            cubeEditors.Add(currentCubeEditor.name, currentCubeEditor.GetComponent<CubeEditor>());
          }
        }
      }
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

