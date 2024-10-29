using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{

    public int damage;
    public float pushForce;
    
    [HideInInspector] public GameObject owner;
    
    public void SetPushForce(float s)
    {
        pushForce = s;
    }

    void Start()
    {
        owner = this.gameObject;
    }
 
}
