using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SpawnPlayer : MonoBehaviour
{
    public static SpawnPlayer instance;
    
    public Transform[] spawnPoints;
    private int id = 0;

    [SerializeField] private GameObject[] P1infos;
    [SerializeField] private GameObject[] P2infos;
    [SerializeField] public PlacementSystem[] placementSystems;
    public PlayerInputing[] players;
    public Camera mainCamera;

    private void Awake()
    {
        instance = this;
    }
    
    /*public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (id >= spawnPoints.Length) return;
        
        playerInput.transform.position = spawnPoints[id].position;
        
        playerInput.transform.name = "Player " + id;
        
        if (placementSystems.Length > id)
            placementSystems[id].Starting(playerInput.GetComponent<PlayerInputing>());

        playerInput.GetComponent<PlayerInputing>().isPlayerOne = id == 0;
        
        ActivateInfos(id, false);
        
        GameManager.instance.players.Add(playerInput.GetComponent<PlayerInputing>());
        
        id++;
    }*/
    
}
