﻿using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {

	void Start ()
    {
        Destroy(this.gameObject, GetComponent<ParticleSystem>().main.duration);
	}
	
}
