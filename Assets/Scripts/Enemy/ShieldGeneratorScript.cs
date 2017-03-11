using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGeneratorScript : MonoBehaviour {

    GameObject battlestation;

    void Start()
    {
        battlestation = GameObject.FindWithTag("Anomaly4");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        battlestation.GetComponent<Anomaly4Script>().OnGeneratorCollisionEnter(col);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        battlestation.GetComponent<Anomaly4Script>().OnGeneratorTriggerEnter(col);
    }
}
