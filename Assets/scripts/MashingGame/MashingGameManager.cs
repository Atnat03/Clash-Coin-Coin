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
    
    
    public TextMeshProUGUI mainTextMesh;
    public Image cooldownBar;

    public Action StartGame;

    [SerializeField] private MashIngGamePlayerScript P1;
    [SerializeField] private MashIngGamePlayerScript P2;
    
    void Awake()
    {
        if(instance == null)instance = this;
    }
    
    void Start()
    {
        cooldownBar.gameObject.SetActive(false);

        
        StartCoroutine(StartGameCoroutine());
    }
    
    public IEnumerator StartGameCoroutine()
    {
        float timeCounter = 3;
        while (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime;
            mainTextMesh.text = Mathf.CeilToInt(timeCounter).ToString("F0");
            yield return null;
        }

        mainTextMesh.text = "Mashez !!!";
        yield return new WaitForSeconds(1);
        
        mainTextMesh.text = "";
        cooldownBar.gameObject.SetActive(true);
        cooldownBar.fillAmount = 1;
        StartGame?.Invoke();

        timeCounter = GameLength;
        while (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime;
            mainTextMesh.text = timeCounter.ToString("F2");
            cooldownBar.fillAmount = timeCounter / GameLength;
            yield return null;
        }
        
        cooldownBar.gameObject.SetActive(false);
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
