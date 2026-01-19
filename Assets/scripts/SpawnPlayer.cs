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

    private void Start()
    {
        ActivateInfos(0, false);
        ActivateInfos(1, false);
    }
    
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (id >= spawnPoints.Length) return;
        
        playerInput.transform.name = "Player " + id;

        Rect r = id == 0 ? new Rect(0f, 0f, 0.5f, 1f) : new Rect(0.5f, 0f, 0.5f, 1f);
        playerInput.GetComponentInChildren<Camera>().rect = r;

        if (placementSystems.Length > id)
            placementSystems[id].Starting(playerInput.GetComponent<PlayerInputing>());

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
