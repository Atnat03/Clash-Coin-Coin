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

    public string[] miniGames;
    public int player_1_Score = -1;
    public int player_2_Score = -1;
    
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
    
        ItemSO database = itemData.playerOneProperty ? SpawnPlayer.instance.placementSystems[0].database : SpawnPlayer.instance.placementSystems[1].database;
        var itemInfo = database.itemsData.Find(x => x.Id == item.id);
        
        if (itemInfo != null)
        {
            itemData.scale = itemInfo.Size;
            itemData.prefab = itemInfo.Prefab;
        }

        if (itemData.playerOneProperty)
        {
            if (!itemPlacedDataP1.Exists(x => x.prefab == itemData.prefab))
            {
                itemPlacedDataP1.Add(itemData);
            }
        }
        else
        {
            if (!itemPlacedDataP2.Exists(x => x.prefab == itemData.prefab))
            {
                itemPlacedDataP2.Add(itemData);
            }
        }
    }

    public void ReturnToMainScene()
    {
        print("bz ta mere");
        StartCoroutine(LoadMainSceneAndCheck());
    }
    
    private IEnumerator LoadMainSceneAndCheck()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("MainScene");

        while (!op.isDone)
        {
            yield return null;
        }

        if (player_1_Score == -1 && player_2_Score == -1)
        {
            stateMachine.ChangeState(GameSate.StartGame);
        }
        else
        {
            stateMachine.ChangeState(GameSate.Reward);
        }
        
        placedItemsP1.Clear();
        placedItemsP2.Clear();
        
        SpawnPlayer.instance.placementSystems[0].ReloadData(itemPlacedDataP1);
        SpawnPlayer.instance.placementSystems[1].ReloadData(itemPlacedDataP2);
        
        itemPlacedDataP1.Clear();
        itemPlacedDataP2.Clear();

        SetAllPlacedItems(false);
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
        
        stateMachine.ChangeState(GameSate.StartGame);
    }
    
    public void SetAllPlacedItems(bool state)
    {
        foreach (Item item in placedItemsP1)
        {
            item.SetActive(state);
        }
        foreach (Item item in placedItemsP2)
        {
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
        foreach (PlayerInputing player in SpawnPlayer.instance.players)
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
        if (SpawnPlayer.instance.players.Length == 2)
        {
            stateMachine.ChangeState(GameSate.MiniGame);
        }
    }
    
    #endregion
    
    #region MiniGamesState

    void StartMiniGame()
    {
        Debug.Log("Enter MiniGames");
        
        foreach (var placementSystem in SpawnPlayer.instance.placementSystems)
        {
            placementSystem.SaveGrid();
        }

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

        foreach (PlacementSystem p in SpawnPlayer.instance.placementSystems)
        {
            p.StartCombat();
        }

        UIManager.instance.HideCombatUI(true);

        StartCoroutine(DecompteCombat(1));
        
        SetAllPlacedItems(true);
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
        
        SpawnPlayer.instance.placementSystems[1].PlaceItem();
        SpawnPlayer.instance.placementSystems[0].PlaceItem();
        
        SetAllPlacedItems(true);
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
}
