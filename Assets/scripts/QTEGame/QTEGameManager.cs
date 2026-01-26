using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    
    public Sprite[] p1keyboardSprites;
    public Sprite[] p2keyboardSprites;

    [SerializeField]private QTEPlayerScript P1;
    [SerializeField]private QTEPlayerScript P2;

    public Image timerImage;
    public bool finished;


    public int scorePalier1;
    public int scorePalier2;
    public int scoreMax;
    public int score1, score2;

    public enum ButtonDirection
    {
        south,
        west,
        north,
        east
    }
    
    public Dictionary<ButtonDirection, Sprite> buttonSpritesP1 = new Dictionary<ButtonDirection, Sprite>();
    public Dictionary<ButtonDirection, Sprite> buttonSpritesP2 = new Dictionary<ButtonDirection, Sprite>();
    public Animator animUI;
    void Awake()
    {
        if(instance == null)instance = this;
        SetUpInputSprite();
    }

    void Start()
    {
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        float elapsedTime = 3;
        yield return new WaitForSeconds(1.4f);
        AudioManager.instance.PlaySound(AudioManager.instance.startSound,0.8f);
        yield return new WaitForSeconds(2f);
        AudioManager.instance.PlayMusic(AudioManager.instance.miniGame);
        yield return new WaitForSeconds(1.2f);

        mainText.text = "";
        inGame = true;

        elapsedTime = gameLength;
        while (elapsedTime > 0 && !finished)
        {
            elapsedTime -= Time.deltaTime;
            timerImage.fillAmount = elapsedTime / gameLength;
            timerImage.color = Color.Lerp(new Color(1f,0.3f,0.3f), new Color(0.3f,1f,0.3f), elapsedTime / gameLength);
            mainText.text = elapsedTime.ToString("F2");
            yield return null;

            if (score1 == scoreMax || score2 == scoreMax)
            {
                finished = true;
            }
        }

        if (elapsedTime < 0) mainText.text = "0";

        inGame = false;
        AudioManager.instance.PlaySound(AudioManager.instance.endSound);
        animUI.SetTrigger("Over");
        
        yield return new WaitForSeconds(2f);
        
        GameManager.instance.player_1_Score = VerifyScore(P1.score);
        GameManager.instance.player_2_Score = VerifyScore(P2.score);
        
        GameManager.instance.ReturnToMainScene();
    }

    private void SetUpInputSprite()
    {
        EnableInputs inputs = GetComponent<EnableInputs>();

        if (inputs.numberManettes == 2)
        {
            buttonSpritesP1.Add(ButtonDirection.north, northButtonSprite);
            buttonSpritesP1.Add(ButtonDirection.south, southButtonSprite);
            buttonSpritesP1.Add(ButtonDirection.east, eastButtonSprite);
            buttonSpritesP1.Add(ButtonDirection.west, westButtonSprite);
            
            buttonSpritesP2.Add(ButtonDirection.north, northButtonSprite);
            buttonSpritesP2.Add(ButtonDirection.south, southButtonSprite);
            buttonSpritesP2.Add(ButtonDirection.east, eastButtonSprite);
            buttonSpritesP2.Add(ButtonDirection.west, westButtonSprite);
            
            print("2 manettes");
        }
        else if (inputs.numberManettes == 1)
        {
            print("1 manette");
            
            buttonSpritesP1.Add(ButtonDirection.north, p1keyboardSprites[1]);
            buttonSpritesP1.Add(ButtonDirection.south, p1keyboardSprites[0]);
            buttonSpritesP1.Add(ButtonDirection.east, p1keyboardSprites[2]);
            buttonSpritesP1.Add(ButtonDirection.west, p1keyboardSprites[3]);
            
            buttonSpritesP2.Add(ButtonDirection.north, northButtonSprite);
            buttonSpritesP2.Add(ButtonDirection.south, southButtonSprite);
            buttonSpritesP2.Add(ButtonDirection.east, eastButtonSprite);
            buttonSpritesP2.Add(ButtonDirection.west, westButtonSprite);
        }
        else if (inputs.numberManettes == 0)
        {
            print("0 manette");
            
            buttonSpritesP1.Add(ButtonDirection.north, p1keyboardSprites[1]);
            buttonSpritesP1.Add(ButtonDirection.south, p1keyboardSprites[0]);
            buttonSpritesP1.Add(ButtonDirection.east, p1keyboardSprites[2]);
            buttonSpritesP1.Add(ButtonDirection.west, p1keyboardSprites[3]);
            
            buttonSpritesP2.Add(ButtonDirection.north, p2keyboardSprites[1]);
            buttonSpritesP2.Add(ButtonDirection.south, p2keyboardSprites[0]);
            buttonSpritesP2.Add(ButtonDirection.east, p2keyboardSprites[2]);
            buttonSpritesP2.Add(ButtonDirection.west, p2keyboardSprites[3]);
        }
    }

    int VerifyScore(int score)
    {
        if (score >= scorePalier1)
        {
            if (score >= scorePalier2)
            {
                return 3;
            }
            return 2;
        }
        return 1;
    }

}
