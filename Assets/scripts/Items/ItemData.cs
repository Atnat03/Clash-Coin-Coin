using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public int id;
    public string name;
    public Vector3Int position;
    public Vector2Int scale;
    public bool playerOneProperty;
    public float PV;
    public float maxPV;

    [HideInInspector] public GameObject prefab;
}


[System.Serializable]
public class GameSaveData
{
    public List<ItemData> itemsP1 = new List<ItemData>();
    public List<ItemData> itemsP2 = new List<ItemData>();
    
    public List<TroopData> troopsP1;
    public List<TroopData> troopsP2;
    
    public int player1Score = -1;
    public int player2Score = -1;
    public float combatDuration = 20f;
}
