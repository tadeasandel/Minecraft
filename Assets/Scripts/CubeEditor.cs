using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CubeEditor : MonoBehaviour
{
  Dictionary<string, Vector3> sideOffset = new Dictionary<string, Vector3>();
  public Dictionary<Vector3, GameObject> childTable = new Dictionary<Vector3, GameObject>();

  [SerializeField] Vector3[] verticles;

  [SerializeField] int gridSize;

  [SerializeField] CubeType currentCubeType;

  ChunkGenerator cubeParent;

  [SerializeField] GameObject cubePrefab;

  float toughness;

  Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down };

  private void Start()
  {
    cubeParent = GetComponentInParent<ChunkGenerator>();
    SetOffset();
    SetCubeType(currentCubeType);
    ApplyCubeType();
    SnapToGridPosition();
    UpdateName();
    AddCubeToParent();
    ProcessNeighbours(true);
  }

  private void AddCubeToParent()
  {
    cubeParent.AddNewCube(this);
  }

  private void RemoveCubeFromParent()
  {
    cubeParent.RemoveCube(this);
  }

  public void ProcessNeighbours(bool firstTime)
  {
    for (int i = 0; i < 6; i++)
    {
      Vector3 neighbourCube = transform.position + directions[i];
      cubeParent = GetComponentInParent<ChunkGenerator>();
      if (childTable.ContainsKey(directions[i]))
      {
        RevealTransform(childTable[directions[i]]);
      }
      if (cubeParent.DoesHaveNeighbour(neighbourCube.ToString()))
      {
        if (childTable.ContainsKey(directions[i]))
        {
          HideSideTransform(childTable[directions[i]]);
        }
        if (firstTime)
        {
          CubeEditor temporaryCubeEditor = cubeParent.GetCubeEditorByIndex(neighbourCube.ToString());
          temporaryCubeEditor.ProcessNeighbours(false);
        }
      }
    }
  }

  private void AdjustNeighbours()
  {
    for (int i = 0; i < 6; i++)
    {
      Vector3 neighbourCube = transform.position + directions[i];
      cubeParent = GetComponentInParent<ChunkGenerator>();
      if (cubeParent.DoesHaveNeighbour(neighbourCube.ToString()))
      {
        CubeEditor temporaryCubeEditor = cubeParent.GetCubeEditorByIndex(neighbourCube.ToString());
        if (temporaryCubeEditor.childTable.ContainsKey(-directions[i]))
        {
          temporaryCubeEditor.childTable[-directions[i]].gameObject.SetActive(true);
        }
      }
    }
  }

  private void RevealTransform(GameObject sideObject)
  {
    sideObject.SetActive(true);
  }

  public void HideSideTransform(GameObject sideObject)
  {
    sideObject.SetActive(false);
  }

  private void SetOffset()
  {
    sideOffset.Add("FrontBlock", new Vector3(0f, 0f, 1f));
    sideOffset.Add("BackBlock", new Vector3(0f, 0f, -1f));
    sideOffset.Add("LeftBlock", new Vector3(-1f, 0f, 0f));
    sideOffset.Add("RightBlock", new Vector3(1f, 0f, 0f));
    sideOffset.Add("TopBlock", new Vector3(0f, 1f, 0f));
    sideOffset.Add("DownBlock", new Vector3(0f, -1f, 0f));
  }

  public CubeType GetCubeType()
  {
    return currentCubeType;
  }

  public void SetCubeType(CubeType cubeType)
  {
    if (cubeType == null) { return; }
    currentCubeType = cubeType;
  }

  public void ApplyCubeType()
  {
    toughness = currentCubeType.toughness;
    for (int i = 0; i < 6; i++)
    {
      if (currentCubeType.materials.Length != 6) { break; }
      transform.GetChild(i).GetComponent<MeshRenderer>().material = currentCubeType.materials[i];
      if (!childTable.ContainsKey(directions[i]))
      {
        childTable.Add(directions[i], transform.GetChild(i).gameObject);
      }
    }
  }

  private void SnapToGridPosition()
  {
    transform.position = new Vector3(Mathf.RoundToInt(transform.position.x * gridSize), Mathf.RoundToInt(transform.position.y * gridSize), Mathf.RoundToInt(transform.position.z * gridSize));
  }

  public void UpdateName()
  {
    if (transform.name != transform.position.ToString())
    {
      transform.name = transform.position.ToString();
    }
  }

  public void VisualizeBlock(string sideTag)
  {
    DrawLineWithIndexes(sideTag, 0, 1);
    DrawLineWithIndexes(sideTag, 1, 2);
    DrawLineWithIndexes(sideTag, 2, 3);
    DrawLineWithIndexes(sideTag, 3, 0);
    DrawLineWithIndexes(sideTag, 0, 4);
    DrawLineWithIndexes(sideTag, 1, 5);
    DrawLineWithIndexes(sideTag, 2, 6);
    DrawLineWithIndexes(sideTag, 3, 7);
    DrawLineWithIndexes(sideTag, 4, 5);
    DrawLineWithIndexes(sideTag, 5, 6);
    DrawLineWithIndexes(sideTag, 6, 7);
    DrawLineWithIndexes(sideTag, 7, 4);
  }

  private void DrawLineWithIndexes(string sideTag, int firstIndex, int secondIndex)
  {
    Gizmos.DrawLine(transform.position + verticles[firstIndex] + sideOffset[sideTag], transform.position + verticles[secondIndex] + sideOffset[sideTag]);
  }

  public void CreateBlock(string sideTag, CubeType cubeType)
  {
    GameObject newCube = Instantiate(cubePrefab, transform.position + sideOffset[sideTag], Quaternion.identity, cubeParent.transform);
    newCube.GetComponent<CubeEditor>().SetCubeType(cubeType);
  }

  public void DestroyBlock()
  {
    AdjustNeighbours();
    RemoveCubeFromParent();
    Destroy(gameObject);
  }
}
