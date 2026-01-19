using UnityEngine;

public class Build : Item
{
    public Build(int  id, string name, float maxPV)
    {
        id = id;
        this.name = name;
        this.maxPV = maxPV;
        this.PV = maxPV;
    }
}
