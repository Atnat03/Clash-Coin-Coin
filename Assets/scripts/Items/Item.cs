using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    [HideInInspector]public int id;
    [HideInInspector]public string name;
    public float PV;
    public float maxPV;
    public Image currentHP;
    
    public bool IsMovementTarget => false;

    private void Start()
    {
        maxPV = PV;
    }

    public Item()
    {
        id = 0;
        name = "";
        Debug.LogWarning("Item created without parameters");
    }
    
    public Item(int  id, string name, float maxPV)
    {
        this.id = id;
        this.name = name;
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
        print(transform.name + "  take damage :" + damage);
        
        print(maxPV);
        
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
