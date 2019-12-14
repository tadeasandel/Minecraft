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

  public Dictionary<Vector3, CubeEditor> cubeEditorTable = new Dictionary<Vector3, CubeEditor>();

  public GenerationSetup[] generationSetups;

  [SerializeField] Vector3[] directions;

  public CubeEditor GetCubeEditorByVector(Vector3 cubeEditorName)
  {
    if (cubeEditorTable.ContainsKey(cubeEditorName))
    {
      return cubeEditorTable[cubeEditorName];
    }
    else
    {
      return null;
    }
  }

  public bool DoesHaveNeighbour(Vector3 neighbourName)
  {
    if (cubeEditorTable.ContainsKey(neighbourName))
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
      if (!neighbourChunk.cubeEditorTable.ContainsKey(neighbourName)) { continue; }
      CubeEditor neighbourCube = neighbourChunk.cubeEditorTable[neighbourName];
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
  }

  public void AddNewCube(CubeEditor cubeEditor)
  {
    if (!cubeEditorTable.ContainsKey(cubeEditor.transform.position))
    {
      cubeEditorTable.Add(cubeEditor.transform.position, cubeEditor);
    }
  }

  public void RemoveCube(CubeEditor cubeEditor)
  {
    if (cubeEditorTable.ContainsKey(cubeEditor.transform.position))
    {
      cubeEditorTable.Remove(cubeEditor.transform.position);
    }
  }

  public void GenerateChunk(ChunkPerlinOffsets chunkPerlinOffsets)
  {
    perlinOffsetX = chunkPerlinOffsets.chunkOffsetX;
    perlinOffsetZ = chunkPerlinOffsets.chunkOffsetZ;
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
          if (!cubeEditorTable.ContainsKey(currentCubeEditor.transform.position))
          {
            cubeEditorTable.Add(currentCubeEditor.transform.position, currentCubeEditor);
          }
          else
          {
            Destroy(currentCubeEditor.gameObject);
          }
        }
      }
    }
  }

  public void DeGenerateChunk()
  {

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
    if (cubeEditorTable.ContainsKey(neighbourName))
    {
      cubeEditorTable[neighbourName].ProcessNeighbours(firstTime);
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

