using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    [HideInInspector] public int id;
    public string name;
    public Vector3Int position;
    public Vector2Int scale;
    public bool playerOneProperty;
    public float PV;
    public float maxPV;
    public GameObject prefab;
}
