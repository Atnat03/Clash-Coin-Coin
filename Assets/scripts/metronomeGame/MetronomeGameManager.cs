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
    public float cursorDefaultSpeed = 1;
    
    public TextMeshProUGUI mainText;
    
    public Action BeginGame;

    [SerializeField] private MetronomePlayerScript P1;
    [SerializeField] private MetronomePlayerScript P2;
    
    void Awake()
    {
        if(instance == null)instance = this;
    }

    void Start()
    {
        mainText.text = "";
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        float elapsedTime = 3;
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            mainText.text = (Mathf.CeilToInt(elapsedTime)).ToString();
            yield return null;
        }
        mainText.text = "go !";
        
        yield return new WaitForSeconds(1f);
        BeginGame?.Invoke();

        elapsedTime = gameLength;
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            mainText.text = elapsedTime.ToString("F2");
            yield return null;
        }
        
        mainText.text = "terminé !";
        yield return new WaitForSeconds(1f);

        if (P1 != null && P2 != null)
        {
            if (P1.points >= P2.points) mainText.text = "joueur 1 a gagné !";
            else mainText.text = "joueur 2 a gagné !";
        }
        
        yield return new WaitForSeconds(2f);

        GameManager.instance.player_1_Score = 1;
        GameManager.instance.player_2_Score = 1;
        
        GameManager.instance.ReturnToMainScene();
    }
}
