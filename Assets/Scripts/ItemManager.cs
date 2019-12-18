using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
  [Header("Image setup")]
  [SerializeField] Image[] images;
  [SerializeField] Color activeColor, InactiveColor;

  bool hasItemToPlace = true;

  [Header("Initial Item setup")]
  [SerializeField] CubeType currentCubeType;
  [SerializeField] ToolType currentToolType;

  private void Update()
  {
    if (currentCubeType == null && hasItemToPlace)
    {
      hasItemToPlace = false;
    }
  }

  // sets this item to active
  public void SetItemActive()
  {
    RecolorImages(activeColor);
    FindObjectOfType<ObjectInteractionController>().EnableMode(currentToolType, currentCubeType);
  }

  // sets this item to inactive
  public void SetItemInactive()
  {
    RecolorImages(InactiveColor);
  }

  // recolors this item to selected / unselected
  private void RecolorImages(Color imageColor)
  {
    foreach (Image image in images)
    {
      image.color = imageColor;
    }
  }

  // potencial future Setter for Item, if it gets changed
  public void SetCubeType(CubeType cubeType)
  {
    currentCubeType = cubeType;
  }

  // potencial future getter for Item
  public CubeType GetCubeType()
  {
    return currentCubeType;
  }
}