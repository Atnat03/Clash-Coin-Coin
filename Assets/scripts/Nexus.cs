using System.Collections;
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
        if(GameManager.instance != null)
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
        playerOneProperty = isPlayer1;

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
        name = "Nexus";
    }

    public bool playerOneProperty { get; set; }
    public void TakeDamage(float damage)
    {
        StartCoroutine(TakeSmoothDamage(damage));
    }
    
    IEnumerator TakeSmoothDamage(float damage)
    {
        float finalPV = PV -= damage;
        float duration = 0.5f;
        float t = 0;
        
        while (t < duration)
        {
            PV = Mathf.Lerp(PV, finalPV, t / duration);
            currentHP.fillAmount = PV / maxPV;
        
            if (PV <= 0)
            { 
                Die();
            }
            
            if(playerOneProperty)
                GameManager.instance.UpdateNexusP1(PV);
            else
                GameManager.instance.UpdateNexusP2(PV);
            
            t += Time.deltaTime;
            
            yield return null;
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