using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth;
    private float health;
    [SerializeField] private float inmuneTime = 0.1f;
    internal float invencibleTimer;
    private bool invencibility;
    private Rigidbody2D rb2d;
    
    //Stun Time
    [Header("Stun")]
    internal bool stunned;
    [SerializeField] private float stunTime = 3f;
    private float stunTimer;
    
    
    //Push Force Stats
    internal bool pushBack;
    private float pushForce;
    private Vector3 attackPosition;
    
    [Header("Feedback damage")]
    
    private Color mainColor;
    [SerializeField] Color damagedColor;
    [SerializeField] float hitColorDuration;

    //Components
    private SpriteRenderer playerRenderer;
    private NavMeshAgent agent;
    [SerializeField] private GameObject _weakPoint;
    [SerializeField] private GameObject bulletHitFx;

    private EnemyDamage enemyDamage;
    private bool onlyOnce;
    
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        enemyDamage = GetComponent<EnemyDamage>();
    }

    private void Start()
    {
        health = maxHealth;
        playerRenderer = GetComponent<SpriteRenderer>();
        mainColor = playerRenderer.color;
    }

    void Update()
    {
        Invencibility();
        Stunned();
    }
    private void FixedUpdate()
    {
        /*
        if (pushBack)
        {
            agent.isStopped = true;
            agent.ResetPath();
            Vector3 direction = (this.transform.position - attackPosition).normalized;
            direction.y = 0;
            rb2d.AddForce(direction * pushForce, ForceMode2D.Impulse);
            pushBack = false;
            agent.isStopped = false;
        }*/
    }

    void Invencibility()
    {
        if (invencibleTimer >= 0)
        {
            invencibility = true;
            invencibleTimer -= Time.deltaTime;
        }
        else invencibility = false;
    }
    void Stunned()
    {
        if (stunned && !onlyOnce)
        {
            agent.enabled = false;
            rb2d.gravityScale = 20f;
            _weakPoint.SetActive(true);
            stunTimer = stunTime;
            onlyOnce = true;

        }

        if (stunTimer <= 0)
        {
            enemyDamage.enabled = true;
            _weakPoint.SetActive(false);
            rb2d.gravityScale = 0;
            agent.enabled = true;
            stunned = false;
            onlyOnce = false;
        }
        else stunTimer -= Time.deltaTime;
    }
    public void ReceiveDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            stunned = true;
            enemyDamage.enabled = false;
        }
        invencibleTimer = inmuneTime;
        Instantiate(bulletHitFx, transform.position, Quaternion.identity);
        StartCoroutine(DamagedColor());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet") && !invencibility && !stunned)
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            attackPosition = bullet.owner.transform.position;
            pushBack = true;
            pushForce = bullet.pushForce;

            ReceiveDamage(bullet.damage);
            Destroy(other.gameObject);
        }
    }
    
    public IEnumerator DamagedColor()
    {
        playerRenderer.color = damagedColor;
        yield return new WaitForSeconds(hitColorDuration);
        playerRenderer.color = mainColor;
    }
}
