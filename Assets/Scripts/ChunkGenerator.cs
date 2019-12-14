using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
  [SerializeField] float chunkHeight;
  [SerializeField] float chunkWidth;
  [SerializeField] float maxDepth;

  [SerializeField] float perlinScale = 0.1f;

  [SerializeField] CubeEditor basicCubePrefab;

  WorldManager worldManager;

  [SerializeField] Vector3 generationOffset;
  float perlinOffsetX;
  float perlinOffsetZ;

  Dictionary<Vector3, CubeEditor> cubeEditors = new Dictionary<Vector3, CubeEditor>();

  public GenerationSetup[] generationSetups;

  [SerializeField] Vector3[] directions;

  public CubeEditor GetCubeEditorByVector(Vector3 cubeEditorName)
  {
    if (cubeEditors.ContainsKey(cubeEditorName))
    {
      return cubeEditors[cubeEditorName];
    }
    else
    {
      return null;
    }
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

  public CubeEditor GetEditorFromNeighbourChunk(Vector3 neighbourName)
  {
    for (int i = 0; i < 4; i++)
    {
      Vector3 neighbourChunkPos = transform.position + directions[i];
      ChunkGenerator neighbourChunk = worldManager.GetChunkGeneratorByVector(neighbourChunkPos);
      if (neighbourChunk == null) { continue; }
      neighbourName = TransformCubeToNeighbourPos(neighbourName);
      CubeEditor neighbourCube = neighbourChunk.cubeEditors[neighbourName];
      return neighbourCube;
    }
    return null;
  }

  public Vector3 TransformCubeToNeighbourPos(Vector3 neighbourPos)
  {
    if (neighbourPos == new Vector3(0, 0, 20))
    {
      neighbourPos = new Vector3(neighbourPos.x, neighbourPos.y, neighbourPos.z - 19);
    }
    else if (neighbourPos == new Vector3(0, 0, -20))
    {
      neighbourPos = new Vector3(neighbourPos.x, neighbourPos.y, neighbourPos.z + 19);
    }
    else if (neighbourPos == new Vector3(20, 0, 0))
    {
      neighbourPos = new Vector3(neighbourPos.x - 19, neighbourPos.y, neighbourPos.z);
    }
    else if (neighbourPos == new Vector3(-20, 0, 0))
    {
      neighbourPos = new Vector3(neighbourPos.x + 19, neighbourPos.y, neighbourPos.z);
    }

    return neighbourPos;
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
          currentCubeEditor.SetCubeType(ProcessCubeType(y));
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

  private CubeType ProcessCubeType(int y)
  {
    foreach (GenerationSetup generationSetup in generationSetups)
    {
      if (y >= generationSetup.minGenerationDepth && y <= generationSetup.maxGenerationDepth)
      {
        return generationSetup.generatedCubeType;
      }
      else
      {
        continue;
      }
    }
    return generationSetups[3].generatedCubeType;
  }

  private float GenerateHeight(int x, int z)
  {
    float xCoord = (float)x / chunkHeight + perlinOffsetZ;
    float zCoord = (float)z / chunkWidth + perlinOffsetX;
    float columnHeight = Mathf.PerlinNoise(xCoord, zCoord);
    columnHeight = columnHeight / 4 * 100;
    columnHeight = Mathf.RoundToInt(columnHeight);
    columnHeight = Mathf.Clamp(columnHeight, 1, maxDepth);
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
    Gizmos.DrawWireCube(transform.position, new Vector3(chunkHeight, maxDepth, chunkWidth));
  }

  public void UpdateName()
  {
    gameObject.name = transform.position.x + "," + transform.position.z;
  }
}

[System.Serializable]
public class GenerationSetup
{
  public CubeType generatedCubeType;
  public float maxGenerationDepth;
  public float minGenerationDepth;
}

