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
    public Text timerText;
    public Image logo;
    public Sprite[] spritesTimer;

    private void Start()
    {
        timerCombat.gameObject.SetActive(false);
    }

    public void UpdateCombatUI(int combat)
    {
        if(logo != null)
        {
            timerText.text = combat.ToString();
            logo.sprite = spritesTimer[combat];
        }
    }

    public void HideCombatUI(bool state)
    {
        if(timerCombat != null)
            timerCombat.gameObject.SetActive(state);
    }
}
