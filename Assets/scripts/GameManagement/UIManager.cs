using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        HideStartText(false);
    }

    [Header("Combat")]
    public Text timerCombat;

    public GameObject StartTextP1;
    public GameObject StartTextP2;
    
    private void Start()
    {
        timerCombat.gameObject.SetActive(false);
    }

    public void UpdateCombatUI(int combat)
    {
        timerCombat.text = combat.ToString();
    }

    public void HideCombatUI(bool state)
    {
        timerCombat.gameObject.SetActive(state);
    }

    public void HideStartText(bool state)
    {
        StartTextP1.gameObject.SetActive(state);
        StartTextP2.gameObject.SetActive(state);
    }
}
