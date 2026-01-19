using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MashIngGamePlayerScript : MonoBehaviour
{
    public float P1JaugeFillAmout = 1f;
    public Image P1Jauge;
    
    public bool ingame;

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
            
            P1JaugeFillAmout -= Time.deltaTime/2;
            
            P1Jauge.fillAmount = P1JaugeFillAmout;
            
            yield return null;
        }
        ingame = false;
    }

    public void PlayerPressedA(InputAction.CallbackContext context)
    {
        if(ingame)P1JaugeFillAmout = Mathf.Clamp(P1JaugeFillAmout + .06f,0 , 1);
    }
}
