using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyWatch : MonoBehaviour
{
    [SerializeField] private float timeBetweenExplosion;
    private float explosionTimer;
    private bool explosion;

    [SerializeField] private TextMeshProUGUI timerText;
    // Start is called before the first frame update
    void Start()
    {
        explosionTimer = timeBetweenExplosion;
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
        
        
    }
}
