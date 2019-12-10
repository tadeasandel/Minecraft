using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractionController : MonoBehaviour
{

  [SerializeField] float maxDistance;

  [SerializeField] string[] tags;

  bool isInConstrutorMode = true;

  ItemTargetSwitcher itemTargetSwitcher;

  struct Target
  {
    public CubeEditor targetCube;
    public string sideTag;
  }

  private void Start()
  {
    itemTargetSwitcher = FindObjectOfType<ItemTargetSwitcher>();
  }

  Target target;

  void Update()
  {
    ProcessRaycast();
  }

  public void EnableMode()
  {

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
            if (Input.GetButtonDown("Fire1"))
            {
              target.targetCube.CreateBlock(target.sideTag, itemTargetSwitcher.GetCubeType());
            }
          }
        }
      }
    }
    else
    {
      target.targetCube = null;
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
    else
    {
      Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
    }
  }
}
