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
    public int PV;
    public int Dmg;
    [HideInInspector]public Vector3 Size;
    public GameObject Prefab;
    
    public Sprite carte;
    public Sprite dosCarte;
    public Sprite iconType;
    public int rarity;
}