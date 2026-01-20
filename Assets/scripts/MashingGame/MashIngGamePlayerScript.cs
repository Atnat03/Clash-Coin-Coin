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

    PlayerInput playerInput;

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
        while (elapsedTime < MashingGameManager.instance.GameLength)
        {
            elapsedTime += Time.deltaTime;
            
            P1Jauge.fillAmount = P1JaugeFillAmout;
            
            yield return null;
        }
        ingame = false;
    }

    public void PlayerPressedA(InputAction.CallbackContext context)
    {
        Debug.Log($"{gameObject.name} a reçu input ! Phase: {context.phase} | Value: {context.ReadValueAsButton()}");

        if (ingame && context.performed)
        {
            P1JaugeFillAmout = Mathf.Clamp(P1JaugeFillAmout + MashingGameManager.instance.amountPerClic, 0f, 1f);
            Debug.Log($"{gameObject.name} jauge mise à jour : {P1JaugeFillAmout}");
        }
    }
}
