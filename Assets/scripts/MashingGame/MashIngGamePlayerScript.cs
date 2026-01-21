using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MashIngGamePlayerScript : MonoBehaviour
{
    public float P1JaugeFillAmout;
    public Image P1Jauge;
    
    public bool ingame;

    public int playerID;

    public float ratioPallier1;
    public float ratioPallier2;

    PlayerInput playerInput;

    public Animator animButton;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        MashingGameManager.instance.StartGame += StartGame;
    }
    
    public void StartGame()
    {
        P1Jauge.fillAmount = P1JaugeFillAmout;

        StartCoroutine(GameCoroutine());
    }

    public IEnumerator GameCoroutine()
    {
        ingame = true;
        float elapsedTime = 0f;
        while (elapsedTime < MashingGameManager.instance.GameLength && !MashingGameManager.instance.someoneWon)
        {
            elapsedTime += Time.deltaTime;
            
            P1Jauge.fillAmount = P1JaugeFillAmout;
            if (P1JaugeFillAmout >= 0.95f)
            {
                MashingGameManager.instance.someoneWon = true;
            }
            yield return null;
        }

        if (P1JaugeFillAmout >= ratioPallier1)
        {
            if (playerID == 1) MashingGameManager.instance.pallierJoueur1 = 2;
            if (playerID == 2) MashingGameManager.instance.pallierJoueur2 = 2;
        }
        if (P1JaugeFillAmout >= ratioPallier2)
        {
            if (playerID == 1) MashingGameManager.instance.pallierJoueur1 = 3;
            if (playerID == 2) MashingGameManager.instance.pallierJoueur2 = 3;
        }

        ingame = false;
    }

    public void PlayerPressedA(InputAction.CallbackContext context)
    {
        if (ingame && context.performed)
        {
            P1JaugeFillAmout = Mathf.Clamp(P1JaugeFillAmout + MashingGameManager.instance.amountPerClic, 0f, 1f);
            animButton.SetTrigger("Clic");
        }
    }
}
