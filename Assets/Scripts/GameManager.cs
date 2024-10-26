using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; set; }

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform getPlayerInitialPos;
    private Transform playerInitialPos;
    [HideInInspector]public bool playerDeath;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
    [HideInInspector]public GameObject playerClone;


    //Game Stats
    [Header("Game Stats")]
    public GameObject enemyPrefab;
    public Transform getEnemyPos;
    private Transform enemyInitialPos;
    private GameObject enemyClone;

    public GameObject powerUpPrefab;
    public Transform getPowerUpPos;
    private Transform powerUpInitialPos;
    private GameObject powerUpClone;

    internal bool pickedPowerUp;
    internal bool enemyKilled;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerInitialPos = getPlayerInitialPos;
        enemyInitialPos = getEnemyPos;
        powerUpInitialPos = getPowerUpPos;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerRespawn();
    }

    public void PlayerRespawn()
    {
        if(playerDeath)
        {
            playerClone = Instantiate(playerPrefab, playerInitialPos);
            cinemachineCamera.Follow = playerClone.transform;
            playerDeath = false;

            if(enemyKilled)
            {
                enemyClone = Instantiate(enemyPrefab, enemyInitialPos);
                enemyKilled = false;
            }

            if(pickedPowerUp)
            {
                powerUpClone = Instantiate(powerUpPrefab, powerUpInitialPos);
                pickedPowerUp = false;
            }
        }
    }
}
