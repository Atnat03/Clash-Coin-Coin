using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour, ITargetable
{
    [HideInInspector]public int id;
    [HideInInspector]public string name;
    public float PV;
    [HideInInspector]public float maxPV;
    public Image currentHP;
    
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
    
    public void Die()
    {
        GameManager.instance.RemovePlacedItem(this);
        Destroy(gameObject);
    }

    public bool playerOneProperty { get; set; }
    
    public float poisonDamageTakenBySeconds;
    public float poisonDuration;
    
    

    public float timer;

    public void Update()
    {
        if (poisonDuration > 0)
        {
            currentHP.color = Color.purple;
            poisonDuration -= Time.deltaTime;
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                TakeDamage(poisonDamageTakenBySeconds);
                timer = 1;
            }
        }
        else
        {
            currentHP.color = Color.red;
        }
    }
    
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
        poisonDuration = duration;
        poisonDamageTakenBySeconds = damage;
        timer = 1f;
    }
}
