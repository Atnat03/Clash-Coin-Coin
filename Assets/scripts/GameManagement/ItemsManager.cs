using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;
    
    public List<ItemData> IemsP1 = new List<ItemData>();
    public List<ItemData> IemsP2 = new List<ItemData>();

    void Awake()
    {
        if(instance == null)instance = this;
        else Destroy(gameObject);
    }
}
