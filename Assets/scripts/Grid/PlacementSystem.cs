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
    
    [SerializeField] private Vector2Int gridSize = new(20, 20);
    [SerializeField] private Vector3Int gridOrigin = Vector3Int.zero;
    private Bounds gridBounds;
    
    [SerializeField] private GameObject uiReady;
    
    private void Awake()
    {
        if (grid == null) return;

        Vector3 min = grid.GetCellCenterWorld(gridOrigin);

        Vector3Int maxCell = new Vector3Int(
            gridOrigin.x + gridSize.x - 1,
            0,
            gridOrigin.z + gridSize.y - 1
        );

        Vector3 max = grid.GetCellCenterWorld(maxCell);

        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min + grid.cellSize;

        gridBounds = new Bounds(center, size);
    }
    
    public void Starting(PlayerInputing playerInpute)
    {
        if (playerInputing != null)
            return;
        
        playerInputing = playerInpute;
        StopPlacement();
        floorData = new();
        furnitureData = new();

        playerInpute.OnSelectBuild += StartPlacement;
        playerInpute.OnSelectTroop += StartPlacement;
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.itemsData.FindIndex(x => x.Id == ID);

        gridVisualisation.SetActive(true);

        previewSystem.StartShowingPlacementPreview(
            database.itemsData[selectedObjectIndex].Prefab,
            database.itemsData[selectedObjectIndex].Size);

        Vector3 startPos = playerInputing.transform.position;
        Vector3Int gridPos = grid.WorldToCell(startPos);
        Vector3 worldPos = grid.GetCellCenterWorld(gridPos);

        previewSystem.UpdatePosition(
            new Vector3(worldPos.x, 0.05f, worldPos.z),
            true);

        lastDetectedPosition = gridPos;
        
        playerInputing.SetAimBounds(gridBounds);

        playerInputing.hasValidate = false;
        
        playerInputing.OnClicked += PlaceStructure;
        playerInputing.OnExit += Validate;
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
        if (!IsInsideGrid(gridPosition))
            return false;

        GridData selectedData = database.itemsData[selectedObjectIndex].Id == 0
            ? floorData
            : furnitureData;

        return selectedData.CanPlaceObejctAt(
            gridPosition,
            database.itemsData[selectedObjectIndex].Size);
    }
    
    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualisation.SetActive(false);
        previewSystem.StopShowingPreview();
        playerInputing.OnClicked -= PlaceStructure;
        playerInputing.OnExit -= StopPlacement;
        playerInputing.SetAimBounds(default);
        lastDetectedPosition = Vector3Int.zero;
    }

    void Validate()
    {
        StopPlacement();
        playerInputing.hasValidate = true;
        uiReady.SetActive(true);
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
    
    private bool IsInsideGrid(Vector3Int cell)
    {
        return cell.x >= gridOrigin.x &&
               cell.z >= gridOrigin.z &&
               cell.x < gridOrigin.x + gridSize.x &&
               cell.z < gridOrigin.z + gridSize.y;
    }
    
    
    private void OnDrawGizmos()
    {
        if (grid == null) return;

        Vector3 min = grid.GetCellCenterWorld(gridOrigin);

        Vector3Int maxCell = new Vector3Int(
            gridOrigin.x + gridSize.x - 1,
            0,
            gridOrigin.z + gridSize.y - 1
        );

        Vector3 max = grid.GetCellCenterWorld(maxCell);

        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min + grid.cellSize;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
}
