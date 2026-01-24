using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public int id;
    public string name;
    public Vector3 position;
    public Vector3 size;
    public bool playerOneProperty;
    public float PV;
    public float maxPV;
    public bool alreadyTakeTP = false;
    
    [HideInInspector] public GameObject prefab;
}


[System.Serializable]
public class GameSaveData
{
    public List<ItemData> itemsP1 = new List<ItemData>();
    public List<ItemData> itemsP2 = new List<ItemData>();

    
    public int player1Score = -1;
    public int player2Score = -1;
    public float combatDuration = 10f;
}
