using UnityEngine;


public class testBuild : Build, ITargetable
{
    public testBuild(int id, string name, float maxPV) : base(id, name, maxPV)
    {
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Take Damage");
    }
}