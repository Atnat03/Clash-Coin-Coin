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
    }

    [Header("Combat")]
    public Text timerCombat;

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
        if(timerCombat != null)
            timerCombat.gameObject.SetActive(state);
    }
}
