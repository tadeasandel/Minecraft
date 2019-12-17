using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
  float chunkHeight;
  float chunkWidth;
  [SerializeField] float maxDepth;

  [SerializeField] float perlinScale = 0.1f;

  [SerializeField] CubeEditor basicCubePrefab;

  WorldManager worldManager;

  Vector3 generationOffset;
  float perlinOffsetX;
  float perlinOffsetZ;

  public Dictionary<Vector3, CubeEditor> cubeEditorTable = new Dictionary<Vector3, CubeEditor>();

  public GenerationSetup[] generationSetups;

  [SerializeField] Vector3[] directions;

  private void Awake()
  {
    SetBoxCollider();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      if (worldManager == null) { return; }
      worldManager.RefreshChunks(transform.position);
    }
  }

  public CubeEditor GetCubeEditorByVector(Vector3 cubeEditorPos)
  {
    if (cubeEditorTable.ContainsKey(cubeEditorPos))
    {
      return cubeEditorTable[cubeEditorPos];
    }
    else
    {
      for (int i = 0; i < 4; i++)
      {
        Vector3 neighbourChunkPos = transform.position + directions[i];
        ChunkGenerator neighbourChunk = worldManager.GetChunkGeneratorByVector(neighbourChunkPos);
        if (neighbourChunk == null) { continue; }
        cubeEditorPos = TransformCubeToNeighbourPos(cubeEditorPos);
        if (!neighbourChunk.cubeEditorTable.ContainsKey(cubeEditorPos)) { continue; }
        CubeEditor neighbourCube = neighbourChunk.cubeEditorTable[cubeEditorPos];
        return neighbourCube;
      }
    }
    return null;
  }

  public bool DoesHaveNeighbour(Vector3 neighbourPos)
  {
    if (cubeEditorTable.ContainsKey(neighbourPos))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  public CubeEditor GetEditorFromNeighbourChunk(Vector3 neighbourPos)
  {
    for (int i = 0; i < 4; i++)
    {
      Vector3 neighbourChunkPos = transform.position + directions[i];
      if (worldManager == null) { print("getting parent"); worldManager = GetComponentInParent<WorldManager>(); }
      ChunkGenerator neighbourChunk = worldManager.GetChunkGeneratorByVector(neighbourChunkPos);
      if (neighbourChunk == null) { continue; }
      neighbourPos = TransformCubeToNeighbourPos(neighbourPos);
      if (!neighbourChunk.cubeEditorTable.ContainsKey(neighbourPos)) { continue; }
      CubeEditor neighbourCube = neighbourChunk.cubeEditorTable[neighbourPos];
      return neighbourCube;
    }
    return null;
  }

  public Vector3 TransformCubeToNeighbourPos(Vector3 neighbourPos)
  {
    if (neighbourPos == new Vector3(0, 0, chunkHeight))
    {
      neighbourPos = new Vector3(neighbourPos.x, neighbourPos.y, neighbourPos.z - chunkHeight + 1);
    }
    else if (neighbourPos == new Vector3(0, 0, -chunkHeight))
    {
      neighbourPos = new Vector3(neighbourPos.x, neighbourPos.y, neighbourPos.z + chunkHeight - 1);
    }
    else if (neighbourPos == new Vector3(chunkHeight, 0, 0))
    {
      neighbourPos = new Vector3(neighbourPos.x - chunkHeight + 1, neighbourPos.y, neighbourPos.z);
    }
    else if (neighbourPos == new Vector3(-chunkHeight, 0, 0))
    {
      neighbourPos = new Vector3(neighbourPos.x + chunkHeight - 1, neighbourPos.y, neighbourPos.z);
    }
    return neighbourPos;
  }

  public void SetBoxCollider()
  {
    BoxCollider boxCollider = GetComponent<BoxCollider>();
    boxCollider.size = new Vector3(chunkHeight, chunkHeight, chunkHeight);
  }

  public void SetChunkSetup()
  {
    worldManager = GetComponentInParent<WorldManager>();
    float chunkDistance = worldManager.GetChunkDistance();
    chunkWidth = chunkDistance;
    chunkHeight = chunkDistance;
    generationOffset = new Vector3(-chunkDistance / 2, 0, -chunkDistance / 2);
  }

  public void CreateCube(CubeEditor cubeEditor, Vector3 cubePos, CubeType cubeType)
  {
    CubeEditor currentCubeEditor = Instantiate(basicCubePrefab, cubePos, Quaternion.identity, transform);
    currentCubeEditor.UpdateName();
    currentCubeEditor.SetCubeParent(this);
    currentCubeEditor.SetCubeType(cubeType);

    if (!cubeEditorTable.ContainsKey(currentCubeEditor.transform.position))
    {
      cubeEditorTable.Add(currentCubeEditor.transform.position, currentCubeEditor);
    }
  }

  public void RemoveCube(CubeEditor cubeEditor)
  {
    Destroy(cubeEditor.gameObject);
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
          CubeType cubeType = ProcessCubeType(y);
          if (!cubeEditorTable.ContainsKey(newCubeLocation))
          {
            CreateCube(basicCubePrefab, newCubeLocation, cubeType);
          }
        }
      }
    }
  }

  public void GenerateLoadedChunk(WorldData.ChunkData chunkData)
  {
    for (int i = 0; i < chunkData.cubePositionsX.Count; i++)
    {
      Vector3 cubePos = new Vector3(chunkData.cubePositionsX[i], chunkData.cubePositionsY[i], chunkData.cubePositionsZ[i]);
      CubeType cubeType = ProcessCubeByName(chunkData.cubeTypeNames[i]);
      CreateCube(basicCubePrefab, cubePos, cubeType);
    }
  }

  private CubeType ProcessCubeType(int cubeDepth)
  {
    foreach (GenerationSetup generationSetup in generationSetups)
    {
      if (cubeDepth >= generationSetup.minGenerationDepth && cubeDepth <= generationSetup.maxGenerationDepth)
      {
        return generationSetup.generatedCubeType;
      }
    }
    return generationSetups[3].generatedCubeType;
  }

  public CubeType ProcessCubeByName(string name)
  {
    foreach (GenerationSetup generationSetup in generationSetups)
    {
      if (generationSetup.cubeTypeName == name)
      {
        return generationSetup.generatedCubeType;
      }
    }
    return generationSetups[3].generatedCubeType;
  }

  private float GenerateHeight(int x, int z)
  {
    float xCoord = (float)x / chunkHeight + perlinOffsetX;
    float zCoord = (float)z / chunkWidth + perlinOffsetZ;
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

  public void UpdateName()
  {
    gameObject.name = transform.position.ToString();
  }
}

[System.Serializable]
public class GenerationSetup
{
  public CubeType generatedCubeType;
  public float maxGenerationDepth;
  public float minGenerationDepth;
  public string cubeTypeName;
}

