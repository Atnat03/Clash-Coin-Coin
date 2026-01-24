using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VariablesManager : MonoBehaviour
{
    public static VariablesManager instance;
    
    public Transform[] spawnPoints;
    private int id = 0;

    [SerializeField] private GameObject[] P1infos;
    [SerializeField] private GameObject[] P2infos;
    [SerializeField] public PlacementSystem[] placementSystems;
    public PlayerInputing[] players;
    public Camera mainCamera;
    public Nexus[] nexus;

    public ItemSO duckItemDatabase;
    public ItemSO frogItemDatabase;

    public Sprite rewardSprite;
    public Sprite placementSprite;
    public Sprite combatSprite;
    public Image logoState;

    public TP_Troop[] tps;
    
    private void Awake()
    {
        instance = this;
    }
}
