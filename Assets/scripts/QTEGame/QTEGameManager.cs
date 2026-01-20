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
            mainText.text = elapsedTime.ToString("F2");
            yield return null;
        }
    }

}
