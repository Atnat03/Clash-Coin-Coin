using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
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
    
    public StateMachine<GameSate> stateMachine =  new StateMachine<GameSate>();
    
    public List<Item> placedItems = new List<Item>();

    public string[] miniGames;
    public int player_1_Score = -1;
    public int player_2_Score = -1;
    
    void Awake()
    {
        if(instance == null)instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    public void ReturnToMainScene()
    {
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
    
    void SetAllPlacedItems(bool state)
    {
        foreach (Item item in placedItems)
        {
            item.SetActive(state);
        }
    }
    
    public void RemovePlacedItem(Item item)
    {
        if (placedItems.Contains(item))
        {
            placedItems.Remove(item);
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
        UIManager.instance.HideStartText(true);
        
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

    public float CombatDuration = 20;

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
        
        //SpawnPlayer.instance.placementSystems[1].PlaceItem();
        //SpawnPlayer.instance.placementSystems[0].PlaceItem();
        
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
        UIManager.instance.HideStartText(false);
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
