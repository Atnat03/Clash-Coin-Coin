using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public List<Troop> spawnedTroopsP1 = new();
    public List<Troop> spawnedTroopsP2 = new();

    public List<TroopData> troopDataP1 = new();
    public List<TroopData> troopDataP2 = new();
    
    public string[] miniGames;
    public int player_1_Score = -1;
    public int player_2_Score = -1;
    
    [Header("Troop Prefabs")]
    public List<GameObject> troopPrefabs;
    
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
    
    void Start()
    {
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
            onEnter:()=> Debug.Log("Enter  EndGame")
        ));
        
        stateMachine.ChangeState(GameSate.Reward);
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
        return true;
    }
    
    #region Function Calling

    void Update()
    {
        stateMachine?.Update();
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

        string sceneName = miniGames[Random.Range(0, miniGames.Length - 1)];
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
        Debug.Log("Enter Combat");

        foreach (Item item in placedItemsP1)
            if (item != null) item.enabled = true;

        foreach (Item item in placedItemsP2)
            if (item != null) item.enabled = true;

        foreach (PlacementSystem p in VariablesManager.instance.placementSystems)
            p.StartCombat();
        
        ActivateAllTroops(true);

        UIManager.instance.HideCombatUI(true);
        StartCoroutine(DecompteCombat(1));
    }
    
    public void ActivateAllTroops(bool state)
    {
        foreach (Troop t in spawnedTroopsP1)
            if (t != null)
            {
                t.enabled = state;
                t.GetComponent<Item>().enabled = state;
            }

        foreach (Troop t in spawnedTroopsP2)
            if (t != null)
            {
                t.enabled = state;
                t.GetComponent<Item>().enabled = state;
            }
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
            ActivateAllTroops(false);
            
            yield return new WaitForSeconds(5f);

            CombatDuration = 20;
            
            stateMachine.ChangeState(GameSate.MiniGame);
        }

    }
    
    void CombatUpdate()
    {
        
    }

    void CombatExit()
    {
        UIManager.instance.HideCombatUI(false);
    }
    
    #endregion
    
    #region PrepareState

    void PrepareEnter()
    {
        Debug.Log("Enter Prepare");
        
        VariablesManager.instance.placementSystems[1].PlaceItem();
        VariablesManager.instance.placementSystems[0].PlaceItem();
        
        SetAllPlacedItems(false);
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

    void RewardEnter()
    {
        Debug.Log("Enter Reward");
        
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
        
        SaveTroops();
        
        UpdateItemDataFromPlacedItems();
        
        SaveSystem.instance.SaveGame();
    }
    
    private void SaveTroops()
    {
        troopDataP1.Clear();
        troopDataP2.Clear();

        foreach (Troop t in spawnedTroopsP1)
        {
            if (t == null) continue;
            troopDataP1.Add(new TroopData
            {
                id = t.id,
                playerOneProperty = true,
                position = t.transform.position,
                rotation = t.transform.rotation,
                PV = t.PV,
                maxPV = t.maxPV
            });
        }

        foreach (Troop t in spawnedTroopsP2)
        {
            if (t == null) continue;
            troopDataP2.Add(new TroopData
            {
                id = t.id,
                playerOneProperty = false,
                position = t.transform.position,
                rotation = t.transform.rotation,
                PV = t.PV,
                maxPV = t.maxPV
            });
        }
    }

    
    private void UpdateItemDataFromPlacedItems()
    {
        // Pour P1
        for (int i = 0; i < placedItemsP1.Count; i++)
        {
            Item item = placedItemsP1[i];
            if (item == null) continue;

            ItemData data;
            if (i < itemPlacedDataP1.Count)
            {
                data = itemPlacedDataP1[i];
            }
            else
            {
                data = new ItemData();
                itemPlacedDataP1.Add(data);
            }

            data.id = item.id;
            data.name = item.name;
            data.PV = item.PV;
            data.maxPV = item.maxPV;
            data.playerOneProperty = item.playerOneProperty;
            data.position = VariablesManager.instance.placementSystems[0].grid.WorldToCell(item.transform.position);
        }

        // Pour P2
        for (int i = 0; i < placedItemsP2.Count; i++)
        {
            Item item = placedItemsP2[i];
            if (item == null) continue;

            ItemData data;
            if (i < itemPlacedDataP2.Count)
            {
                data = itemPlacedDataP2[i];
            }
            else
            {
                data = new ItemData();
                itemPlacedDataP2.Add(data);
            }

            data.id = item.id;
            data.name = item.name;
            data.PV = item.PV;
            data.maxPV = item.maxPV;
            data.playerOneProperty = item.playerOneProperty;
            data.position = VariablesManager.instance.placementSystems[1].grid.WorldToCell(item.transform.position);
        }
    }
    
    public void LoadAfterSceneChange()
    {
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
                var itemInfo = database.itemsData.Find(x => x.Id == itemData.id);
    
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
                var itemInfo = database.itemsData.Find(x => x.Id == itemData.id);
    
                if (itemInfo != null)
                {
                    itemData.prefab = itemInfo.Prefab;
                    itemData.scale = itemInfo.Size;
                    itemPlacedDataP2.Add(itemData);
                }
            }

            RestoreTroops(saveData);
            
            RestorePlacedItems();
        }
        else
        {
            Debug.Log("Aucune sauvegarde à charger");
        }
    }
    
    private void RestoreTroops(GameSaveData saveData)
    {
        spawnedTroopsP1.Clear();
        spawnedTroopsP2.Clear();

        foreach (TroopData data in saveData.troopsP1)
        {
            SpawnTroopFromData(data);
        }

        foreach (TroopData data in saveData.troopsP2)
        {
            SpawnTroopFromData(data);
        }
    }

    private void SpawnTroopFromData(TroopData data)
    {
        GameObject prefab = GetTroopPrefabById(data.id);
        if (prefab == null) return; // sécurité

        GameObject go = Instantiate(prefab, data.position, data.rotation);
        Troop t = go.GetComponent<Troop>();

        t.playerOneProperty = data.playerOneProperty;
        t.PV = data.PV;
        t.maxPV = data.maxPV;

        // Désactiver avant le combat
        t.enabled = false;
        t.GetComponent<Item>().enabled = false;

        if (data.playerOneProperty)
            spawnedTroopsP1.Add(t);
        else
            spawnedTroopsP2.Add(t);
    }
    
    public GameObject GetTroopPrefabById(int id)
    {
        GameObject prefab = troopPrefabs.Find(t => t.GetComponent<Troop>().id == id);
        if (prefab != null)
            return prefab.gameObject;
    
        Debug.LogError($"❌ Prefab de troupe introuvable pour l'id : {id}");
        return null;
    }
    
    public void RegisterTroop(Troop t)
    {
        if (t.playerOneProperty)
            spawnedTroopsP1.Add(t);
        else
            spawnedTroopsP2.Add(t);
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
