using UnityEngine;
using ShipWeapons;
using System.Collections;

public class EnemyShipScript : MonoBehaviour {

    public enum ShipType { Fighter, MissileCruiser };
    public ShipType sType = ShipType.Fighter;
    public float projectile_speed;
    public int hitPoints;
    public int damage = 0;
    private GameObject playerShip;
    private float booster_time;
    private float booster_interval;
    private float fire_time;
    public float fire_interval;
    private Camera mainCamera;
    private float speed;
    private Transform[] firingPositions;
    private float rotateSpeed;
    public GameObject hit_effect;

    public GameObject Bullet;
    private bool switchFire;
    public AudioSource laserSound;

    private float projectileMass = 1;

    private bool HIT_BY_GRAVITY_DMG = false;
    private bool spawnProtection = false;

    public int XP = 0;

    private float destroyed_time;
    private float destroyed_interval = 0.3f;

    private int enemyFighterMass = 30;
    private int enemyMissileCruiserMass = 500;
    private int enemyFighterHitPoints = 50;
    private int enemyMissileCruiserHitPoints = 150;
    private int enemyFighterDamage = 1;
    private int enemyMissileCruiserDamage = 2;

    void Start () {
        rotateSpeed = 1f;

        switch (sType)
        {
            case ShipType.Fighter:
                XP = 6;
                projectile_speed = 2f;
                fire_interval = 2f;
                Bullet = Resources.Load("EnemyBullet1") as GameObject;
                GetComponent<Rigidbody2D>().mass = enemyFighterMass * GameControlScript.gameControl.currentLevel;
                damage = enemyFighterDamage * GameControlScript.gameControl.currentLevel;
                hitPoints = enemyFighterHitPoints * GameControlScript.gameControl.currentLevel; // + 200 * GameControlScript.gameControl.currentLevel / 10;
                break;
            case ShipType.MissileCruiser:
                XP = 12;
                transform.localRotation = Quaternion.Euler(0, 0, 180);
                projectile_speed = 0.5f;
                fire_interval = 1f;
                Bullet = Resources.Load("Missile") as GameObject;
                GetComponent<Rigidbody2D>().mass = enemyMissileCruiserMass * GameControlScript.gameControl.currentLevel;
                damage = enemyMissileCruiserDamage * GameControlScript.gameControl.currentLevel;
                hitPoints = enemyMissileCruiserHitPoints * GameControlScript.gameControl.currentLevel; // + 200 * GameControlScript.gameControl.currentLevel / 10;
                break;
            default:
                break;
        }


        
        switchFire = false;
        speed = 1.2f;
        
        booster_interval = 1.5f;
        firingPositions = new Transform[2];
        firingPositions[0] = transform.FindChild("firingPosition1");
        firingPositions[1] = transform.FindChild("firingPosition2");
        playerShip = GameObject.FindWithTag("ShipHull");
        mainCamera = Camera.main;
        booster_time = Time.time;
        fire_time = Time.time;
        hit_effect = Resources.Load("Explosion") as GameObject;
    }
	
	void Update () {

        //Destroy if "out of bounds"
        if (gameObject.transform.position.x > 19 || gameObject.transform.position.x < -10 || gameObject.transform.position.y < -7 || gameObject.transform.position.y > 7)
            Destroy(gameObject);

        if (playerShip != null && sType == ShipType.Fighter)
        {
            if (Time.time - booster_time >= booster_interval && hitPoints > 0)
            {
                if (Vector3.Distance(playerShip.transform.position, transform.position) > 5)
                    GetComponent<Rigidbody2D>().velocity = (playerShip.transform.position - transform.position).normalized * speed;
                else
                    GetComponent<Rigidbody2D>().velocity = -1 * (playerShip.transform.position - transform.position).normalized * speed;
                booster_time = Time.time;
            }
        }

        if (sType == ShipType.MissileCruiser && hitPoints > 0)
        {
            GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector3.right * 0.5f);
        }

        //Rotate towards target
        if (hitPoints > 0)
        {
            if (sType == ShipType.Fighter)
            {
                RotateTowards(playerShip.transform.position);
            }
            else if (sType == ShipType.MissileCruiser)
            {
                Vector3 target = transform.position;
                target.x -= 1;
                RotateTowards(target);
            }
        }

        if (!IsOnScreen())
            return;

        if (Time.time - fire_time >= fire_interval && hitPoints > 0)
            Shoot();

        if (Time.time - destroyed_time >= destroyed_interval && hitPoints <= 0)
        {
            Vector3 rngpos = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0f);

            HitEffect();
            destroyed_time = Time.time;
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //if (!IsOnScreen())
        //    return;

        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
        }

        switch (col.gameObject.tag)
        {
            case "Collector":
                isHit(hitPoints);
                Destroy(gameObject);
                break;

            default:
                break;

        }

        if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            isHit(col.gameObject.GetComponent<MeteorScript>().damage);
        }


    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //if (!IsOnScreen())
        //    return;

        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
        }
    }

    void isHit(int Damage)
    {
        if (hitPoints <= 0)
            return;

        hitPoints -= Damage;
        if (hitPoints <= 0)
        {
            Explode();
            destroyed_time = Time.time;
        }
    }

    void Explode()
    {
        //if (GameControlScript.gameControl.AUDIO_SOUNDS)
        //    soundExplode.Play();

        GameControlScript.gameControl.ExperienceGained(XP);
        //GetComponent<SpriteRenderer>().enabled = false;
        //GetComponent<PolygonCollider2D>().enabled = false;
        //Destroy(this.gameObject, 1);
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        HitEffect();
        HitEffect();
        HitEffect();

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
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, transform.position, Quaternion.identity) as GameObject;
        hiteffect.GetComponent<ParticleSystem>().startColor = Color.red; //gameObject.GetComponent<SpriteRenderer>().color;
    }

    private bool IsOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(gameObject.transform.position);
        if (screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1)
            return true;
        else
            return false;
    }

    private void Shoot()
    {
        if (GameControlScript.gameControl.AUDIO_SOUNDS)
            laserSound.Play();
        int firepos;
        if (!switchFire)
        {
            firepos = 0;
            switchFire = true;
        }
        else
        {
            firepos = 1;
            switchFire = false;
        }

        GameObject bulletInstance = Instantiate(Bullet, firingPositions[firepos].position, firingPositions[firepos].rotation) as GameObject;
        if (sType == ShipType.Fighter)
        {
            bulletInstance.GetComponent<EnemyProjectileScript>().pType = EnemyProjectileScript.ProjectileType.Bullet;
        }
        else if (sType == ShipType.MissileCruiser)
        {
            bulletInstance.GetComponent<EnemyProjectileScript>().pType = EnemyProjectileScript.ProjectileType.Missile;
        }
        bulletInstance.GetComponent<EnemyProjectileScript>().mass = projectileMass;
        bulletInstance.GetComponent<EnemyProjectileScript>().damage = damage;
        bulletInstance.GetComponent<Rigidbody2D>().velocity = firingPositions[firepos].TransformDirection(Vector3.right * projectile_speed);
        fire_time = Time.time;
    }
}
