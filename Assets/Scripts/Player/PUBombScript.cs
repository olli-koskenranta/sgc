using UnityEngine;
using PUBombs;
using System.Collections;

public class PUBombScript : MonoBehaviour {

    private float scaleModifier = 0.05f;
    private float growTime = 0f;
    private float growInterval = 0.1f;
    public PUBombType type = PUBombType.Gravity;
    public Transform trans;

    void Start () {

        trans = transform;
        if (type == PUBombType.Gravity)
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (type == PUBombType.Kinetic)
            GetComponent<SpriteRenderer>().color = Color.white;
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        growTime = Time.time;

        if (PlayerPrefs.GetInt(GameControl.gc.GetSoundKey(), 1) == 1)
            GetComponent<AudioSource>().Play();
	}
	
    void FixedUpdate()
    {
        if (Time.time - growTime >= growInterval)
        {
            Vector3 scaleVector = trans.localScale;
            scaleVector.x += scaleModifier;
            scaleVector.y += scaleModifier;
            trans.localScale = scaleVector;
            Color newColor = gameObject.GetComponent<SpriteRenderer>().color;
            newColor.a -= 0.0075f;
            gameObject.GetComponent<SpriteRenderer>().color = newColor;

        }
        if (trans.localScale.x > 7)
            Destroy(gameObject);
    }

    public void HitEffect(Vector3 hitPosition)
    {
        GameObject hiteffect;
        hiteffect = ObjectPool.pool.GetPooledObject(GameControl.gc.hit_effect, 1);

        if (hiteffect == null)
            return;

        ParticleSystem.MainModule mm;
        hiteffect.transform.position = hitPosition;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
        hiteffect.SetActive(true);
    }
}
