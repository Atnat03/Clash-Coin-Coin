using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QTEGameManager : MonoBehaviour
{
    public static QTEGameManager instance;
    public float gameLength;
    
    public bool inGame = false;
    public TextMeshProUGUI mainText;
    public Sprite southButtonSprite;
    public Sprite northButtonSprite;
    public Sprite eastButtonSprite;
    public Sprite westButtonSprite;

    [SerializeField]private QTEPlayerScript P1;
    [SerializeField]private QTEPlayerScript P2;

    public enum ButtonDirection
    {
        south,
        west,
        north,
        east
    }
    
    public Dictionary<ButtonDirection, Sprite> buttonSprites = new Dictionary<ButtonDirection, Sprite>();

    void Awake()
    {
        if(instance == null)instance = this;
        buttonSprites.Add(ButtonDirection.north, northButtonSprite);
        buttonSprites.Add(ButtonDirection.south, southButtonSprite);
        buttonSprites.Add(ButtonDirection.east, eastButtonSprite);
        buttonSprites.Add(ButtonDirection.west, westButtonSprite);
    }

    void Start()
    {
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        float elapsedTime = 3;

        yield return new WaitForSeconds(3.5f);

        mainText.text = "";
        inGame = true;

        elapsedTime = gameLength;
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            mainText.text = elapsedTime.ToString("F2");
            yield return null;
        }

        inGame = false;
        
        mainText.text = "partie terminée !";
        yield return new WaitForSeconds(1);
        if(P1.score == P2.score)mainText.text = "égalité !";
        else if (P1.score > P2.score) mainText.text = "Joueur 1 a gagné !";
        else mainText.text = "Joueur 2 a gagné !";
        
        yield return new WaitForSeconds(2f);

        GameManager.instance.player_1_Score = 1;
        GameManager.instance.player_2_Score = 1;
        
        GameManager.instance.ReturnToMainScene();
    }

}
