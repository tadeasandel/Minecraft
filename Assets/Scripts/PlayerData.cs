using UnityEngine;

public class PlayerData : MonoBehaviour
{

  ObjectInteractionController objectInteractionController;
  Positions positions = new Positions();

  private void Start()
  {
    objectInteractionController = GetComponent<ObjectInteractionController>();
  }

  public void SaveData()
  {
    positions.xPosition = objectInteractionController.transform.position.x;
    positions.yPosition = objectInteractionController.transform.position.y;
    positions.ZPosition = objectInteractionController.transform.position.z;
  }

  [System.Serializable]
  public class Positions
  {
    public float xPosition;
    public float yPosition;
    public float ZPosition;
  }
}