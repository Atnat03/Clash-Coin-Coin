using UnityEngine;
using UnityEngine.UI;

public class Nexus : MonoBehaviour, ITargetable
{
    public bool isPlayer1 = false;
    public float PV = 200;
    float maxPV;
    public Image currentHP;

    private void Start()
    {
        maxPV = PV;
        playerOneProperty = isPlayer1;
        name = "Nexus"; 
    }

    public bool playerOneProperty { get; set; }
    public void TakeDamage(float damage)
    {
        PV -= damage;
        print("take damage" + damage);
        
        currentHP.fillAmount = PV / maxPV;
        
        if(playerOneProperty)
            GameManager.instance.UpdateNexusP1(PV);
        else
            GameManager.instance.UpdateNexusP2(PV);
        
        if (PV <= 0)
        { 
            Die();
        }    
    }

    private void Die()
    {
        print("Nexus pété");
    }

    public bool CanBeAttacked => true;
    public void GetPoisoned(float duration, float damage)
    { }

    public bool IsMovementTarget => false;
}