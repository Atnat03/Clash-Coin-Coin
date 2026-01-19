using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MashingGameManager : MonoBehaviour
{
    public static MashingGameManager instance;
    
    public float GameLength = 10f;
    public TextMeshProUGUI maintextMesh;

    public Action StartGame;

    [SerializeField] private MashIngGamePlayerScript P1;
    [SerializeField] private MashIngGamePlayerScript P2;

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
        float timeCounter = 3;
        while (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime;
            maintextMesh.text = Mathf.CeilToInt(timeCounter).ToString("F0");
            yield return null;
        }

        maintextMesh.text = "go !!!";
        yield return new WaitForSeconds(1);
        
        maintextMesh.text = "";
        StartGame?.Invoke();
        
        yield return new WaitForSeconds(GameLength);
        
        maintextMesh.text = "game Finished !";

        yield return new WaitForSeconds(2f);

        if (P1 != null && P2 != null)
        {
            if (P1.P1JaugeFillAmout > P2.P1JaugeFillAmout) maintextMesh.text = "player 1 wins";
            else maintextMesh.text = "player 2 wins";
        }
    }
}
