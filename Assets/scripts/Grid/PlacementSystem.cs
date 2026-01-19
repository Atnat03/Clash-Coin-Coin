using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private PlayerInputing playerInputing = null;
    [SerializeField] private Grid grid;

    [SerializeField] private ItemSO database;
    private int selectedObjectIndex = -1;
    
    [SerializeField] private GameObject gridVisualisation;
    [SerializeField] private GridData floorData, furnitureData;
    
    private List<GameObject> placedObjects = new List<GameObject>();
    
    [SerializeField] private PreviewSystem previewSystem;
    
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    
    public void Starting(PlayerInputing playerInpute)
    {
        if (playerInputing != null)
            return;
        
        playerInputing = playerInpute;
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
        
        playerInputing.OnClicked += PlaceStructure;
        playerInputing.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (playerInputing.IsPointerOverUI()) return;
        
        Vector3 mousePosition = playerInputing.GetWorldAimPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false) return;
        
        GameObject go = Instantiate(database.itemsData[selectedObjectIndex].Prefab);
        Vector3 worldPosition = grid.GetCellCenterWorld(gridPosition);
        go.transform.position = new Vector3(worldPosition.x, 0.1f, worldPosition.z);
        
        placedObjects.Add(go);
        GridData selectedData = database.itemsData[selectedObjectIndex].Id == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(gridPosition, 
            database.itemsData[selectedObjectIndex].Size,
            database.itemsData[selectedObjectIndex].Id,
            placedObjects.Count - 1);

        Item itemPlaced = go.GetComponentInChildren<Item>();
        ItemsData data = database.itemsData[selectedObjectIndex];
        itemPlaced.enabled = true;
        itemPlaced.GetComponent<ITargetable>().playerOneProperty = playerInputing.isPlayerOne;
        itemPlaced.maxPV = data.maxPV;
        itemPlaced.PV = data.maxPV;
        
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
        playerInputing.OnClicked -= PlaceStructure;
        playerInputing.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0) return;
        
        Vector3 mousePosition = playerInputing.GetWorldAimPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        Vector3 worldPosition = grid.GetCellCenterWorld(gridPosition);

        print(playerInputing.transform.name);
        
        if (lastDetectedPosition != gridPosition)
        {
            print("Is in");
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            previewSystem.UpdatePosition(new Vector3(worldPosition.x, 0.05f, worldPosition.z), placementValidity);
            
            lastDetectedPosition = gridPosition;
        }
    }
    
}
