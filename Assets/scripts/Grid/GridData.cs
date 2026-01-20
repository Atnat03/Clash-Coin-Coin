using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    private Dictionary<Vector3Int, PlacementData> placedObjects = new();

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
    
    public Vector3Int? GetItemPosition(Item item)
    {
        // Get the index of this item in the placed objects list
        int itemIndex = -1;
        
        if (GameManager.instance.placedItemsP1.Contains(item))
        {
            itemIndex = GameManager.instance.placedItemsP1.IndexOf(item);
        }
        else if (GameManager.instance.placedItemsP2.Contains(item))
        {
            itemIndex = GameManager.instance.placedItemsP2.IndexOf(item);
        }
        
        if (itemIndex == -1)
            return null;

        foreach (PlacementData data in placedObjects.Values)
        {
            if (data.PlacedObjectIndex == itemIndex)
            {
                if (data.occupiedPosition.Count > 0)
                    return data.occupiedPosition[0];
            }
        }

        return null;
    }
    
    public void ClearGrid()
    {
        placedObjects.Clear();
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