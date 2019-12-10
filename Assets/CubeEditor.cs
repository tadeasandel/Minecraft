using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEditor : MonoBehaviour
{
  Dictionary<string, Vector3> sideOffset = new Dictionary<string, Vector3>();

  [SerializeField] Vector3[] verticles;

  private void Start()
  {
    sideOffset.Add("FrontBlock", new Vector3(0f, 0f, 1f));
    sideOffset.Add("BackBlock", new Vector3(0f, 0f, -1f));
    sideOffset.Add("LeftBlock", new Vector3(-1f, 0f, 0f));
    sideOffset.Add("RightBlock", new Vector3(1f, 0f, 0f));
    sideOffset.Add("TopBlock", new Vector3(0f, 1f, 0f));
    sideOffset.Add("DownBlock", new Vector3(0f, -1f, 0f));
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

  public void CreateBlock(string sideTag)
  {
    Instantiate(this, transform.position + sideOffset[sideTag], Quaternion.identity);
  }
}
