using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlacementSystem : MonoBehaviour
{
    public bool isDuckPlayer = false;
    
    [SerializeField] private PlayerInputing playerInputing = null;
    [SerializeField] private Grid grid;

    [HideInInspector] public ItemSO database;
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

    public int currentItemToPlace = -1;
    
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

        database = isDuckPlayer
            ? VariablesManager.instance.duckItemDatabase
            : VariablesManager.instance.frogItemDatabase;
    }

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
        
        playerInputing.OnSelectTroop += StartPlacement;
    }

    public void PlaceItem()
    {
        if(currentItemToPlace == -1)
            return;

        StartPlacement(currentItemToPlace);
    }
    
    
    IEnumerator WaitALittle()
    {
        yield return new WaitForSeconds(0.3f);
    }
    
    public void StartPlacement(int ID)
    {
        print(ID);
        
        StartCoroutine(WaitALittle());

        StopPlacement();
        selectedObjectIndex = database.itemsData.FindIndex(x => x.Id == ID);

        print(database.itemsData[selectedObjectIndex].Name);
        
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

        playerInputing.IsReady = false;
        
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
        itemPlaced.enabled = false;
        itemPlaced.playerOneProperty = playerInputing.isPlayerOne;
        itemPlaced.maxPV = data.maxPV;
        itemPlaced.PV = data.maxPV;

        selectedData.RegisterItemPosition(itemPlaced, gridPosition);
        
        if (itemPlaced.playerOneProperty)
        {
            GameManager.instance.placedItemsP1.Add(itemPlaced);
        }
        else
        {
            GameManager.instance.placedItemsP2.Add(itemPlaced);
        }
        
        GameManager.instance.AddItemInList(itemPlaced, gridPosition);
        
        currentItemToPlace = -1;
        
        previewSystem.UpdatePosition(gridPosition, false);

        Validate();
    }

    public void PlaceStructureAt(ItemData itemData)
    {
        if (playerInputing.IsPointerOverUI()) return;
        
        GameObject go = Instantiate(itemData.prefab);
        Vector3 worldPosition = grid.GetCellCenterWorld(itemData.position);
        go.transform.position = new Vector3(worldPosition.x, 0.1f, worldPosition.z);
        
        placedObjects.Add(go);
        GridData selectedData = itemData.id == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(itemData.position, 
            itemData.scale,
            itemData.id,
            placedObjects.Count - 1);

        Item itemPlaced = go.GetComponentInChildren<Item>();
        ItemData data = itemData;
        itemPlaced.enabled = false;
        
        
        itemPlaced.playerOneProperty = itemData.playerOneProperty;
        
        itemPlaced.maxPV = data.maxPV;
        itemPlaced.PV = data.PV;
        
        selectedData.RegisterItemPosition(itemPlaced, itemData.position);
        
        if (itemPlaced.playerOneProperty)
        {
            GameManager.instance.placedItemsP1.Add(itemPlaced);
        }
        else
        {
            GameManager.instance.placedItemsP2.Add(itemPlaced);
        }
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
        playerInputing.IsReady = true;
        uiReady.SetActive(true);
    }

    public void StartCombat()
    {
        uiReady.SetActive(false);
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

    public void ReloadData(List<ItemData> list)
    {
        Debug.Log($"Reloading {list.Count} items for {playerInputing.name}");
    
        foreach (ItemData item in list)
        {
            if (item.prefab != null)
            {
                PlaceStructureAt(item);
            }
            else
            {
                Debug.LogWarning($"ItemData avec ID {item.id} a un prefab null");
            }
        }
    
        list.Clear();
    }
    
    public void SaveGrid()
    {
        // Nettoyer les listes avant de sauvegarder
        GameManager.instance.itemPlacedDataP1.Clear();
        GameManager.instance.itemPlacedDataP2.Clear();
    
        foreach (Item item in GameManager.instance.placedItemsP1)
        {
            Vector3Int? pos = furnitureData.GetItemPosition(item);
            if (pos == null)
                pos = floorData.GetItemPosition(item);
    
            if (pos != null)
            {
                GameManager.instance.AddItemInList(item, pos.Value);
            }
        }

        foreach (Item item in GameManager.instance.placedItemsP2)
        {
            Vector3Int? pos = furnitureData.GetItemPosition(item);
            if (pos == null)
                pos = floorData.GetItemPosition(item);
    
            if (pos != null)
            {
                GameManager.instance.AddItemInList(item, pos.Value);
            }
        }
    }

}
