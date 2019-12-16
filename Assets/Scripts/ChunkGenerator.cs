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
  public List<Vector3> cubePositionTable = new List<Vector3>();

  public GenerationSetup[] generationSetups;

  [SerializeField] Vector3[] directions;

  [SerializeField] float distanceForDisabling;
  [SerializeField] float distanceForDestroying;

  List<CubeData> cubesData = new List<CubeData>();

  GameObject player;

  // private void Update()
  // {
  //   if (DistanceToPlayer() > distanceForDestroying)
  //   {
  //     DeGenerateChunk();
  //   }
  //   else if (DistanceToPlayer() > distanceForDisabling)
  //   {
  //     worldManager.DisableChunk(transform.position);
  //   }
  // }

  private void Awake()
  {
    SetBoxCollider();
  }

  private float DistanceToPlayer()
  {
    return Vector3.Distance(transform.position, player.transform.position);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
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

  private void Start()
  {
    worldManager = FindObjectOfType<WorldManager>();
    player = GameObject.FindWithTag("Player");
  }

  public void SetBoxCollider()
  {
    BoxCollider boxCollider = GetComponent<BoxCollider>();
    boxCollider.size = new Vector3(chunkHeight, chunkHeight, chunkHeight);
  }

  public void SetChunkSetup()
  {
    float chunkDistance = FindObjectOfType<WorldManager>().GetChunkDistance();
    chunkWidth = chunkDistance;
    chunkHeight = chunkDistance;
    generationOffset = new Vector3(-chunkDistance / 2, 0, -chunkDistance / 2);
  }

  public void AddNewCube(CubeEditor cubeEditor)
  {
    if (!cubeEditorTable.ContainsKey(cubeEditor.transform.position))
    {
      cubeEditorTable.Add(cubeEditor.transform.position, cubeEditor);
    }
    if (cubePositionTable.Contains(cubeEditor.transform.position))
    {
      cubePositionTable.Add(cubeEditor.transform.position);
    }
    CubeData cubeData = new CubeData(cubeEditor);
    cubesData.Add(cubeData);

  }

  public void RemoveCube(CubeEditor cubeEditor)
  {
    Destroy(cubeEditor.gameObject);
    if (cubeEditorTable.ContainsKey(cubeEditor.transform.position))
    {
      cubeEditorTable.Remove(cubeEditor.transform.position);
    }
    if (cubePositionTable.Contains(cubeEditor.transform.position))
    {
      cubePositionTable.Remove(cubeEditor.transform.position);
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
          currentCubeEditor.SetCubeParent(this);
          currentCubeEditor.SetCubeType(ProcessCubeType(y));
          if (!cubeEditorTable.ContainsKey(currentCubeEditor.transform.position))
          {
            AddNewCube(currentCubeEditor);
          }
          else
          {
            print("removing cube");
            RemoveCube(currentCubeEditor);
          }
        }
      }
    }
  }

  public void DeGenerateChunk()
  {
    // for (int x = 0; x < chunkHeight; x++)
    // {
    //   for (int z = 0; z < chunkWidth; z++)
    //   {
    //     float columnHeight = GenerateHeight(x, z);
    //     for (int y = 0; y < columnHeight; y++)
    //     {
    //       Vector3 cubeLocation = transform.position + new Vector3(x, y, z) + generationOffset;
    //       if (cubeEditorTable.ContainsKey(cubeLocation))
    //       {

    //       }
    //       CubeEditor currentCubeEditor = cubeEditorTable[cubeLocation];
    //       Destroy(currentCubeEditor.gameObject);
    //     }
    //   }
    // }
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
}

