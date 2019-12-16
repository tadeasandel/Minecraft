using UnityEngine;

public class CubeData : MonoBehaviour
{
  public CubeEditor cubeEditor;
  public CubeType cubeType;

  public CubeData(CubeEditor cubeEditor)
  {
    this.cubeEditor = cubeEditor;
    cubeType = cubeEditor.GetCubeType();
  }
}