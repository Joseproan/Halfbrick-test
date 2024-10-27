using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] private GameObject powerUpFlashPrefab;

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.Instance.GiveWeapon();
        GameObject powerUpFlash = Instantiate(powerUpFlashPrefab,transform.position, Quaternion.identity);
        Destroy(powerUpFlash,1.5f);
        gameManager.pickedPowerUp = true;
        GameObject.Destroy(gameObject);
    }
}
