using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractionController : MonoBehaviour
{

  [SerializeField] float maxDistance;

  [SerializeField] string[] tags;

  bool isInConstrutorMode = true;

  struct Target
  {
    public CubeEditor targetCube;
    public string sideTag;
  }

  Target target;

  void Update()
  {
    ProcessRaycast();
  }

  private void ProcessRaycast()
  {
    RaycastHit hit;
    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
    {
      foreach (string tag in tags)
      {
        if (hit.transform.tag == tag)
        {
          target.sideTag = hit.transform.tag;
          target.targetCube = hit.transform.GetComponent<CubeEditor>();
          if (isInConstrutorMode)
          {
            VisualizeGrid(hit);
          }
        }
      }
    }
  }

  private void VisualizeGrid(RaycastHit hit)
  {
    target.targetCube = hit.transform.GetComponentInParent<CubeEditor>();
  }

  private void OnDrawGizmos()
  {
    if (target.targetCube != null)
    {
      target.targetCube.VisualizeBlock(target.sideTag);
    }
  }
}
