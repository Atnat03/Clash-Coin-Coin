using UnityEngine;
using UnityEngine.UI;

public class Nexus : MonoBehaviour, ITargetable
{
    public bool isPlayer1 = false;
    public float PV = 200;
    float maxPV;
    public Image currentHP;

    void OnEnable()
    {
        GameManager.instance.OnComeBack += SetUp;
    }

    void OnDisable()
    {
        GameManager.instance.OnComeBack -= SetUp;
    }

    public void SetUp()
    {
        if(playerOneProperty)
        {
            PV = GameManager.instance.PVNexus_P1;
            maxPV = GameManager.instance.maxPVNexus_P1;
        }
        else
        {
            PV = GameManager.instance.PVNexus_P2;
            maxPV = GameManager.instance.maxPVNexus_P2;
        }        
        currentHP.fillAmount = PV / maxPV;
    }

    private void Start()
    {
        if(playerOneProperty)
        {
            PV = GameManager.instance.PVNexus_P1;
            maxPV = GameManager.instance.maxPVNexus_P1;
        }
        else
        {
            PV = GameManager.instance.PVNexus_P2;
            maxPV = GameManager.instance.maxPVNexus_P2;
        }        
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

    //public Animator animEnd;
    //public GameObject screen;

    private void Die()
    {
        //Time.timeScale = 0;
        //screen.SetActive(true);
        //animEnd.SetTrigger("End");
    }

    public bool CanBeAttacked => true;
    public void GetPoisoned(float duration, float damage)
    { }

    public bool IsMovementTarget => false;
}