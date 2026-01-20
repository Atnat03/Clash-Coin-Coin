using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SpawnPlayer : MonoBehaviour
{
    public Transform[] spawnPoints;
    private int id = 0;

    [SerializeField] private GameObject[] P1infos;
    [SerializeField] private GameObject[] P2infos;
    [SerializeField] private PlacementSystem[] placementSystems;
    public PlayerInput playerPrefab;
    public PlayerInputManager playerInputManager;
    public Camera mainCamera;
    
    private void Start()
    {
        ActivateInfos(0, true);
        ActivateInfos(1, true);
    }
    
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (id >= spawnPoints.Length) return;
        
        playerInput.transform.position = spawnPoints[id].position;
        
        playerInput.transform.name = "Player " + id;
        
        playerInput.camera = mainCamera;

        if (placementSystems.Length > id)
            placementSystems[id].Starting(playerInput.GetComponent<PlayerInputing>());

        playerInput.GetComponent<PlayerInputing>().isPlayerOne = id == 0;
        
        ActivateInfos(id, false);
        
        GameManager.instance.players.Add(playerInput.GetComponent<PlayerInputing>());
        
        id++;
    }


    public void ActivateInfos(int id, bool state)
    {
        GameObject[] infos = id == 0 ? P1infos : P2infos;
        
        foreach (GameObject info in infos)
        {
            info.SetActive(state);
        }
    }
}
