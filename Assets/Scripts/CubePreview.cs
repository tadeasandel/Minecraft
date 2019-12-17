using System;
using System.Collections.Generic;
using UnityEngine;

public class CubePreview : MonoBehaviour
{
  [Range(0, 1)] [SerializeField] float visualizedAlphaValue;
  const float transparentAlphaValue = 0;

  [SerializeField] GameObject blockPrefabToVisualize;

  public CubeType currentCubeType;

  ObjectInteractionController objectInteractionController;

  List<MeshRenderer> childMeshes = new List<MeshRenderer>();

  private void Start()
  {
    objectInteractionController = FindObjectOfType<ObjectInteractionController>();
    foreach (Transform child in transform)
    {
      childMeshes.Add(child.GetComponent<MeshRenderer>());
    }
    ApplyCubeType();
  }

  public void TargetChanged()
  {
    if (!objectInteractionController.IsUsingToolType() && objectInteractionController.target.targetCube != null)
    {
      ProcessChildren(true, visualizedAlphaValue);
    }
    else
    {
      ProcessChildren(false, transparentAlphaValue);
    }
  }

  private void ProcessChildren(bool isRevealed, float alphaValue)
  {
    foreach (MeshRenderer child in childMeshes)
    {
      child.gameObject.SetActive(isRevealed);
      child.material.color = new Color(child.material.color.r, child.material.color.g, child.material.color.b, alphaValue);
    }
  }

  public void ApplyCubeType()
  {
    if (objectInteractionController == null) { objectInteractionController = FindObjectOfType<ObjectInteractionController>(); }
    currentCubeType = objectInteractionController.GetCubeType();

    if (currentCubeType == null) { TargetChanged(); return; }
    if (currentCubeType.materials.Length != 6) { return; }
    for (int i = 0; i < 6; i++)
    {
      childMeshes[i].material = currentCubeType.materials[i];
    }
    TargetChanged();
  }

  public CubeType GetCubeType()
  {
    return currentCubeType;
  }

  public void SetCubeType(CubeType cubeType)
  {
    currentCubeType = cubeType;
  }
}