using System;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public Transform[] spawnPoints;
    private int id = 0;

    [SerializeField] private GameObject[] P1infos;
    [SerializeField] private GameObject[] P2infos;

    private void Start()
    {
        ActivateInfos(0, true);
        ActivateInfos(1, true);
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        player.transform.position = spawnPoints[id].transform.position;
        ActivateInfos(id, false);
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
