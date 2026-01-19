using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [HideInInspector]public int id;
    public string name;
    public float PV;
    public float maxPV;

    
    public Item()
    {
        id = 0;
        name = "";
        maxPV = 0;
        PV = 0;
        Debug.LogWarning("Item created without parameters");
    }
    
    public Item(int  id, string name, float maxPV)
    {
        this.id = id;
        this.name = name;
        this.maxPV = maxPV;
        this.PV = maxPV;
    }
    
    public virtual void DoThing(){}

    public void TakeDamages(int damages)
    {
        PV -= damages;
        if (PV <= 0)
        { 
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
