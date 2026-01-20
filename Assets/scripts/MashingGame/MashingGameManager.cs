using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MashingGameManager : MonoBehaviour
{
    public static MashingGameManager instance;
    
    public float GameLength = 10f;

    [Tooltip("le max de la jauge c'est 1")] public float amountPerClic;
    
    
    public TextMeshProUGUI mainTextMesh, timerText;
    public Image cooldownBar;

    public Action StartGame;

    [SerializeField] private MashIngGamePlayerScript P1;
    [SerializeField] private MashIngGamePlayerScript P2;

    public bool someoneWon;
    
    void Awake()
    {
        if(instance == null)instance = this;
    }
    
    void Start()
    {
        StartCoroutine(StartGameCoroutine());
    }
    
    public IEnumerator StartGameCoroutine()
    {
        someoneWon = false;
        float timeCounter = 3;
        while (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime*1.5f;
            mainTextMesh.text = Mathf.CeilToInt(timeCounter).ToString("F0");
            yield return null;
        }

        mainTextMesh.text = "Mashez !";
        yield return new WaitForSeconds(0.5f);
        
        mainTextMesh.text = "";
        cooldownBar.gameObject.SetActive(true);
        cooldownBar.fillAmount = 1;
        StartGame?.Invoke();

        timeCounter = GameLength;
        while (timeCounter > 0 && !someoneWon)
        {
            timeCounter -= Time.deltaTime;
            timerText.text = timeCounter.ToString("F2");
            float fill = timeCounter / GameLength;
            cooldownBar.fillAmount = fill;
            cooldownBar.color = Color.Lerp(new Color(1f,0.3f,0.3f), new Color(0.3f,1f,0.3f), fill);

            yield return null;
        }
        
        
        mainTextMesh.text = "Fini !";

        yield return new WaitForSeconds(2f);

        EndGame();

        if (P1 != null && P2 != null)
        {
            if (P1.P1JaugeFillAmout > P2.P1JaugeFillAmout) mainTextMesh.text = "Le joueur 1 a gagné";
            else mainTextMesh.text = "Le joueur 2 a gagné";
        }
    }

    private void EndGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.player_1_Score = 2;
            GameManager.instance.player_2_Score = 1;

            GameManager.instance.ReturnToMainScene();
        }
    }
}
