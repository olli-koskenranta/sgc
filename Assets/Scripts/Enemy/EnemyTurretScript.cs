using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretScript : MonoBehaviour {

    private GameObject Bullet;
    private GameObject playerShip;
    private GameObject hit_effect;
    private float rotateSpeed = 3f;
    private float projectileMass = 1;
    private int damage = 10;
    private float projectile_speed = 2f;
    private float fire_time;
    private float fire_interval = 5f;
    

    void Start ()
    {
        Bullet = Resources.Load("EnemyBullet1") as GameObject;
        playerShip = GameObject.FindWithTag("ShipHull");
        hit_effect = Resources.Load("Explosion") as GameObject;
        fire_time = Time.time;
        
    }
	
	void Update () {
        RotateTowards(playerShip.transform.position);
        if (Time.time - fire_time >= fire_interval && GetComponentInParent<EnemyPlatformScript>().ALIVE)
            Shoot();

        
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, rotateSpeed * Time.deltaTime);
    }

    private void HitEffect()
    {
        ParticleSystem.MainModule mm;
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, transform.position, Quaternion.identity) as GameObject;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    private void Shoot()
    {
        //if (GameControlScript.gameControl.AUDIO_SOUNDS)
        //    laserSound.Play();

        GameObject bulletInstance = Instantiate(Bullet, transform.position, transform.rotation) as GameObject;
        bulletInstance.GetComponent<EnemyProjectileScript>().pType = EnemyProjectileScript.ProjectileType.Bullet;
        bulletInstance.GetComponent<EnemyProjectileScript>().mass = projectileMass;
        bulletInstance.GetComponent<EnemyProjectileScript>().damage = damage;
        bulletInstance.GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector3.right * projectile_speed);
        fire_time = Time.time;
    }

    
}
