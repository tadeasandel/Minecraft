using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractionController : MonoBehaviour
{
  [SerializeField] float cubeInteractDistance;

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

  public void LoadState(Vector3 playerPos)
  {
    SetPosition(playerPos);
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

  public void SetPosition(Vector3 playerNewPos)
  {
    GetComponent<PlayerMovement>().MoveTo(playerNewPos);
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
    if (!Input.GetButton("Fire1") || !Input.GetButton("Fire2")) { return; }
    RaycastHit hit;
    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, cubeInteractDistance))
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
        currentObjectName = hit.transform.name;
      }
      if (currentObjectName != hit.transform.parent.name)
      {
        miningTime = 0;
      }
      currentObjectName = hit.transform.parent.name;
      miningCalculation = target.targetCube.GetCubeType().toughness / currentToolType.miningSpeed;
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
