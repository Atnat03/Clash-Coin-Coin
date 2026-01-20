using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public List<ItemsData> itemsData;
}

[System.Serializable]
public class ItemsData
{
    public int Id;
    public string Name;
    public Vector2Int Size = Vector2Int.one;
    public GameObject Prefab;
    [HideInInspector] public float PV;
    public float maxPV;
    
    public Sprite carte;
    public Sprite dosCarte;
    public int rarity;
}