using UnityEngine;

public class ExplosionScript : MonoBehaviour {

    public bool KILL_ME = false;
	void OnEnable ()
    {
        GetComponent<ParticleSystem>().Play();
        Invoke("EndLife", GetComponent<ParticleSystem>().main.duration);
	}

    private void EndLife()
    {
        if (KILL_ME)
            Destroy(gameObject);
        gameObject.SetActive(false);
    }
	
}
