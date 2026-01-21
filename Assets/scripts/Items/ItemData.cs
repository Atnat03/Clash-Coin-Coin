using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    [HideInInspector] public int id;
    public string name;
    public Vector3Int position;
    public Vector2Int scale;
    public bool playerOneProperty;
    public float PV;
    public float maxPV;
    public GameObject prefab;
    
    public string prefabPath;
}

[System.Serializable]
public class GameSaveData
{
    public List<ItemData> itemsP1 = new List<ItemData>();
    public List<ItemData> itemsP2 = new List<ItemData>();
    public int player1Score = -1;
    public int player2Score = -1;
    public float combatDuration = 20f;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    
    private const string SAVE_KEY = "GameSaveData";
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();
        
        // Sauvegarder les items placés
        saveData.itemsP1 = new List<ItemData>(GameManager.instance.itemPlacedDataP1);
        saveData.itemsP2 = new List<ItemData>(GameManager.instance.itemPlacedDataP2);
        
        // Sauvegarder les scores
        saveData.player1Score = GameManager.instance.player_1_Score;
        saveData.player2Score = GameManager.instance.player_2_Score;
        saveData.combatDuration = GameManager.instance.CombatDuration;
        
        string json = JsonUtility.ToJson(saveData, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        
        Debug.Log("Jeu sauvegardé : " + json);
    }
    
    public GameSaveData LoadGame()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            
            Debug.Log("Jeu chargé : " + json);
            return saveData;
        }
        
        Debug.Log("Aucune sauvegarde trouvée");
        return null;
    }
    
    public void DeleteSave()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("Sauvegarde supprimée");
        }
    }
    
    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }
}