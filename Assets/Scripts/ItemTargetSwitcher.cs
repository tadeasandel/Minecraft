using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTargetSwitcher : MonoBehaviour
{
  int currentItem = 0;

  private void Start()
  {
    SetItemActive();
  }

  private void Update()
  {
    int previousItem = currentItem;
    ProcessInput();

    if (currentItem != previousItem)
    {
      SetItemActive();
    }
  }

  private void SetItemActive()
  {
    int itemIndex = 0;
    foreach (Transform item in transform)
    {
      if (itemIndex == currentItem)
      {
        item.GetComponent<ItemManager>().SetItemActive();
      }
      else
      {
        item.GetComponent<ItemManager>().SetItemInactive();
      }
      itemIndex++;
    }
  }

  private void ProcessInput()
  {
    if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
    {
      if (currentItem >= transform.childCount - 1)
      {
        currentItem = 0;
      }
      else
      {
        currentItem++;
      }
    }
    if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
    {
      if (currentItem <= 0)
      {
        currentItem = transform.childCount - 1;
      }
      else
      {
        currentItem--;
      }
    }
  }

  public CubeType GetCubeType()
  {
    return transform.GetChild(currentItem).GetComponent<ItemManager>().GetCubeType();
  }
}
