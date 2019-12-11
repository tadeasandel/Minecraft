using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CubeEditor : MonoBehaviour
{
  Dictionary<string, Vector3> sideOffset = new Dictionary<string, Vector3>();

  [SerializeField] Vector3[] verticles;

  [SerializeField] int gridSize;

  [SerializeField] CubeType currentCubeType;

  float toughness;

  private void Start()
  {
    SetOffset();
    SetCubeType(currentCubeType);
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
    toughness = cubeType.toughness;
    for (int i = 0; i < 6; i++)
    {
      if (cubeType.materials.Length != 6) { break; }
      transform.GetChild(i).GetComponent<MeshRenderer>().material = cubeType.materials[i];
    }
  }

  private void Update()
  {
    SnapToGridPosition();
    UpdateName();
  }

  private void SnapToGridPosition()
  {
    transform.position = new Vector3(Mathf.RoundToInt(transform.position.x * gridSize), Mathf.RoundToInt(transform.position.y * gridSize), Mathf.RoundToInt(transform.position.z * gridSize));
  }

  private void UpdateName()
  {
    string positionName = currentCubeType.cubeTypeName + " " + transform.position.x + "," + transform.position.y + "," + transform.position.z;
    if (transform.name != positionName)
    {
      transform.name = positionName;
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
    CubeEditor newCube = Instantiate(this, transform.position + sideOffset[sideTag], Quaternion.identity);
    newCube.SetCubeType(cubeType);
  }

  public void DestroyBlock()
  {
    Destroy(gameObject);
  }
}
