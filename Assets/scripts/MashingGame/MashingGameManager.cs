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

    public int pallierJoueur1, pallierJoueur2;
    
    void Awake()
    {
        if(instance == null)instance = this;
    }
    
    void Start()
    {
        StartCoroutine(StartGameCoroutine());
        AudioManager.instance.PlayRandomMusic();
    }
    
    public IEnumerator StartGameCoroutine()
    {
        someoneWon = false;
        yield return new WaitForSeconds(1.4f);
        AudioManager.instance.PlaySound(AudioManager.instance.startSound,0.8f);
        yield return new WaitForSeconds(3.2f);
        
        cooldownBar.gameObject.SetActive(true);
        cooldownBar.fillAmount = 1;
        StartGame?.Invoke();

        float timeCounter = GameLength;
        while (timeCounter > 0 && !someoneWon)
        {
            timeCounter -= Time.deltaTime;
            timerText.text = timeCounter.ToString("F2");
            float fill = timeCounter / GameLength;
            cooldownBar.fillAmount = fill;
            cooldownBar.color = Color.Lerp(new Color(1f,0.3f,0.3f), new Color(0.3f,1f,0.3f), fill);

            yield return null;
        }

        if (timeCounter < 0) timerText.text = "0:00";
        
        AudioManager.instance.PlaySound(AudioManager.instance.endSound);
        animUI.SetTrigger("Over");

        yield return new WaitForSeconds(1.5f);

        EndGame();
    }

    public Animator animUI;

    private void EndGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.player_1_Score = pallierJoueur1;
            GameManager.instance.player_2_Score = pallierJoueur2;

            GameManager.instance.ReturnToMainScene();
        }
    }
}
