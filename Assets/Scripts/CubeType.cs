using UnityEngine;

[CreateAssetMenu(fileName = "CubeType", menuName = "Cubes/Create New Cube Type", order = 0)]
public class CubeType : ScriptableObject
{
  public float toughness;
  public string cubeTypeName;

  public Material[] materials;

  public GameObject dropItem;
}