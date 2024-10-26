using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.Instance.GiveWeapon();
        gameManager.pickedPowerUp = true;
        GameObject.Destroy(gameObject);
    }
}
