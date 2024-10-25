using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeakPoint : MonoBehaviour
{
    [HideInInspector] public bool weakPoint;

    public void SetEnemyWeakPoint(bool activate)
    {
        weakPoint = activate;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetEnemyWeakPoint(true);
            Debug.Log(weakPoint);
        }
    }
}
