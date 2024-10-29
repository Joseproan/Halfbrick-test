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

    [Header("Colors")]
    [SerializeField] private Color dangerColor;
    private Color basicColor;

    private Animator anim;
    float secondCounter = 0f;

    [SerializeField]
    private GameObject deathFx;

    private bool death;
    private void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent<Animator>();
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
        basicColor = timerText.color;
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
                if (explosionTimer < 4)
                {
                    timerText.color = dangerColor;
                    secondCounter += Time.deltaTime;

                    if (secondCounter >= 1f) // Cada segundo
                    {
                        secondCounter = 0f; // Reinicia el contador para el siguiente segundo
                        anim.SetTrigger("Warning"); // Activa el trigger de la animación
                    }
                }
                else timerText.color = basicColor;
                

            }
        }


        if ((!death || !_enemyHealth.stunned || !_enemyHealth.pushBack) && playerPos != null) FollowPlayer();


    }

    void FollowPlayer()
    {
        if (agent.enabled)
        {
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

    }

    void Explosion()
    {
        Instantiate(explosionFx, transform.position, Quaternion.identity);
        GameObject explosion = Instantiate(explosionCollider, transform.position, Quaternion.identity);
        Destroy(explosion,explosionColldierDuration);
    }

    public void Death()
    {
        Instantiate(deathFx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
