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
    public GameObject timerCombat;
    public Image logo;
    public Sprite[] spritesTimer;

    private void Start()
    {
        timerCombat.gameObject.SetActive(false);
    }

    public void UpdateCombatUI(int combat)
    {
        if(logo != null)
            logo.sprite = spritesTimer[combat];
    }

    public void HideCombatUI(bool state)
    {
        if(timerCombat != null)
            timerCombat.gameObject.SetActive(state);
    }
}
