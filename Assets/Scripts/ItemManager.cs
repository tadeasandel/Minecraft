using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
  [SerializeField] Image[] images;

  [SerializeField] Color activeColor, InactiveColor;

  bool hasItemToPlace = true;

  [SerializeField] CubeType currentCubeType;
  [SerializeField] ToolType currentToolType;

  private void Update()
  {
    if (currentCubeType == null && hasItemToPlace)
    {
      hasItemToPlace = false;
    }
  }

  public void SetItemActive()
  {
    RecolorImages(activeColor);
    FindObjectOfType<ObjectInteractionController>().EnableMode(currentToolType, currentCubeType);
  }

  public void SetItemInactive()
  {
    RecolorImages(InactiveColor);
  }

  private void RecolorImages(Color imageColor)
  {
    foreach (Image image in images)
    {
      image.color = imageColor;
    }
  }

  public void SetCubeType(CubeType cubeType)
  {
    currentCubeType = cubeType;
  }

  public CubeType GetCubeType()
  {
    return currentCubeType;
  }
}