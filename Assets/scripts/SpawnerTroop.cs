using System;
using UnityEngine;

public class SpawnerTroop : Build, ITargetable
{
    public float spawnRate = 1;
    public GameObject troopPrefab;
    public Transform spawnerPos;
    private float t = 0;
    
    public SpawnerTroop(int id, string name, float maxPV) : base(id, name, maxPV)
    { }

    private void Start()
    {
        Transform target = playerOneProperty ? VariablesManager.instance.nexus[0].transform : VariablesManager.instance.nexus[1].transform; 
        
        transform.LookAt(target);
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
        t.playerOneProperty = playerOneProperty;
        t.ForceRecalculatePath();
    }
}
