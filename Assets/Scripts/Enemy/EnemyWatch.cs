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
    // Start is called before the first frame update
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

        if (explosionTimer <= 0)
        {
            explosionTimer = timeBetweenExplosion;
            explosion = true;
        }
        else
        {
            explosionTimer -= Time.deltaTime;
        }
        
        FollowPlayer();
    }

    void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos.position);

        // Si la distancia es mayor que el mínimo, el enemigo persigue al jugador
        if (distanceToPlayer > minDistance)
        {
            agent.SetDestination(playerPos.position);
        }
        else
        {
            // Detiene al agente si está dentro de la distancia mínima
            agent.SetDestination(this.transform.position);
        }
    }
}
