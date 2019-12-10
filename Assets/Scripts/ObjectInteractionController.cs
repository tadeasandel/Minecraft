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

  ToolType currentToolType;
  ToolType defaultToolType;

  float timeSincePlacedBlock = Mathf.Infinity;
  [SerializeField] float blockPlaceTimer = 0.3f;

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
    if (currentToolType != null)
    {
      isInConstrutorMode = false;
    }
    else
    {
      isInConstrutorMode = true;
      currentToolType = defaultToolType;
    }
    ProcessRaycast();
    timeSincePlacedBlock += Time.deltaTime;
  }

  public void EnableMode(ToolType toolType)
  {
    if (toolType != null)
    {
      currentToolType = toolType;
    }
  }

  private void ProcessRaycast()
  {
    RaycastHit hit;
    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
    {
      ProcessCubeTargeting(hit);
    }
    else
    {
      target.targetCube = null;
    }
  }

  private void ProcessCubeTargeting(RaycastHit hit)
  {
    foreach (string tag in tags)
    {
      if (hit.transform.tag == tag)
      {
        target.sideTag = hit.transform.tag;
        target.targetCube = hit.transform.GetComponentInParent<CubeEditor>();
        ProcessMining(hit);

        if (isInConstrutorMode)
        {
          ProcessBuilding(hit);
        }
      }
    }
  }

  private void ProcessMining(RaycastHit hit)
  {
    if (Input.GetButton("Fire1"))
    {
      target.targetCube.DestroyBlock();
    }
  }

  public void ProcessBuilding(RaycastHit hit)
  {
    VisualizeGrid(hit);
    if (Input.GetButtonDown("Fire2"))
    {
      timeSincePlacedBlock = 0;
      target.targetCube.CreateBlock(target.sideTag, itemTargetSwitcher.GetCubeType());
      return;
    }
    else if (Input.GetButton("Fire2") && blockPlaceTimer < timeSincePlacedBlock)
    {
      timeSincePlacedBlock = 0;
      target.targetCube.CreateBlock(target.sideTag, itemTargetSwitcher.GetCubeType());
    }
  }

  private void VisualizeGrid(RaycastHit hit)
  {
    target.targetCube = hit.transform.GetComponentInParent<CubeEditor>();
  }

  private void OnDrawGizmos()
  {
    if (target.targetCube != null && isInConstrutorMode)
    {
      target.targetCube.VisualizeBlock(target.sideTag);
    }
  }
}
