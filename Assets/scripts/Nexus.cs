using System;
using UnityEngine;

public class Nexus : Build, ITargetable
{
    public bool isPlayer1 = false;
    
    public Nexus(int id, string name, float maxPV) : base(id, name, maxPV)
    {
    }

    private void Start()
    {
        playerOneProperty = isPlayer1;
    }
}
