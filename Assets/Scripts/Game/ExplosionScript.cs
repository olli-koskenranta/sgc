using UnityEngine;

public class ExplosionScript : MonoBehaviour {

	void Start ()
    {
        Destroy(this.gameObject, GetComponent<ParticleSystem>().main.duration);
	}
	
}
