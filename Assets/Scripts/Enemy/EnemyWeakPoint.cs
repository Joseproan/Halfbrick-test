using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeakPoint : MonoBehaviour
{
    private GameManager gameManager;

    [HideInInspector] public bool weakPoint;
    public GameObject enemy;
    public Animator enemyAnim;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public void SetEnemyWeakPoint(bool activate)
    {
        weakPoint = activate;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemyAnim.SetTrigger("Death");
            //Destroy(enemy.gameObject);
        }
    }
}
