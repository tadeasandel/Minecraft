using UnityEngine;

[CreateAssetMenu(fileName = "ToolType", menuName = "Tool types/ Create new tool type", order = 0)]
public class ToolType : ScriptableObject
{
  public float miningSpeed;
  public bool destroyAble;
  public float durability;
}