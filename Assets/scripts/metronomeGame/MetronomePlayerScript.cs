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
    
    public TextMeshProUGUI text;
    public Image jaugePoints;
    public bool ingame;

    public void Start()
    {
        jaugePoints.fillAmount = 0;
        ingame = false;
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
            yield return null;
        }
    }
    
    public void PlayerPressedA(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        
        if (ingame)
        {
            if (Mathf.Abs(slider.value) <= MetronomeGameManager.instance.SliderTolerence
                && points < MetronomeGameManager.instance.pointsToScore)
            {
                ScorePoint();
            }
        }
    }

    public void ScorePoint()
    {
        points++;
        points = Mathf.Clamp(points, 0, MetronomeGameManager.instance.pointsToScore);

        text.text = "points : " + points;
        jaugePoints.fillAmount = (float)points / MetronomeGameManager.instance.pointsToScore;
        
        float phase = elapsedTime * cursorSpeed;
        
        cursorSpeed *= 1.2f;
        
        elapsedTime = phase / cursorSpeed;
    }

}
