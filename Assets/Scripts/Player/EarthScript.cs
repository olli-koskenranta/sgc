using UnityEngine;
using UnityEngine.UI;

public class EarthScript : MonoBehaviour {

    private AdManagerScript adManager;

	void Start ()
    {
        adManager = GetComponent<AdManagerScript>();
        Invoke("EndingReached", 25);
    }
	
	void FixedUpdate ()
    {
        if (transform.position.x < -18)
            Destroy(gameObject);
        Vector3 newpos = transform.position;
        newpos.x -= 0.005f;
        transform.position = newpos; 
    }

    private void EndingReached()
    {
        GameObject.Find("Canvas").transform.Find("EndingPanel").gameObject.SetActive(true);
        GameControl.gc.PauseGame();
    }
}

