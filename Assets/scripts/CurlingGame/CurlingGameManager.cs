using System.Collections;
using TMPro;
using UnityEngine;

public class CurlingGameManager : MonoBehaviour
{
    public static CurlingGameManager instance;

    public float jaugeFillingSpeed = 0.05f;

    public float gameLength;
    public bool inGame = false;
    
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI chronoText;

    public CurplingPlayerScript P1;
    public CurplingPlayerScript P2;
    
    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        chronoText.text = "";
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
        
        yield return new WaitForSeconds(1);
        
        mainText.text = "";
        inGame = true;

        elapsedTime = gameLength;
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            chronoText.text = elapsedTime.ToString("F2");
            yield return null;
        }
        
        chronoText.text = "";
        mainText.text = "fini !";
        
        yield return new WaitForSeconds(1);

        if (P1.played == false)
        {
            mainText.text = "joueur 2 a gagné !";
            yield break;
        }

        if (P2.played == false)
        {
            mainText.text = "joueur 1 a gagné !";
            yield break;
        }

        if (P1.score > P2.score)
        {
            mainText.text = "joueur 2 a gagné";
        }
        else
        {
            mainText.text = "joueur 1 a gagné";
        }
        
    }
}
