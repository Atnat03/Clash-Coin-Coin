using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Grid grid;

    [SerializeField] private ItemSO database;
    private int selectedObjectIndex = -1;
    
    [SerializeField] private GameObject gridVisualisation;
    [SerializeField] private GridData floorData, furnitureData;
    
    private List<GameObject> placedObjects = new List<GameObject>();
    
    [SerializeField] private PreviewSystem previewSystem;
    
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    
    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.itemsData.FindIndex(x => x.Id == ID);
        gridVisualisation.SetActive(true);
        previewSystem.StartShowingPlacementPreview(
            database.itemsData[selectedObjectIndex].Prefab, 
            database.itemsData[selectedObjectIndex].Size);
        playerInput.OnClicked += PlaceStructure;
        playerInput.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (playerInput.IsPointerOverUI()) return;
        
        Vector3 mousePosition = playerInput.GetWorldAimPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false) return;
        
        GameObject go = Instantiate(database.itemsData[selectedObjectIndex].Prefab);
        go.transform.position = gridPosition;
        
        placedObjects.Add(go);
        GridData selectedData = database.itemsData[selectedObjectIndex].Id == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(gridPosition, 
            database.itemsData[selectedObjectIndex].Size, 
            database.itemsData[selectedObjectIndex].Id, 
            placedObjects.Count - 1);
        
        previewSystem.UpdatePosition(gridPosition, false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int i)
    {
        GridData selectedData = database.itemsData[selectedObjectIndex].Id == 0 ? floorData : furnitureData;
        
        return selectedData.CanPlaceObejctAt(gridPosition, database.itemsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualisation.SetActive(false);
        previewSystem.StopShowingPreview();
        playerInput.OnClicked -= PlaceStructure;
        playerInput.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.back;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0) return;
        
        Vector3 mousePosition = playerInput.GetWorldAimPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            previewSystem.UpdatePosition(gridPosition, placementValidity);
            
            lastDetectedPosition = gridPosition;
        }
    }
    
}
