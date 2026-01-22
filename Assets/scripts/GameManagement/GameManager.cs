using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameSate
{
    StartGame,
    Loading,
    Transition,
    Reward,
    Prepare,
    Combat,
    MiniGame,
    EndGame
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public StateMachine<GameSate> stateMachine =  new StateMachine<GameSate>();
    
    public List<Item> placedItemsP1 = new List<Item>();
    public List<Item> placedItemsP2 = new List<Item>();
    public List<ItemData> itemPlacedDataP1 = new List<ItemData>();
    public List<ItemData> itemPlacedDataP2 = new List<ItemData>();
    
    public string[] miniGames;
    public int player_1_Score = -1;
    public int player_2_Score = -1;
    
    [Header("Troop Prefabs")]
    public List<GameObject> troopPrefabsP1;
    public List<GameObject> troopPrefabsP2;

    public float PVNexus_P1 = 200;
    public float maxPVNexus_P1;
    public float PVNexus_P2 = 200;
    public float maxPVNexus_P2;

    public GameObject uiEnd;
    public int winIndex = -1;
    public bool isEnd = false;

    public int MAX_NUMBER_TOUR = 20;
    public int CURRENT_NUMBER_TOUR = 0;
    public Text toursTXT;
    
    void Awake()
    {
        if(instance == null)instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }
    
    public void AddItemInList(Item item, Vector3Int position)
    {
        ItemData itemData = new ItemData();
        itemData.id = item.id;
        itemData.name = item.name;
        itemData.position = position;
        itemData.playerOneProperty = item.playerOneProperty;
        itemData.PV = item.PV;
        itemData.maxPV = item.maxPV;

        ItemSO database = itemData.playerOneProperty
            ? VariablesManager.instance.placementSystems[0].database
            : VariablesManager.instance.placementSystems[1].database;

        var itemInfo = database.itemsData.Find(x => x.Id == item.id);
        if (itemInfo != null)
        {
            itemData.scale = itemInfo.Size;
            itemData.prefab = itemInfo.Prefab;
        }

        if (itemData.playerOneProperty)
            itemPlacedDataP1.Add(itemData);
        else
            itemPlacedDataP2.Add(itemData);
    }

    public void UpdateNexusP1(float value) => PVNexus_P1 = value;
    public void UpdateNexusP2(float value) => PVNexus_P2 = value;
    
    void Start()
    {
        maxPVNexus_P1 = PVNexus_P1;
        maxPVNexus_P2 = PVNexus_P2;
        
        //StartGame
        stateMachine.Add(new State<GameSate>(
            GameSate.StartGame,
            onEnter: StartEnter,
            onUpdate: StartUpdate
        ));
        
        //Loading
        stateMachine.Add(new State<GameSate>(
            GameSate.Loading,
            onEnter:()=> Debug.Log("Enter  Loading")
        ));
        
        //Transition
        stateMachine.Add(new State<GameSate>(
            GameSate.Transition,
            onEnter:()=> Debug.Log("Enter  Transition")
        ));
        
        //Reward
        stateMachine.Add(new State<GameSate>(
            GameSate.Reward,
            onEnter:RewardEnter,
            onUpdate:RewardUpdate
        ));
        
        //Prepare
        stateMachine.Add(new State<GameSate>(
            GameSate.Prepare,
            onEnter:PrepareEnter,
            onUpdate: PrepareUpdate
        ));
        
        //Combat
        stateMachine.Add(new State<GameSate>(
            GameSate.Combat,
            onEnter: CombatEnter,
            onUpdate: CombatUpdate,
            onExit: CombatExit
        ));
        
        //MiniGame
        stateMachine.Add(new State<GameSate>(
            GameSate.MiniGame,
            onEnter:StartMiniGame,
            onUpdate:UpdateMiniGame
        ));
        
        //EndGame
        stateMachine.Add(new State<GameSate>(
            GameSate.EndGame,
            onEnter:EndGameEnter,
            onExit: EndGameExit
        ));
        
        stateMachine.ChangeState(GameSate.MiniGame);
    }

    void EndGameEnter()
    {
        Debug.Log("Enter EndGame");
        
        uiEnd.gameObject.SetActive(true);
        uiEnd.GetComponent<UiEnd>().SetUp(winIndex);
        SetAllPlacedItems(false);

        VariablesManager.instance.SetInputA();
    }

    void EndGameExit()
    {
        VariablesManager.instance.DisableInputA();
    }
    
    public void SetAllPlacedItems(bool state)
    {
        foreach (Item item in placedItemsP1)
        {
            if(item != null)
                item.SetActive(state);
        }
        foreach (Item item in placedItemsP2)
        {
            if(item != null)
                item.SetActive(state);
        }
    }
    
    public void RemovePlacedItem(Item item)
    {
        if (placedItemsP1.Contains(item))
        {
            placedItemsP1.Remove(item);
        }
        else
        {
            if (placedItemsP2.Contains(item))
            {
                placedItemsP2.Remove(item);
            }
        }
    }

    public bool EndOfTurn()
    {
        bool isTroop = true;
        
        List<Item> l = placedItemsP1.Concat(placedItemsP2).ToList();
        
        foreach (Item item in l)
        {
            if(item is Troop t)
                isTroop = false;
        }

        return isTroop;
    }
    
    public void RemovePlacedDataItem(Item item)
    {
        ItemData dataP1 = itemPlacedDataP1.Find(x => x.id == item.id && x.prefab.GetInstanceID() == item.GetInstanceID());
        if (dataP1 != null)
        {
            itemPlacedDataP1.Remove(dataP1);
            return;
        }

        ItemData dataP2 = itemPlacedDataP2.Find(x => x.id == item.id && x.prefab.GetInstanceID() == item.GetInstanceID());
        if (dataP2 != null)
        {
            itemPlacedDataP2.Remove(dataP2);
        }
    }


    public bool isAllPlayerReadyToFight()
    {
        foreach (PlayerInputing player in VariablesManager.instance.players)
        {
            if (!player.IsReady) return false;
        }

        // 🔧 Vérifier P1
        foreach (Item item in placedItemsP1)
        {
            if(item is Troop troop)
                if(troop.GetComponent<AnimFlip>().did == false)
                    return false;
        }
    
        // 🔧 Vérifier P2 (pas P1 !)
        foreach (Item item in placedItemsP2)
        {
            if(item is Troop troop)
                if(troop.GetComponent<AnimFlip>().did == false)
                    return false;
        }

        return true;
    }
    
    #region Function Calling

    void Update()
    {
        stateMachine?.Update();

        toursTXT.text = "Tour " + CURRENT_NUMBER_TOUR;

        if(isEnd) return;
        
        if (PVNexus_P1 <= 0)
        {
            GameOverByDead(0);
        }

        if (PVNexus_P2 <= 0)
        {
            GameOverByDead(1);
        }
    }

    private void GameOverByDead(int i)
    {
        StopAllCoroutines();
        isEnd = true;
        winIndex = i;
        stateMachine?.ChangeState(GameSate.EndGame);
    }
    
    private void GameOverByEndNbTurn()
    {
        isEnd = true;
        winIndex = PVNexus_P1 >= PVNexus_P2 ? 0 : 1;
        stateMachine?.ChangeState(GameSate.EndGame);
    }

    void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
    }
    
    
    #endregion
    
    #region StartState

    void StartEnter()
    {
        Debug.Log("Enter Start");
    }
    
    void StartUpdate()
    {

    }
    
    #endregion
    
    #region MiniGamesState

    void StartMiniGame()
    {
        Debug.Log("Enter MiniGames");

        SaveBeforeSceneChange();

        int i = UnityEngine.Random.Range(0, miniGames.Length);

        print(i);

        string sceneName = miniGames[i];
        SceneManager.LoadScene(sceneName);
    }
    
    void UpdateMiniGame()
    {
        if (isAllPlayerReadyToFight())
        {
            stateMachine.ChangeState(GameSate.Combat);
        }
    }
    
    #endregion
    
    #region CombatState

    void CombatEnter()
    {
        VariablesManager.instance.logoState.sprite = VariablesManager.instance.combatSprite;
        
        Debug.Log("Enter Combat");

        foreach (Item item in placedItemsP1)
            if (item != null) item.enabled = true;

        foreach (Item item in placedItemsP2)
            if (item != null) item.enabled = true;

        foreach (PlacementSystem p in VariablesManager.instance.placementSystems)
            p.StartCombat();
        
        UIManager.instance.HideCombatUI(true);
        StartCoroutine(DecompteCombat(1));
    }


    public float CombatDuration = 10;

    IEnumerator DecompteCombat(float intervale)
    {
        yield return new WaitForSeconds(intervale);
        CombatDuration -= intervale;
      
        if (CombatDuration > 0)
        {
            UIManager.instance.UpdateCombatUI((int)CombatDuration);
            StartCoroutine(DecompteCombat(intervale));
        }
        else
        {
            SetAllPlacedItems(false);
            
            yield return new WaitForSeconds(1f);

            CombatDuration = 10;
            
            stateMachine.ChangeState(GameSate.MiniGame);
        }

    }
    
    void CombatUpdate()
    {

    }

    IEnumerator WaitBeforeMiniGame()
    {
        SetAllPlacedItems(false);
        
        yield return new WaitForSeconds(1f);
        stateMachine.ChangeState(GameSate.MiniGame);
    }


    void CombatExit()
    {
        UIManager.instance.HideCombatUI(false);
    }
    
    #endregion
    
    #region PrepareState

    void PrepareEnter()
    {
        VariablesManager.instance.logoState.sprite = VariablesManager.instance.placementSprite;
    
        foreach (PlayerInputing player in VariablesManager.instance.players)
        {
            player.IsReady = false;
        }
    
        StartCoroutine(Pepare());
    }
        
    IEnumerator Pepare()
    {
        yield return new WaitForSeconds(0.5f);        
        Debug.Log("Enter Prepare");
        
        VariablesManager.instance.placementSystems[1].PlaceItem();
        VariablesManager.instance.placementSystems[0].PlaceItem();
        
        SetAllPlacedItems(true);
        
        foreach (Item item in placedItemsP1)
            if (item != null) item.enabled = false;

        foreach (Item item in placedItemsP2)
            if (item != null) item.enabled = false;
    }
    
    void PrepareUpdate()
    {
        if (isAllPlayerReadyToFight())
        {
            stateMachine.ChangeState(GameSate.Combat);
        }
    }
    
    #endregion
    
    #region RewardState

    public Action OnComeBack;

    void RewardEnter()
    {
        Debug.Log("Enter Reward");

        CURRENT_NUMBER_TOUR++;

        if (CURRENT_NUMBER_TOUR >= 20)
        {
            GameOverByEndNbTurn();
            return;
        }
    
        VariablesManager.instance.logoState.sprite = VariablesManager.instance.rewardSprite;

        foreach (PlayerInputing player in VariablesManager.instance.players)
        {
            player.IsReady = false;
        }

        OnComeBack?.Invoke();
    
        CardChoice.instance.ResolveMiniGameResults(player_1_Score, player_2_Score);
    }
    
    void RewardUpdate()
    {
        if (AllPlayerAreChoosed())
        {
            CardChoice.instance.ResetCardSolves();
            stateMachine.ChangeState(GameSate.Prepare);
        }
    }

    bool AllPlayerAreChoosed()
    {
        return !CardChoice.instance.inSelection1 && !CardChoice.instance.inSelection2;
    }
    
    #endregion


    #region SAVE

    public void SaveBeforeSceneChange()
    {
        Debug.Log("Sauvegarde des items avant changement de scène");
        
        placedItemsP1.RemoveAll(item => item == null);
        placedItemsP2.RemoveAll(item => item == null);
        
        UpdateItemDataFromPlacedItems();
        
        SaveSystem.instance.SaveGame();
    }    
    
    private void UpdateItemDataFromPlacedItems()
{
    // VIDER ET RECONSTRUIRE COMPLÈTEMENT
    itemPlacedDataP1.Clear();
    itemPlacedDataP2.Clear();
    
    // Pour P1
    foreach (Item item in placedItemsP1)
    {
        if (item == null) continue;

        ItemData data = new ItemData();
        data.id = item.id;
        data.name = item.name;
        data.PV = item.PV;
        data.maxPV = item.maxPV;
        data.playerOneProperty = item.playerOneProperty;
        data.position = item.transform.position;
        
        // Récupérer prefab et scale depuis la database
        ItemSO database = VariablesManager.instance.placementSystems[0].database;
        var itemInfo = database.itemsData.Find(x => x.Id == item.id);
        if (itemInfo != null)
        {
            data.scale = itemInfo.Size;
            data.prefab = itemInfo.Prefab;
        }
        
        // Sauvegarder alreadyTakeTP pour les troupes
        if (item is Troop troop)
        {
            data.alreadyTakeTP = troop.alreadyTakeTP;
        }

        itemPlacedDataP1.Add(data);
    }

    // Pour P2
    foreach (Item item in placedItemsP2)
    {
        if (item == null) continue;

        ItemData data = new ItemData();
        data.id = item.id;
        data.name = item.name;
        data.PV = item.PV;
        data.maxPV = item.maxPV;
        data.playerOneProperty = item.playerOneProperty;
        data.position = item.transform.position;
        
        // Récupérer prefab et scale depuis la database
        ItemSO database = VariablesManager.instance.placementSystems[1].database;
        var itemInfo = database.itemsData.Find(x => x.Id == item.id);
        if (itemInfo != null)
        {
            data.scale = itemInfo.Size;
            data.prefab = itemInfo.Prefab;
        }
        
        // Sauvegarder alreadyTakeTP pour les troupes
        if (item is Troop troop)
        {
            data.alreadyTakeTP = troop.alreadyTakeTP;
        }

        itemPlacedDataP2.Add(data);
    }
    
    Debug.Log($"Sauvegarde mise à jour - P1: {itemPlacedDataP1.Count} items, P2: {itemPlacedDataP2.Count} items");
}
    
    public void LoadAfterSceneChange()
    {
        StartCoroutine(DelayedRestore());
    }

    private IEnumerator DelayedRestore()
    {
        // attendre la fin de frame pour que tous les PlacementSystem aient exécuté Awake/Start
        yield return new WaitForEndOfFrame();
    
        GameSaveData saveData = SaveSystem.instance.LoadGame();
    
        if (saveData != null)
        {
            Debug.Log("Chargement des items sauvegardés");

            // Restaurer les scores
            player_1_Score = saveData.player1Score;
            player_2_Score = saveData.player2Score;
            CombatDuration = saveData.combatDuration;

            itemPlacedDataP1.Clear();
            itemPlacedDataP2.Clear();

            foreach (ItemData itemData in saveData.itemsP1)
            {
                ItemSO database = VariablesManager.instance.placementSystems[0].database;
                ItemsData itemInfo = database.itemsData.Find(x => x.Id == itemData.id);

                if (itemInfo != null)
                {
                    itemData.prefab = itemInfo.Prefab;
                    itemData.scale = itemInfo.Size;

                    itemPlacedDataP1.Add(itemData);
                }
            }

            foreach (ItemData itemData in saveData.itemsP2)
            {
                ItemSO database = VariablesManager.instance.placementSystems[1].database;
                ItemsData itemInfo = database.itemsData.Find(x => x.Id == itemData.id);

                if (itemInfo != null)
                {
                    itemData.prefab = itemInfo.Prefab;
                    itemData.scale = itemInfo.Size;
                    itemPlacedDataP2.Add(itemData);
                }
            }

            RestorePlacedItems();
        }
        else
        {
            Debug.Log("Aucune sauvegarde à charger");
        }
    }

    public GameObject GetTroopPrefabById(int id, bool isPlayer1)
    {
        if (isPlayer1)
        {
            GameObject prefab = troopPrefabsP1.Find(t => t.GetComponent<Troop>().id == id);
            if (prefab != null)
                return prefab.gameObject;
        }
        else
        {
            GameObject prefab = troopPrefabsP2.Find(t => t.GetComponent<Troop>().id == id);
            if (prefab != null)
                return prefab.gameObject;
        }

        Debug.LogError($"❌ Prefab de troupe introuvable pour l'id : {id}");
        return null;
    }


    private void RestorePlacedItems()
    {
        if (VariablesManager.instance.placementSystems == null || 
            VariablesManager.instance.placementSystems.Length < 2)
        {
            Debug.LogError("PlacementSystems non initialisés");
            return;
        }
        
        placedItemsP1.Clear();
        placedItemsP2.Clear();
        
        foreach (ItemData data in itemPlacedDataP1)
        {
            VariablesManager.instance.placementSystems[0].PlaceStructureAt(data);
        }
        
        foreach (ItemData data in itemPlacedDataP2)
        {
            VariablesManager.instance.placementSystems[1].PlaceStructureAt(data);
        }
        
        Debug.Log($"Items restaurés - P1: {placedItemsP1.Count}, P2: {placedItemsP2.Count}");
    }
    
    public void ReturnToMainScene()
    {
        stateMachine.ChangeState(GameSate.Transition);
        
        print("Retour à la scène principale");
        
        StartCoroutine(LoadMainSceneAndCheck());
    }
    
    private IEnumerator LoadMainSceneAndCheck()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("MainScene");

        while (!op.isDone)
        {
            yield return null;
        }
        
        yield return new WaitForEndOfFrame();
        
        LoadAfterSceneChange();
        
        stateMachine.ChangeState(GameSate.Reward);
    }
    
    public void ResetGame()
    {
        SceneManager.LoadScene("MainScene");
        
        placedItemsP1.Clear();
        placedItemsP2.Clear();
        itemPlacedDataP1.Clear();
        itemPlacedDataP2.Clear();
        
        player_1_Score = -1;
        player_2_Score = -1;
        CombatDuration = 20;
        
        SaveSystem.instance.DeleteSave();
        
        SetAllPlacedItems(false);
        
        Debug.Log("Jeu réinitialisé");
    }

    #endregion
}
