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

    void Start () {
        mainCamera = Camera.main;
        gameObject.GetComponent<Rigidbody2D>().mass = mass;
        rotateSpeed = 3f;
        missileSpeed = 5f;
        hit_effect = Resources.Load("Explosion") as GameObject;
        if (pType == ProjectileType.Missile)
        {
            playerShip = GameObject.FindWithTag("ShipHull");
        }
    }
	
	void Update () {
        //if (!IsOnScreen())
        //    Destroy(gameObject);

        //Destroy if "out of bounds"
        if (gameObject.transform.position.x > 9 || gameObject.transform.position.x < -9 || gameObject.transform.position.y < -5 || gameObject.transform.position.y > 20)
            Destroy(gameObject);

        if (pType == ProjectileType.Missile)
        {
            Vector3 dir = playerShip.transform.position - transform.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, rotateSpeed * Time.deltaTime);

            GetComponent<Rigidbody2D>().velocity = transform.right * missileSpeed;
        }
	}

    private bool IsOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(gameObject.transform.position);
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
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, transform.position, Quaternion.identity) as GameObject;
        hiteffect.GetComponent<ParticleSystem>().startColor = gameObject.GetComponent<SpriteRenderer>().color;
    }
}
