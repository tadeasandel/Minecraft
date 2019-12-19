using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTargetSwitcher : MonoBehaviour
{
  int currentItemIndex = 0;

  public CubeType GetCubeType()
  {
    return transform.GetChild(currentItemIndex).GetComponent<ItemManager>().GetCubeType();
  }

  private void Start()
  {
    SetItemActive();
  }

  // Checks for current item index vs the item last assigned
  private void Update()
  {
    int previousItem = currentItemIndex;
    ProcessInput();

    if (currentItemIndex != previousItem)
    {
      SetItemActive();
    }
  }

  // sets item by index active
  private void SetItemActive()
  {
    int itemIndex = 0;
    foreach (Transform item in transform)
    {
      if (itemIndex == currentItemIndex)
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
      if (currentItemIndex >= transform.childCount - 1)
      {
        currentItemIndex = 0;
      }
      else
      {
        currentItemIndex++;
      }
    }
    if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
    {
      if (currentItemIndex <= 0)
      {
        currentItemIndex = transform.childCount - 1;
      }
      else
      {
        currentItemIndex--;
      }
    }
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      currentItemIndex = 0;
    }
    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      currentItemIndex = 1;
    }
    if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      currentItemIndex = 2;
    }
    if (Input.GetKeyDown(KeyCode.Alpha4))
    {
      currentItemIndex = 3;
    }
    if (Input.GetKeyDown(KeyCode.Alpha5))
    {
      currentItemIndex = 4;
    }
    if (Input.GetKeyDown(KeyCode.Alpha6))
    {
      currentItemIndex = 5;
    }
  }
}
