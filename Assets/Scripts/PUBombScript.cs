using UnityEngine;
using PUBombs;
using System.Collections;

public class PUBombScript : MonoBehaviour {

    private float scaleModifier = 0.05f;
    private float growTime = 0f;
    private float growInterval = 0.1f;
    public PUBombType type = PUBombType.Gravity;
    private GameObject hit_effect;

    void Start () {

        hit_effect = Resources.Load("Explosion") as GameObject;
        if (type == PUBombType.Gravity)
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (type == PUBombType.Kinetic)
            GetComponent<SpriteRenderer>().color = Color.white;
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        growTime = Time.time;

        if (GameControlScript.gameControl.AUDIO_SOUNDS)
            GetComponent<AudioSource>().Play();
	}
	
	void Update () {
        if (Time.time - growTime >= growInterval)
        {
            gameObject.transform.localScale += new Vector3(scaleModifier, scaleModifier, 1);
            Color newColor = gameObject.GetComponent<SpriteRenderer>().color;
            newColor.a -= 0.0075f;
            gameObject.GetComponent<SpriteRenderer>().color = newColor;
            
        }
        if (gameObject.transform.localScale.x > 7)
            Destroy(gameObject);
	
	}

    public void HitEffect(Vector3 hitPosition)
    {
        ParticleSystem.MainModule mm;
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, transform.position, Quaternion.identity) as GameObject;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
    }
}
