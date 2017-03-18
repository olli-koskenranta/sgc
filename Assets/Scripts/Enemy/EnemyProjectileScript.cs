using UnityEngine;
using System.Collections;

public class EnemyProjectileScript : MonoBehaviour {

    public enum ProjectileType { Bullet, Missile };
    private Camera mainCamera;
    public GameObject hit_effect;
    public GameObject missileTarget;
    public ProjectileType pType = ProjectileType.Bullet;
    public GameObject playerShip;

    public float mass;
    public int damage;
    private float rotateSpeed;
    public float missileSpeed;
    public Transform trans;

    void Start () {
        trans = transform;
        mainCamera = Camera.main;
        gameObject.GetComponent<Rigidbody2D>().mass = mass;
        rotateSpeed = 3f;
        missileSpeed = 5f;
        hit_effect = GameControl.gc.hit_effect;
        if (pType == ProjectileType.Missile)
        {
            playerShip = GameObject.FindWithTag("ShipHull");
        }
    }

    void FixedUpdate()
    {
        //if (!IsOnScreen())
        //    Destroy(gameObject);

        //Destroy if "out of bounds"
        if (trans.position.x > 9 || trans.position.x < -9 || trans.position.y < -5 || trans.position.y > 20)
            Destroy(gameObject);

        if (pType == ProjectileType.Missile)
        {
            Vector3 dir = playerShip.transform.position - trans.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            trans.rotation = Quaternion.Lerp(trans.rotation, q, rotateSpeed * Time.deltaTime);

            GetComponent<Rigidbody2D>().velocity = trans.right * missileSpeed;
        }
    }
	
	void Update () {
        
	}

    private bool IsOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(trans.position);
        if (screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1)
            return true;
        else
            return false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        HitEffect();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        HitEffect();
        Destroy(gameObject);
    }

    private void HitEffect()
    {
        ParticleSystem.MainModule mm;
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, trans.position, Quaternion.identity) as GameObject;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
    }
}
