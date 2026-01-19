using UnityEngine;


public class testBuild : Build, ITargetable
{
    public testBuild(int id, string name, float maxPV) : base(id, name, maxPV)
    {
    }

    public bool playerOneProperty { get; set; }

    public void TakeDamage(float damage)
    {
        Debug.Log("Take Damage");
    }
}