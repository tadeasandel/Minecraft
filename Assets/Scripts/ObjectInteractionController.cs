using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractionController : MonoBehaviour
{

  [Header("Side string setup")]
  [SerializeField] string[] tags;


  [Header("Default tool setup")]
  [SerializeField] ToolType defaultToolType;

  [Header("Block interaction setup")]
  [SerializeField] float blockDestroyTimer = 0.3f;
  [SerializeField] float blockPlaceTimer = 0.3f;
  [SerializeField] float cubeInteractDistance;

  CubeType currentCubeType;
  ToolType currentToolType;
  string currentObjectName;

  float timeSinceDestroyedBlock = Mathf.Infinity;
  float timeSincePlacedBlock = Mathf.Infinity;

  bool isInConstrutorMode = true;

  float cubeToughness;

  float miningCalculation;
  float miningTime;

  Target currentTarget;
  public Target target = new Target();
  CubePreview cubePreview;
  ItemTargetSwitcher itemTargetSwitcher;

  // information about the Cube that is targeted and it's side hit by player
  public struct Target
  {
    public CubeEditor targetCube;
    public string sideTag;
  }

  private void Start()
  {
    itemTargetSwitcher = FindObjectOfType<ItemTargetSwitcher>();
    cubePreview = FindObjectOfType<CubePreview>();
  }

  public CubeType GetCubeType()
  {
    return currentCubeType;
  }

  void Update()
  {
    ProcessRaycast();
    UpdateTimers();
  }

  // Gets called when player can place a cube
  // moves the CubePreview prefab to raycast position
  private void MoveCubePreview(Vector3 cubePreviewPos)
  {
    cubePreview.transform.position = cubePreviewPos;
  }

  // Loads player position from file
  public void LoadState(Vector3 playerPos)
  {
    SetPosition(playerPos);
  }

  private void UpdateTimers()
  {
    timeSincePlacedBlock += Time.deltaTime;
    timeSinceDestroyedBlock += Time.deltaTime;
  }

  // Gets called when item selection is changed
  public void EnableMode(ToolType toolType, CubeType cubeType)
  {
    ActivateCubeType(cubeType);
    ActivateToolType(toolType);
    cubePreview.ApplyCubeType();
  }

  // Activates ToolType of current selected item
  private void ActivateToolType(ToolType toolType)
  {
    if (toolType == null)
    {
      currentToolType = defaultToolType;
      return;
    }
    currentToolType = toolType;
  }

  // Activates CubeType of current selected item
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

  // Gets if player has an active ToolType time excluding default one
  public bool IsUsingToolType()
  {
    if ((currentToolType == defaultToolType && currentCubeType == null) || (currentToolType != defaultToolType && currentCubeType == null))
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  // sets Player position
  public void SetPosition(Vector3 playerNewPos)
  {
    GetComponent<PlayerMovement>().SetPositionTo(playerNewPos);
  }

  // does process raycasting on cubes
  private void ProcessRaycast()
  {
    RaycastHit hit;
    if (currentTarget.targetCube != target.targetCube) { cubePreview.ApplyCubeType(); }
    currentTarget.targetCube = target.targetCube;
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

  // Further process of what cube target was hit
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

  // Process of active left mouse button for mining
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

  // Process of active right mouse button for Building a block as well as visualizing it before it's placed
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

  // Visualizes block before it's placed
  private void VisualizeGrid(RaycastHit hit)
  {
    target.targetCube = hit.transform.GetComponentInParent<CubeEditor>();
    if (target.targetCube != null && isInConstrutorMode)
    {
      MoveCubePreview(target.targetCube.GetVizualizationCenter(target.sideTag));
    }
  }

  private void OnDrawGizmos()
  {
    if (target.targetCube != null && isInConstrutorMode)
    {
      target.targetCube.VisualizeBlock(target.sideTag);
    }
  }
}
