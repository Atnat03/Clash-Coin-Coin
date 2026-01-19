using UnityEngine;

public class ItemData
{
    [HideInInspector] public int id;
    public string name;
    public Vector2 position;
    public Vector2 scale;
    public bool playerOneProperty;
    public float PV;
    public float maxPV;
    public GameObject prefab;
}
