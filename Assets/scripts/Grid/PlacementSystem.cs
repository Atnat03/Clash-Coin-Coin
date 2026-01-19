using System;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Grid grid;

    [SerializeField] private ItemSO database;
    private int selectedObjectIndex = -1;
    
    [SerializeField] private GameObject gridVisualisation;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.itemsData.FindIndex(x => x.Id == ID);
        gridVisualisation.SetActive(true);
        cellIndicator.SetActive(true);
        playerInput.OnClicked += PlaceStructure;
        playerInput.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (playerInput.IsPointerOverUI()) return;
        
        Vector3 mousePosition = playerInput.GetWorldAimPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        GameObject go = Instantiate(database.itemsData[selectedObjectIndex].Prefab);
        go.transform.position = gridPosition;
        cellIndicator.transform.position = gridPosition;
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualisation.SetActive(false);
        cellIndicator.SetActive(false);
        playerInput.OnClicked -= PlaceStructure;
        playerInput.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0) return;
        
        Vector3 mousePosition = playerInput.GetWorldAimPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        cellIndicator.transform.position = gridPosition;
    }
    
}
