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
  [SerializeField] ToolType defaultToolType;

  CubeType currentCubeType;

  string currentObjectName;

  float timeSinceDestroyedBlock = Mathf.Infinity;
  [SerializeField] float blockDestroyTimer = 0.3f;

  float timeSincePlacedBlock = Mathf.Infinity;
  [SerializeField] float blockPlaceTimer = 0.3f;

  float cubeToughness;

  bool isDigging = false;

  float miningCalculation;
  float miningTime;

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
    UpdateTimers();
  }

  private void UpdateTimers()
  {
    timeSincePlacedBlock += Time.deltaTime;
    timeSinceDestroyedBlock += Time.deltaTime;
  }

  public void EnableMode(ToolType toolType, CubeType cubeType)
  {
    ActivateCubeType(cubeType);
    ActiveToolType(toolType);
  }

  private void ActiveToolType(ToolType toolType)
  {
    if (toolType == null)
    {
      currentToolType = defaultToolType;
      return;
    }
    currentToolType = toolType;
  }

  private void ActivateCubeType(CubeType cubeType)
  {
    currentCubeType = cubeType;
    if (currentCubeType != null)
    {
      isInConstrutorMode = true;
    }
    else
    {
      isInConstrutorMode = false;
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
      miningTime = 0;
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
    if (Input.GetButton("Fire1") && blockDestroyTimer < timeSinceDestroyedBlock)
    {
      if (currentObjectName == null)
      {
        print("first object name initialized");
        currentObjectName = hit.transform.name;
      }
      if (currentObjectName != hit.transform.parent.name)
      {
        print(target.targetCube.name + " and " + hit.transform.parent.name);
        print("different names - reseting mining");
        miningTime = 0;
      }
      currentObjectName = hit.transform.parent.name;
      miningCalculation = currentToolType.miningSpeed / target.targetCube.GetCubeType().toughness;
      miningTime += Time.deltaTime;
      if (miningCalculation <= miningTime)
      {
        miningTime = 0;
        timeSinceDestroyedBlock = 0;
        target.targetCube.DestroyBlock();
      }
    }
    else
    {
      miningTime = 0;
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
