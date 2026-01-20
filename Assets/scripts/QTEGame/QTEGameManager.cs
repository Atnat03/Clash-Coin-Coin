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

    [SerializeField]private QTEPlayerScript P1;
    [SerializeField]private QTEPlayerScript P2;

    public Image timerImage;
    public bool finished;

    public enum ButtonDirection
    {
        south,
        west,
        north,
        east
    }
    
    public Dictionary<ButtonDirection, Sprite> buttonSprites = new Dictionary<ButtonDirection, Sprite>();
    public Animator animUI;
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
        while (elapsedTime > 0 && !finished)
        {
            elapsedTime -= Time.deltaTime;
            timerImage.fillAmount = elapsedTime / gameLength;
            timerImage.color = Color.Lerp(new Color(1f,0.3f,0.3f), new Color(0.3f,1f,0.3f), elapsedTime / gameLength);
            mainText.text = elapsedTime.ToString("F2");
            yield return null;
        }

        if (elapsedTime < 0) mainText.text = "0";

        inGame = false;
        
        animUI.SetTrigger("Over");
        
        yield return new WaitForSeconds(2f);

        GameManager.instance.player_1_Score = 1;
        GameManager.instance.player_2_Score = 1;
        
        GameManager.instance.ReturnToMainScene();
    }

}
