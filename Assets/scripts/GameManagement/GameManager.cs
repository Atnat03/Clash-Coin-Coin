using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    enum GameSate
    {
        StartGame,
        Loading,
        Transition,
        Prepare,
        Combat,
        MiniGame,
        EndGame
    }
    
    StateMachine<GameSate> stateMachine =  new StateMachine<GameSate>();
    public State currentState;
    
    public List<Item> placedItems = new List<Item>();
    public List<PlacementSystem> placementSystems = new List<PlacementSystem>();
    public List<PlayerInputing> players =  new List<PlayerInputing>();
    
    void Awake()
    {
        if(instance == null)instance = this;
        else Destroy(gameObject);
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
            onUpdate: CombatUpdate
        ));
        
        //MiniGame
        stateMachine.Add(new State<GameSate>(
            GameSate.MiniGame,
            onEnter:()=> Debug.Log("Enter  MiniGame")
        ));
        
        //EndGame
        stateMachine.Add(new State<GameSate>(
            GameSate.EndGame,
            onEnter:()=> Debug.Log("Enter  EndGame")
        ));
        
        stateMachine.ChangeState(GameSate.StartGame);
    }
    
    void SetAllPlacedItems(bool state)
    {
        foreach (Item item in placedItems)
        {
            item.enabled = state;
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
        foreach (PlayerInputing player in players)
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
    
    #region CombatState

    void StartEnter()
    {
        Debug.Log("Enter Start");
    }
    
    void StartUpdate()
    {
        if (players.Count == 2)
        {
            stateMachine.ChangeState(GameSate.Prepare);
        }
    }
    
    #endregion
    
    
    #region CombatState

    void CombatEnter()
    {
        Debug.Log("Enter Combat");

        foreach (PlacementSystem p in placementSystems)
        {
            p.StartCombat();
        }
        
        SetAllPlacedItems(true);
    }
    
    void CombatUpdate()
    {
        
    }
    
    #endregion
    
    #region PrepareState

    void PrepareEnter()
    {
        Debug.Log("Enter Prepare");

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
}
