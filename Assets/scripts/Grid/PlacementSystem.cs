using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Grid grid;

    private void Update()
    {
        Vector3 mousePosition = playerInput.GetWorldAimPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        cellIndicator.transform.position = gridPosition;
    }
    
}
