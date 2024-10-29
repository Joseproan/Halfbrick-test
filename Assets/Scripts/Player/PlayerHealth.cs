using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health stats")]
    private Rigidbody2D rb;
    [SerializeField] private int maxHealth;
    private int health;
    
    [SerializeField] private float inmuneTime = 0.1f;
    internal float invencibleTimer;

    [Header("Attacker stats")]
    private GameObject lastAttacker;
    internal bool pushBack;
    internal Vector3 attackPosition;
    private float pushForce;
    private Player _player;

    [Header("Feedback damage")]
    private SpriteRenderer playerRenderer;
    [SerializeField] private Image innerSliderColor;
    private Color mainColor;
    [SerializeField] Color damagedColor;
    [SerializeField] float hitColorDuration;

    [SerializeField] private GameObject deathEffect;

    private GameManager gameManager;
    public EnemyHealth enemyHealth;
    [SerializeField] private Slider healthSlider;
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
        playerRenderer = GetComponent<SpriteRenderer>();
        mainColor = playerRenderer.color;
    }

    void Start()
    {
        health = maxHealth;
        gameManager = GameManager.instance;
    }
    
    void Update()
    {
        if (invencibleTimer >= 0) invencibleTimer -= Time.deltaTime;
        Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy") && invencibleTimer <= 0 && !enemyHealth.stunned)
        {
            EnemyDamage enemyDamage = collision.gameObject.GetComponent<EnemyDamage>();
            lastAttacker = enemyDamage.owner;
            attackPosition = enemyDamage.owner.transform.position;
            pushBack = true;
            pushForce = enemyDamage.pushForce;
            ReceiveDamage(enemyDamage.damage);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Spikes") && invencibleTimer <= 0  )
        {
            Spikes spikes = other.gameObject.GetComponent<Spikes>();
            lastAttacker = spikes.owner;
            attackPosition = spikes.owner.transform.position;
            pushBack = true;
            pushForce = spikes.pushForce;
            ReceiveDamage(spikes.damage);
        }

        if (other.transform.CompareTag("Explosion"))
        {
            Spikes spikes = other.gameObject.GetComponent<Spikes>();
            lastAttacker = spikes.owner;
            attackPosition = spikes.owner.transform.position;
            pushBack = true;
            pushForce = spikes.pushForce;
            ReceiveDamage(spikes.damage);
        }
    }

    public void ReceiveDamage(int damage)
    {
        health -= damage;
        healthSlider.value = health;
        StartCoroutine(DamagedColor());
        invencibleTimer = inmuneTime;
    }

    public void Die()
    {
        if (health <= 0)
        {
            Instantiate(deathEffect,transform.position, Quaternion.identity);
            gameManager.playerDeath = true;
            Destroy(this.gameObject);
        }
    }

    public IEnumerator DamagedColor()
    {
        innerSliderColor.color = damagedColor;
        playerRenderer.color = damagedColor;
        yield return new WaitForSeconds(hitColorDuration);
        innerSliderColor.color = mainColor;
        playerRenderer.color = mainColor;
    }
}
