using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public enum TypeItem
{
    Grenouille,
    Canard
}

public abstract class Item : MonoBehaviour, ITargetable
{
    public int id;
    [HideInInspector]public string name;
    public float PV;
    public float maxPV;
    public Image currentHP;
    public float Damage = 10f;
    
    public TypeItem type;
    
    public bool IsMovementTarget => false;

    private void Start()
    {
        ItemSO data = type == TypeItem.Grenouille
            ? VariablesManager.instance.frogItemDatabase
            : VariablesManager.instance.duckItemDatabase;

        PV = data.itemsData[id].PV;
        Damage = data.itemsData[id].Dmg;
        
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
        StartCoroutine(TakeSmoothDamage(damage));
    }

    IEnumerator TakeSmoothDamage(float damage)
    {
        print("Take smooth damage");
        float finalPV = PV - damage;
        float statedPV = PV;
        float duration = 0.5f;
        float t = 0;
        
        while (t < duration)
        {
            PV = Mathf.Lerp(statedPV, finalPV, t / duration);
            currentHP.fillAmount = PV / maxPV;
            
            print(PV + " take dmg...");
        
            if (PV <= 0)
            { 
                Die();
            }
            
            t += Time.deltaTime;
            
            yield return null;
        }
    }

    public void GetPoisoned(float duration, float damage)
    {
        StartCoroutine(PoisenedCoroutine(duration, damage));
    }

    IEnumerator PoisenedCoroutine(float duration, float damage)
    {
        float elapsedtime = 0f;
        while (elapsedtime < duration)
        {
            elapsedtime += 1f;
            TakeDamage(damage);
            yield return new WaitForSeconds(1f);
        }
        currentHP.color = Color.green;
    }    

    public virtual void SetActive(bool state)
    {
        enabled = state;
    }
}
