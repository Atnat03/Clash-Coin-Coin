using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurlingGameManager : MonoBehaviour
{
    public static CurlingGameManager instance;

    public float jaugeFillingSpeed = 0.05f;

    public float gameLength;
    public bool inGame = false;
    
    public TextMeshProUGUI chronoText;

    public CurplingPlayerScript P1;
    public CurplingPlayerScript P2;
    
    public Image cooldownBar;

    public Animator animUI;
    
    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        float elapsedTime = 3;
        yield return new WaitForSeconds(0.1f);
        AudioManager.instance.PlaySound(AudioManager.instance.startSound);
        yield return new WaitForSeconds(3.4f);
        
        inGame = true;

        elapsedTime = gameLength;
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            chronoText.text = elapsedTime.ToString("F2");
            float fill = elapsedTime / gameLength;
            cooldownBar.fillAmount = fill;
            cooldownBar.color = Color.Lerp(new Color(1f,0.3f,0.3f), new Color(0.3f,1f,0.3f), fill);
            yield return null;
        }
        AudioManager.instance.PlaySound(AudioManager.instance.endSound);
        animUI.SetTrigger("Over");
        
        

        if (P1.played == false)
        {
            GameManager.instance.player_1_Score = 1;
            GameManager.instance.player_2_Score = 3;
            yield break;
        }

        if (P2.played == false)
        {
            GameManager.instance.player_1_Score = 3;
            GameManager.instance.player_2_Score = 1;
            yield break;
        }

        if (P1.score > P2.score)
        {
            GameManager.instance.player_1_Score = 1;
            GameManager.instance.player_2_Score = 3;
        }
        else
        {
            GameManager.instance.player_1_Score = 3;
            GameManager.instance.player_2_Score = 1;
        }
        
        yield return new WaitForSeconds(1f);
        
        GameManager.instance.ReturnToMainScene();
    }
}
