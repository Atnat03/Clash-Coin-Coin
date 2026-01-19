using UnityEngine;

public class Troop : Item
{
    public float Speed;


    public Troop(int  id, string name, float maxPV, float speed)
    {
        id = id;
        this.name = name;
        this.maxPV = maxPV;
        this.PV = maxPV;
        speed = speed;
    }
}
