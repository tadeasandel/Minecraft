using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEditor : MonoBehaviour
{
  [SerializeField] Transform frontTransform;
  [SerializeField] Transform backTransform;
  [SerializeField] Transform leftTransform;
  [SerializeField] Transform rightTransform;
  [SerializeField] Transform topTransform;
  [SerializeField] Transform downTransform;

  [SerializeField] Vector3[] frontTransformPoints;
  [SerializeField] Vector3[] backTransformPoints;
  [SerializeField] Vector3[] leftTransformPoints;
  [SerializeField] Vector3[] rightTransformPoints;
  [SerializeField] Vector3[] topTransformPoints;
  [SerializeField] Vector3[] downTransformPoints;



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

  Vector3[] cubePoints = new Vector3[8];

  public void DrawSideLines(Vector3[] sides, Transform sideTransform)
  {
    Gizmos.DrawLine(sideTransform.position + sides[0], sideTransform.position + sides[1]);
    Gizmos.DrawLine(sideTransform.position + sides[1], sideTransform.position + sides[2]);
    Gizmos.DrawLine(sideTransform.position + sides[2], sideTransform.position + sides[3]);
    Gizmos.DrawLine(sideTransform.position + sides[3], sideTransform.position + sides[0]);
    Gizmos.DrawLine(sideTransform.position + sides[0], sideTransform.position + sides[4]);
    Gizmos.DrawLine(sideTransform.position + sides[1], sideTransform.position + sides[5]);
    Gizmos.DrawLine(sideTransform.position + sides[2], sideTransform.position + sides[6]);
    Gizmos.DrawLine(sideTransform.position + sides[3], sideTransform.position + sides[7]);
    Gizmos.DrawLine(sideTransform.position + sides[4], sideTransform.position + sides[5]);
    Gizmos.DrawLine(sideTransform.position + sides[5], sideTransform.position + sides[6]);
    Gizmos.DrawLine(sideTransform.position + sides[6], sideTransform.position + sides[7]);
    Gizmos.DrawLine(sideTransform.position + sides[7], sideTransform.position + sides[4]);
  }

  public void VisualizeBlock(string sideTag)
  {
    print("visualizing on " + sideTag);
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
