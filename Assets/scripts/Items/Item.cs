using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    [HideInInspector]public int id;
    [HideInInspector]public string name;
    public float PV;
    [HideInInspector]public float maxPV;
    public Image currentHP;
    
    public bool IsMovementTarget => false;
    
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
    
    public bool CanBeAttacked => true;
    
    public void Die()
    {
        GameManager.instance.RemovePlacedItem(this);
        GameManager.instance.RemovePlacedDataItem(this);
        Destroy(gameObject);
    }

    public bool playerOneProperty { get; set; }
    public void TakeDamage(float damage)
    {
        PV -= damage;
        print("take damage" + damage);
        
        currentHP.fillAmount = PV / maxPV;
        
        if (PV <= 0)
        { 
            Die();
        }    
    }

    public void GetPoisoned(float duration, float damage)
    {
        
    }

    public virtual void SetActive(bool state)
    {
        enabled = state;
    }
}
