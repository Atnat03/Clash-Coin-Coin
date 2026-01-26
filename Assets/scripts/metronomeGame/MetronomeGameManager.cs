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
    
    [SerializeField] private Sprite[] spritesA;
    [SerializeField] private Image imageInputA_p1, imageInputA_p2;

    public int score1, score2;

    public int cursorPosition;
    public Slider pointSlider;
    
    public Image cooldownBar;
    
    void Awake()
    {
        if(instance == null)instance = this;
    }

    void Start()
    {
        mainText.text = "";
        StartCoroutine(GameCoroutine());
        
                
        EnableInputs inputs = GetComponent<EnableInputs>();

        switch (inputs.numberManettes)
        {
            case 2:
                imageInputA_p1.sprite = spritesA[0];
                imageInputA_p2.sprite = spritesA[0];
                break;
            case 1:
                imageInputA_p1.sprite = spritesA[1];
                imageInputA_p2.sprite = spritesA[0];
                break;
            case 0: 
                imageInputA_p1.sprite = spritesA[1];
                imageInputA_p2.sprite = spritesA[2];
                break;
        }
    }

    public Animator animUI;
    public bool finished;
    
    [SerializeField] private float sliderSmoothSpeed = 5f;

    private float targetSliderValue;

    void Update()
    {
        targetSliderValue = (float)cursorPosition / (float)pointsToScore;

        pointSlider.value = Mathf.Lerp(pointSlider.value, targetSliderValue, Time.deltaTime * sliderSmoothSpeed);
    }

    IEnumerator GameCoroutine()
    {
        float elapsedTime = 3;
        yield return new WaitForSeconds(1.4f);
        AudioManager.instance.PlaySound(AudioManager.instance.startSound,0.8f);       
        yield return new WaitForSeconds(2f);
        AudioManager.instance.PlayMusic(AudioManager.instance.miniGame);
        yield return new WaitForSeconds(1.2f);
        BeginGame?.Invoke();

        elapsedTime = gameLength;
        while (elapsedTime > 0 && !finished)
        {
            elapsedTime -= Time.deltaTime;
            mainText.text = elapsedTime.ToString("F2");
            yield return null;
            
            float fill = elapsedTime / gameLength;
            cooldownBar.fillAmount = fill;
            cooldownBar.color = Color.Lerp(new Color(1f,0.3f,0.3f), new Color(0.3f,1f,0.3f), fill);

            if (cursorPosition == pointsToScore || cursorPosition == -pointsToScore)
            {
                finished = true;
            }
        }
        AudioManager.instance.PlaySound(AudioManager.instance.endSound);
        animUI.SetTrigger("Over");
        
        yield return new WaitForSeconds(1.5f);

        GameManager.instance.player_1_Score = 0;
        GameManager.instance.player_2_Score = 0;
        
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
