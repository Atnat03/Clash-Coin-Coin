using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MetronomeGameManager : MonoBehaviour
{
    public static MetronomeGameManager instance;

    public float gameLength;
    [Tooltip("le slider va de -1 a 1")]public float SliderTolerence = .1f;
    public int pointsToScore;
    public float cursorAccelerationFactor = 1.2f;
    
    public TextMeshProUGUI mainText;
    
    public Action BeginGame;
    
    void Awake()
    {
        if(instance == null)instance = this;
    }

    void Start()
    {
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        yield return new WaitForSeconds(1f);
        BeginGame?.Invoke();
    }
}
