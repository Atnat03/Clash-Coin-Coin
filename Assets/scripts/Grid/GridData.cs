using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    private Dictionary<Vector3, PlacementData> placedObjects = new();
    private Dictionary<int, Vector3> itemPositions = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (Vector3Int position in positionToOccupy)
        {
            if (placedObjects.ContainsKey(position))
            {
                throw new Exception("Already occupied");
            }
            placedObjects[position] = data;
        }
    }
    
    public void RegisterItemPosition(Item item, Vector3 position)
    {
        itemPositions[item.GetInstanceID()] = position;
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObejctAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (Vector3Int pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }
    
    public Vector3? GetItemPosition(Item item)
    {
        if (item == null) return null;
        
        int instanceID = item.GetInstanceID();
        if (itemPositions.ContainsKey(instanceID))
        {
            return itemPositions[instanceID];
        }
        
        return null;
    }
    
    public void ClearGrid()
    {
        placedObjects.Clear();
        itemPositions.Clear();
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPosition = new();
    public int ID;
    public int PlacedObjectIndex;

    public PlacementData(List<Vector3Int> occupiedPosition, int ID, int PlacedObjectIndex)
    {
        this.occupiedPosition = occupiedPosition;
        this.ID = ID;
        this.PlacedObjectIndex = PlacedObjectIndex;
    }
}