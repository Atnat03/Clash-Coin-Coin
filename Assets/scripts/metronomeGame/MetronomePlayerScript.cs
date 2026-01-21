using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MetronomePlayerScript : MonoBehaviour
{
    public Slider slider;

    public float cursorSpeed;
    public int points;
    
    public bool ingame;
    public int playerID;

    public void Start()
    {
        ingame = false;
        cursorSpeed = MetronomeGameManager.instance.cursorDefaultSpeed;
        MetronomeGameManager.instance.BeginGame += BeginGame;
    }

    public void BeginGame()
    {
        ingame = true;
        StartCoroutine(PlayGame());
    }

    private float elapsedTime = 0f;
    
    public IEnumerator PlayGame()
    {
        elapsedTime = 0f;
        while (elapsedTime < MetronomeGameManager.instance.gameLength)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Sin(elapsedTime * cursorSpeed);
            
            if (slider.value > 0.99f)
            {
                float phase = elapsedTime * cursorSpeed;
        
                cursorSpeed *= MetronomeGameManager.instance.cursorAccelerationFactor;
        
                elapsedTime = phase / cursorSpeed;
            }
            yield return null;
        }
    }
    
    public void PlayerPressedA(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        
        if (ingame)
        {
            if (Mathf.Abs(slider.value) <= MetronomeGameManager.instance.SliderTolerence && points < MetronomeGameManager.instance.pointsToScore)
            {
                ScorePoint();
            }
        }
    }

    public void ScorePoint()
    {
        if (playerID == 1) MetronomeGameManager.instance.cursorPosition--;
        if (playerID == 2) MetronomeGameManager.instance.cursorPosition++;
    }

}
