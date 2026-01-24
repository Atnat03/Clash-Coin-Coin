using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlacementSystem : MonoBehaviour
{
    public bool isDuckPlayer = false;
    
    [SerializeField] private PlayerInputing playerInputing = null;
    [SerializeField] public Grid grid;

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
        
        floorData = new();
        furnitureData = new();
    }

    private void Start()
    {
        StopPlacement();
        
        playerInputing.OnSelectTroop += StartPlacement;
    }

    public void PlaceItem()
    {
        if(currentItemToPlace == -1)
            return;

        StartPlacement(currentItemToPlace);
    }
    
    public void StartPlacement(int ID)
    {
        
        StopPlacement();
        selectedObjectIndex = database.itemsData.FindIndex(x => x.Id == ID);

        print(database.itemsData[selectedObjectIndex].Name);
        
        gridVisualisation.SetActive(true);

        previewSystem.StartShowingPlacementPreview(
            database.itemsData[selectedObjectIndex].Prefab,
            database.itemsData[selectedObjectIndex].Size);

        Vector3 gridCenter = gridBounds.center;
        Vector3Int gridPos = grid.WorldToCell(gridCenter);
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
    Vector3 mousePosition = playerInputing.GetWorldAimPosition();
    Vector3Int gridPosition = grid.WorldToCell(mousePosition);
    
    bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
    if (placementValidity == false) return;
    
    GameObject go = Instantiate(database.itemsData[selectedObjectIndex].Prefab);
    Vector3 worldPosition = grid.GetCellCenterWorld(gridPosition);
    go.transform.position = new Vector3(worldPosition.x, -0.5f, worldPosition.z);
    
    placedObjects.Add(go);
    GridData selectedData = database.itemsData[selectedObjectIndex].Id == 0 ? floorData : furnitureData;
    selectedData.AddObjectAt(gridPosition, 
        database.itemsData[selectedObjectIndex].Size,
        database.itemsData[selectedObjectIndex].Id,
        placedObjects.Count - 1);

    ItemsData data = database.itemsData[selectedObjectIndex];

    if (data.Id >= 12 && data.Id <= 14)
    {
        List<Transform> childrenToDetach = new List<Transform>();
        
        for (int i = 0; i < 3 && i < go.transform.childCount; i++)
        {
            childrenToDetach.Add(go.transform.GetChild(i));
        }
        
        foreach (Transform child in childrenToDetach)
        {
            Item itemPlaced = child.GetComponent<Item>();
            
            if (itemPlaced == null)
            {
                Debug.LogWarning($"Enfant {child.name} de {go.name} n'a pas de composant Item");
                continue;
            }
            
            child.SetParent(null);

            itemPlaced.id = 3;
            itemPlaced.enabled = false;
            itemPlaced.playerOneProperty = playerInputing.isPlayerOne;

            Vector3Int childGridPos = grid.WorldToCell(child.position);
            selectedData.RegisterItemPosition(itemPlaced, childGridPos);
            
            if (itemPlaced.playerOneProperty)
            {
                GameManager.instance.placedItemsP1.Add(itemPlaced);
            }
            else
            {
                GameManager.instance.placedItemsP2.Add(itemPlaced);
            }
            
            GameManager.instance.AddItemInList(itemPlaced, gridPosition);
        }
        
        // Détruire le parent vide maintenant que les enfants sont détachés
        Destroy(go);
    }
    else
    {
        Item itemPlaced = go.GetComponentInChildren<Item>();
        
        if (itemPlaced == null)
        {
            Debug.LogError($"Aucun composant Item trouvé sur {go.name}");
            return;
        }

        itemPlaced.id = data.Id;
        itemPlaced.enabled = false;
        itemPlaced.playerOneProperty = playerInputing.isPlayerOne;

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
    }
    
    currentItemToPlace = -1;
    
    previewSystem.UpdatePosition(gridPosition, false);

    Validate();
}

    public void PlaceStructureAt(ItemData itemData)
    {
        if (itemData == null || itemData.prefab == null)
        {
            Debug.LogError("ItemData ou prefab invalide");
            return;
        }

        GameObject go = Instantiate(itemData.prefab);
        go.transform.position = itemData.position;

        placedObjects.Add(go);

        GridData selectedData = itemData.id == 0 ? floorData : furnitureData;

        Vector3Int gridPos = grid.WorldToCell(itemData.position);
        selectedData.AddObjectAt(gridPos, itemData.scale, itemData.id, placedObjects.Count - 1);

        Item itemPlaced = go.GetComponentInChildren<Item>();
        if (itemPlaced == null)
        {
            Debug.LogError($"Item component non trouvé sur {go.name}");
            return;
        }

        itemPlaced.playerOneProperty = itemData.playerOneProperty;

        itemPlaced.id = itemData.id;
        itemPlaced.name = itemData.name;
        itemPlaced.maxPV = itemData.maxPV;
        itemPlaced.PV = itemData.PV;
        itemPlaced.currentHP.fillAmount = itemPlaced.PV / itemData.maxPV;

        if (itemPlaced is Troop troop)
            troop.alreadyTakeTP = itemData.alreadyTakeTP;

        itemPlaced.enabled = false;

        selectedData.RegisterItemPosition(itemPlaced, itemData.position);

        if (itemPlaced.playerOneProperty)
            GameManager.instance.placedItemsP1.Add(itemPlaced);
        else
            GameManager.instance.placedItemsP2.Add(itemPlaced);

        Debug.Log($"Item restauré: {itemData.name} à la position {itemData.position} avec {itemData.PV}/{itemData.maxPV} PV");
    }
    
    private bool CheckPlacementValidity(Vector3Int gridPosition, int i)
{
    if (!IsInsideGrid(gridPosition))
        return false;

    GridData selectedData = database.itemsData[i].Id == 0
        ? floorData
        : furnitureData;

    return selectedData.CanPlaceObejctAt(
        gridPosition,
        database.itemsData[i].Size);
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

        List<Item> itemsToRegister = isDuckPlayer ? GameManager.instance.placedItemsP1 : GameManager.instance.placedItemsP2;

        foreach (Item item in itemsToRegister)
        {
            if(item is Troop troop)
                troop.GetComponent<AnimFlip>().TriggerAnim();
        }

        uiReady.SetActive(true);
    }

    public void StartCombat()
    {
        if(uiReady != null)
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
    
    private bool IsInsideGrid(Vector3 cell)
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
