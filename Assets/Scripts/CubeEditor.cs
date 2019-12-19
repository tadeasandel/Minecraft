using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEditor : MonoBehaviour
{
  // offset with tag names used to identify potencial new cube spawn location from player
  Dictionary<string, Vector3> sideOffset = new Dictionary<string, Vector3>();
  //dictionary with access to all 6 sides with Vector3
  public Dictionary<Vector3, GameObject> childTable = new Dictionary<Vector3, GameObject>();

  // Verticles of the cube, used to draw lines in building mode
  [SerializeField] Vector3[] verticles;
  [SerializeField] int gridSize;


  ChunkGenerator cubeParent;

  [Header("Cube Setup")]
  public CubeType currentCubeType;
  float toughness;

  // positions of possible cube neighbours
  Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down };

  // Sets this cube's ChunkGenerator parent
  public void SetCubeParent(ChunkGenerator chunkGenerator)
  {
    cubeParent = chunkGenerator;
  }

  // calls parent to create block by player with position and CubeType
  public void CreateBlock(string sideTag, CubeType cubeType)
  {
    cubeParent.CreateCube(this, transform.position + sideOffset[sideTag], cubeType);
  }

  // gets called when cube is destroyed by player
  public void DestroyBlock()
  {
    AdjustNeighbours();
    DestroyCube();
  }

  // getter for this Cube's CubeType
  public CubeType GetCubeType()
  {
    return currentCubeType;
  }

  // sets this cube's CubeType
  public void SetCubeType(CubeType cubeType)
  {
    if (cubeType == null) { return; }
    currentCubeType = cubeType;
  }

  // after setting this cube's CubeType this updates local values to it's responsive CubeType
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

  private void Start()
  {
    if (cubeParent == null)
    {
      print("getting parent");
      cubeParent = GetComponentInParent<ChunkGenerator>();
    }
    SetOffset();
    ApplyCubeType();
    ProcessNeighbours(true);
  }

  // Processes all neighbours surrounding the cube
  // Disables all sides which get covered by another cube
  // Calls the neighbour cubes to also cover their sides towards this cube
  public void ProcessNeighbours(bool firstTime)
  {
    for (int i = 0; i < 6; i++)
    {
      Vector3 neighbourCube = transform.position + directions[i];
      CubeEditor temporaryCubeEditor;
      if (cubeParent == null) { print("getting parent"); cubeParent = GetComponentInParent<ChunkGenerator>(); }
      if (childTable.ContainsKey(directions[i]))
      {
        RevealTransform(childTable[directions[i]]);
      }
      if (cubeParent.DoesHaveNeighbour(neighbourCube))
      {
        if (childTable.ContainsKey(directions[i]))
        {
          HideSideTransform(childTable[directions[i]]);
        }
        if (firstTime)
        {
          temporaryCubeEditor = cubeParent.GetCubeEditorByVector(neighbourCube);
          temporaryCubeEditor.ProcessNeighbours(false);
        }
      }
      else
      {
        temporaryCubeEditor = cubeParent.GetEditorFromNeighbourChunk(neighbourCube);
        if (temporaryCubeEditor == null) { continue; }
        HideTransformByVector(directions[i]);
        if (temporaryCubeEditor.childTable.ContainsKey(-directions[i]))
        {
          temporaryCubeEditor.HideTransformByVector(-directions[i]);
        }
      }
    }
  }

  // updates name based on it's location + CubeType name
  public void UpdateName()
  {
    if (transform.name != transform.position.ToString())
    {
      transform.name = currentCubeType.cubeTypeName + " " + transform.position.ToString();
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

  // gets a center point of a pre-visualizing block before placed
  public Vector3 GetVizualizationCenter(string sideTag)
  {
    return transform.position + sideOffset[sideTag];
  }

  // gets called when this cube is being destroyed
  // Refreshes other surrounding cubes
  private void AdjustNeighbours()
  {
    for (int i = 0; i < 6; i++)
    {
      Vector3 neighbourCube = transform.position + directions[i];
      if (cubeParent == null) { print("getting parent"); cubeParent = GetComponentInParent<ChunkGenerator>(); }
      CubeEditor temporaryCubeEditor = cubeParent.GetCubeEditorByVector(neighbourCube);
      if (temporaryCubeEditor == null) { continue; }
      if (temporaryCubeEditor.childTable.ContainsKey(-directions[i]))
      {
        temporaryCubeEditor.childTable[-directions[i]].gameObject.SetActive(true);
      }
      else
      {
        temporaryCubeEditor = cubeParent.GetEditorFromNeighbourChunk(neighbourCube);
        if (temporaryCubeEditor == null) { continue; }
        if (temporaryCubeEditor.childTable.ContainsKey(-directions[i]))
        {
          temporaryCubeEditor.HideTransformByVector(-directions[i]);
        }
      }
    }
  }

  // hides side GameObject by Vector3
  private void HideTransformByVector(Vector3 transformVector)
  {
    if (childTable.ContainsKey(transformVector))
    {
      childTable[transformVector].SetActive(false);
    }
  }

  private void RevealTransform(GameObject sideObject)
  {
    sideObject.SetActive(true);
  }

  private void HideSideTransform(GameObject sideObject)
  {
    sideObject.SetActive(false);
  }

  // Gets called upon creation
  // Fills all required information a dictionary needs for offsets
  private void SetOffset()
  {
    sideOffset.Add("FrontBlock", new Vector3(0f, 0f, 1f));
    sideOffset.Add("BackBlock", new Vector3(0f, 0f, -1f));
    sideOffset.Add("LeftBlock", new Vector3(-1f, 0f, 0f));
    sideOffset.Add("RightBlock", new Vector3(1f, 0f, 0f));
    sideOffset.Add("TopBlock", new Vector3(0f, 1f, 0f));
    sideOffset.Add("DownBlock", new Vector3(0f, -1f, 0f));
  }

  // snaps the cube to it's grid position. No longer used
  private void SnapToGridPosition()
  {
    transform.position = new Vector3(Mathf.RoundToInt(transform.position.x * gridSize), Mathf.RoundToInt(transform.position.y * gridSize), Mathf.RoundToInt(transform.position.z * gridSize));
  }

  private void DrawLineWithIndexes(string sideTag, int firstIndex, int secondIndex)
  {
    Gizmos.DrawLine(transform.position + verticles[firstIndex] + sideOffset[sideTag], transform.position + verticles[secondIndex] + sideOffset[sideTag]);
  }


  // calls parent to destroy this cube
  private void DestroyCube()
  {
    cubeParent.RemoveCube(this);
  }
}
