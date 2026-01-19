using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    
    
    enum GameSate
    {
        StartGame,
        Loading,
        Transition,
        Combat,
        MiniGame,
        EndGame
    }
    
    StateMachine<GameSate> stateMachine =  new StateMachine<GameSate>();
    private State currentState;


    void Awake()
    {
        if(instance == null)instance = this;
        else Destroy(gameObject);
    }
    
    
    void Start()
    {
        stateMachine.Add(new State<GameSate>(
            GameSate.StartGame,
            onEnter:()=> Debug.Log("Enter StartGame")
        ));
        
        stateMachine.Add(new State<GameSate>(
            GameSate.Loading,
            onEnter:()=> Debug.Log("Enter  Loading")
        ));
        
        stateMachine.Add(new State<GameSate>(
            GameSate.Transition,
            onEnter:()=> Debug.Log("Enter  Transition")
        ));
        
        stateMachine.Add(new State<GameSate>(
            GameSate.Combat,
            onEnter:()=> Debug.Log("Enter  Combat"),
            onUpdate: CombatUpdate
        ));
        
        stateMachine.Add(new State<GameSate>(
            GameSate.MiniGame,
            onEnter:()=> Debug.Log("Enter  MiniGame")
        ));
        
        stateMachine.Add(new State<GameSate>(
            GameSate.EndGame,
            onEnter:()=> Debug.Log("Enter  EndGame")
        ));
        
        stateMachine.ChangeState(GameSate.StartGame);
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

    void CombatUpdate()
    {
        
    }
    
    #endregion
}
