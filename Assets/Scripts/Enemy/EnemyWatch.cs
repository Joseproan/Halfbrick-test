using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWatch : MonoBehaviour
{
    [SerializeField] private float timeBetweenExplosion;
    private float explosionTimer;
    private bool explosion;

    [SerializeField] private TextMeshProUGUI timerText;

    private NavMeshAgent agent;
    private GameObject player;
    private Transform playerPos;
    [SerializeField] private float speed;

    public float minDistance = 2.0f;
    private float distanceToPlayer;
    
    //Explosion
    [SerializeField] private GameObject explosionFx; 
    [SerializeField] private GameObject explosionCollider; 
    [SerializeField] private float explosionColldierDuration;

    private EnemyHealth _enemyHealth;

    private void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    void Start()
    {
        explosionTimer = timeBetweenExplosion;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        playerPos = player.transform;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = Mathf.CeilToInt(explosionTimer).ToString(); // Redondea hacia arriba

        if (!_enemyHealth.stunned)
        {
            if (explosionTimer <= 0)
            {
                explosionTimer = timeBetweenExplosion;
                Explosion();
                explosion = true;
            }
            else
            {
                explosionTimer -= Time.deltaTime;
            }
        }


        if (!_enemyHealth.stunned && playerPos != null) FollowPlayer();


    }

    void FollowPlayer()
    {
        if (agent.enabled == false) agent.enabled = true;
        if (playerPos != null) distanceToPlayer = Vector3.Distance(transform.position, playerPos.position);
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerPos = player.transform;
        }
        // Si la distancia es mayor que el m�nimo, el enemigo persigue al jugador
        if (distanceToPlayer > minDistance)
        {
            agent.isStopped = false; 
            agent.SetDestination(playerPos.position);
        }
        else
        {
            // Detiene al agente si est� dentro de la distancia m�nima
            agent.isStopped = true; 
            
        }
    }

    void Explosion()
    {
        Instantiate(explosionFx, transform.position, Quaternion.identity);
        GameObject explosion = Instantiate(explosionCollider, transform.position, Quaternion.identity);
        Destroy(explosion,explosionColldierDuration);
    }
}
