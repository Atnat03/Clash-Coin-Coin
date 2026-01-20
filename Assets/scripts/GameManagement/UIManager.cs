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

    [Header("Reward")] 
    public Button buttonReward_P1;
    public Button buttonReward_P2;

    private void Start()
    {
        timerCombat.gameObject.SetActive(false);
        buttonReward_P1.gameObject.SetActive(false);
        buttonReward_P2.gameObject.SetActive(false);
    }

    public void UpdateCombatUI(int combat)
    {
        timerCombat.text = combat.ToString();
    }

    public void HideCombatUI(bool state)
    {
        timerCombat.gameObject.SetActive(state);
    }

    public void ShowReward()
    {
        buttonReward_P1.gameObject.SetActive(true);
        buttonReward_P2.gameObject.SetActive(true);

        buttonReward_P1.onClick.AddListener(() =>
            SpawnPlayer.instance.placementSystems[0].StartPlacement(0));
                    
        buttonReward_P1.onClick.AddListener( () => 
                GameManager.instance.players[0].HasReward = true);
        
        buttonReward_P2.onClick.AddListener( () => 
            SpawnPlayer.instance.placementSystems[1].StartPlacement(0));
        
        buttonReward_P2.onClick.AddListener( () => 
            GameManager.instance.players[1].HasReward = true);
    }
}
