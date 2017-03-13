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
        battlestation.GetComponent<AnomalyScript>().OnGeneratorCollisionEnter(col);
    }
}
