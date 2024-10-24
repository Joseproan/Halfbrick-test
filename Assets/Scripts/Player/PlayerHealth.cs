using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private int maxHealth;
    private int health;
    
    [SerializeField] private float inmuneTime = 0.1f;
    internal float invencibleTimer;
    
    private GameObject lastAttacker;
    internal bool pushBack;
    internal Vector3 attackPosition;
    private float pushForce;
    private Player _player;

    [SerializeField] private CameraShake _cameraShake;
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
    }

    void Start()
    {
        health = maxHealth;
    }
    
    void Update()
    {
        if (invencibleTimer >= 0) invencibleTimer -= Time.deltaTime;
        Die();
    }
    
    private void FixedUpdate()
    {
        if (pushBack)
        {
          
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
    }

    public void ReceiveDamage(int damage)
    {
        health -= damage;
        _cameraShake.ShakeCamera();
        invencibleTimer = inmuneTime;
    }

    public void Die()
    {
        if (health <= 0) SceneManager.LoadScene("Scenes/Game");
        
    }
}
