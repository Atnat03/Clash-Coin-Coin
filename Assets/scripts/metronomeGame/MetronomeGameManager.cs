using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetronomeGameManager : MonoBehaviour
{
    public static MetronomeGameManager instance;

    public float gameLength;
    [Tooltip("le slider va de -1 a 1")]public float SliderTolerence = .1f;
    public int pointsToScore;
    public float cursorAccelerationFactor = 1.2f;
    public float cursorDefaultSpeed = 1;
    
    public TextMeshProUGUI mainText;
    
    public Action BeginGame;

    [SerializeField] private MetronomePlayerScript P1;
    [SerializeField] private MetronomePlayerScript P2;

    public int score1, score2;

    public int cursorPosition;
    public Slider pointSlider;
    
    void Awake()
    {
        if(instance == null)instance = this;
    }

    void Start()
    {
        mainText.text = "";
        StartCoroutine(GameCoroutine());
    }

    public Animator animUI;

    IEnumerator GameCoroutine()
    {
        float elapsedTime = 3;
        
        yield return new WaitForSeconds(3.5f);
        BeginGame?.Invoke();

        elapsedTime = gameLength;
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            mainText.text = elapsedTime.ToString("F2");
            yield return null;

            pointSlider.value = (float)cursorPosition / (float)pointsToScore;

            if (cursorPosition == pointsToScore || cursorPosition == -pointsToScore)
            {
                
            }
        }
        
        animUI.SetTrigger("Over");
        
        yield return new WaitForSeconds(1.5f);

        
        if (cursorPosition < 0)
        {
            GameManager.instance.player_1_Score = 3;
            GameManager.instance.player_2_Score = 1; 
        }
        if (cursorPosition > 0)
        {
            GameManager.instance.player_1_Score = 1;
            GameManager.instance.player_2_Score = 3; 
        }

        if (cursorPosition == 0)
        {
            GameManager.instance.player_1_Score = 2;
            GameManager.instance.player_2_Score = 2; 
        }
        
        
        GameManager.instance.ReturnToMainScene();
    }
}
