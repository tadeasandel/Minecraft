using System;
using System.Collections.Generic;
using UnityEngine;

public class CubePreview : MonoBehaviour
{
  [Header("Transparency settings")]
  [Range(0, 1)] [SerializeField] float visualizedAlphaValue;
  const float transparentAlphaValue = 0;

  public CubeType currentCubeType;

  // reference to player
  ObjectInteractionController objectInteractionController;

  // List of all childs Meshes
  List<MeshRenderer> childMeshes = new List<MeshRenderer>();

  // Applies current Cube Type, also calls refresh
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

  private void Start()
  {
    objectInteractionController = FindObjectOfType<ObjectInteractionController>();
    foreach (Transform child in transform)
    {
      childMeshes.Add(child.GetComponent<MeshRenderer>());
    }
    ApplyCubeType();
  }

  // Acts as a refresh method for player switching cube targets or ToolType
  private void TargetChanged()
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

  // Enables / Disables MeshRenderes, also sets transparency level
  private void ProcessChildren(bool isRevealed, float alphaValue)
  {
    foreach (MeshRenderer child in childMeshes)
    {
      child.gameObject.SetActive(isRevealed);
      child.material.color = new Color(child.material.color.r, child.material.color.g, child.material.color.b, alphaValue);
    }
  }
}
