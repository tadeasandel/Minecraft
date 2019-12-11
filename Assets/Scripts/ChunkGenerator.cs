using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
  [SerializeField] float chunkHeight;
  [SerializeField] float chunkWidth;
  [SerializeField] float chunkDepth;

  private void OnDrawGizmos()
  {
    Gizmos.DrawWireCube(transform.position, new Vector3(chunkHeight, chunkDepth, chunkWidth));
  }

  private void Update()
  {
    UpdateName();
  }

  private void UpdateName()
  {
    gameObject.name = transform.position.x + "," + transform.position.z;
  }
}

