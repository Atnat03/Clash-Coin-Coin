using System;
using UnityEngine;

public class SpawnerTroop : Build, ITargetable
{
    public float spawnRate = 1;
    public GameObject troopPrefab;
    public Transform spawnerPos;
    private float t = 0;
    [HideInInspector]public int troopId;
    
    public SpawnerTroop(int id, string name, float maxPV) : base(id, name, maxPV)
    { }

    private void Start()
    {
        print(name + " : " + id);
        
        Transform target = playerOneProperty ? VariablesManager.instance.nexus[0].transform : VariablesManager.instance.nexus[1].transform; 
        
        transform.LookAt(target);
        
        troopId = troopPrefab.GetComponent<Troop>().id;
    }

    void Update()
    {
        if (t < spawnRate)
        {
            t += Time.deltaTime;
        }
        else
        {
            SpawningTroop();
            
            t = 0;
        }
    }

    private void SpawningTroop()
    {
        GameObject newTroop = Instantiate(troopPrefab, spawnerPos.position, spawnerPos.rotation);
        Troop t = newTroop.GetComponent<Troop>();

        t.id = troopId;
        t.playerOneProperty = playerOneProperty;
        t.ForceRecalculatePath();

        GameManager.instance.RegisterTroop(t);
    }

}

[System.Serializable]
public class TroopData
{
    public int id;
    public bool playerOneProperty;

    public Vector3 position;
    public Quaternion rotation;

    public float PV;
    public float maxPV;
}

