using UnityEngine;

public class PlayerData : MonoBehaviour
{
  float[] position;

  public PlayerData(ObjectInteractionController objectInteractionController)
  {
    position[0] = objectInteractionController.transform.position.x;
    position[1] = objectInteractionController.transform.position.y;
    position[2] = objectInteractionController.transform.position.z;
  }
}