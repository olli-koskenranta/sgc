using UnityEngine;
using ShipWeapons;
using Asteroids;

using System.Collections;

public class MeteorScript : MonoBehaviour {

    private bool spawnProtection;
    private float spawnTime;
    float speed = 1f;
    public int hitPoints;
    public int meteorFragments = 3;
    public GameObject scrapMeteor;
    public GameObject asteroidFragment;
    public GameObject Meteor;
    public AudioSource soundExplode;
    private Camera mainCamera;
    private bool HIT_BY_GRAVITY_DMG = false;
    private bool HIT_BY_KINECTIC_DMG = false;
    public GameObject anomaly1, anomaly2;
    private GameObject shipHull;
    private int XP = 0;
    public AsteroidType asteroidType = AsteroidType.NONE;
    public int damage;

    private int medMeteorMass = 10;
    private int bigMeteorMass = 30;
    private int hugeMeteorMass = 90;

    private int medMeteorHitPoints = 10;
    private int bigMeteorHitPoints = 30;
    private int hugeMeteorHitPoints = 90;

    private int medMeteorDamage = 2;
    private int bigMeteorDamage = 6;
    private int hugeMeteorDamage = 18;

    void Start()
    {
        spawnProtection = true;
        spawnTime = Time.time;

        //if (GameControlScript.gameControl.PowerUps[0])
        //    speed *= 0.5f;
        GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector3.left * speed);
        GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward);

        mainCamera = Camera.main;
        if (asteroidType != AsteroidType.Anomaly1)
        {
            FindAnomaly(1);
            if (anomaly1 != null)
                gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;

            FindAnomaly(2);
            if (anomaly2 != null)
                gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }

        shipHull = GameObject.FindWithTag("ShipHull");

        switch (asteroidType)
        {
            case AsteroidType.NONE:
                Debug.Log("No meteor type given! Meteor destroyed.");
                Destroy(gameObject);
                break;
            case AsteroidType.Scrap:
                break;
            case AsteroidType.Medium:
                XP = 1;
                damage = medMeteorDamage;
                GetComponent<Rigidbody2D>().mass = medMeteorMass * GameControlScript.gameControl.currentLevel;
                hitPoints = medMeteorHitPoints * GameControlScript.gameControl.currentLevel; // + 100 * GameControlScript.gameControl.currentLevel / 10; ;
                break;
            case AsteroidType.Big:
                XP = 3;
                damage = bigMeteorDamage;
                GetComponent<Rigidbody2D>().mass = bigMeteorMass * GameControlScript.gameControl.currentLevel;
                hitPoints = bigMeteorHitPoints * GameControlScript.gameControl.currentLevel; // + 300 * GameControlScript.gameControl.currentLevel / 10; ;
                break;
            case AsteroidType.Huge:
                XP = 9;
                damage = hugeMeteorDamage;
                GetComponent<Rigidbody2D>().mass = hugeMeteorMass * GameControlScript.gameControl.currentLevel;
                hitPoints = hugeMeteorHitPoints * GameControlScript.gameControl.currentLevel; // + 900 * GameControlScript.gameControl.currentLevel / 10; ;
                break;
            default:
                break;
        }
        

    }

    void Update()
    {

        //Destroy if "out of bounds"
        if (gameObject.transform.position.x > 19 || gameObject.transform.position.x < -9 || gameObject.transform.position.y < -7 || gameObject.transform.position.y > 7)
            Destroy(gameObject);

        //GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward);

        if (anomaly1 != null)
        {
            if (anomaly1.GetComponent<Anomaly1Script>().ALIVE)
            {
                Vector2 forceVector = (anomaly1.transform.position - transform.position).normalized; // * GameControlScript.gameControl.currentLevel;
                GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
            }
                //GetComponent<Rigidbody2D>().velocity = (anomaly1.transform.position - transform.position).normalized;
        }

        if (anomaly2 != null)
        {
            if (anomaly2.GetComponent<Anomaly2Script>().ALIVE)
            {
                Vector2 forceVector = (shipHull.transform.position - transform.position).normalized * 0.5f; // * GameControlScript.gameControl.currentLevel;
                GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
            }
            //GetComponent<Rigidbody2D>().velocity = (anomaly1.transform.position - transform.position).normalized;
        }

        if (Time.time - spawnTime > 1)
        {
            spawnProtection = false;
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsOnScreen())
            return;

        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            if (col.gameObject.GetComponent<PlayerProjectileScript>().GravityDamage)
            {
                HIT_BY_GRAVITY_DMG = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                gameObject.GetComponent<Rigidbody2D>().gravityScale += col.gameObject.GetComponent<PlayerProjectileScript>().gravityDmgAmount;
            }

            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
        }

        else if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            if (anomaly1 != null)
            {
                return;

            }
            else
            {
                if (!spawnProtection)
                    isHit(col.gameObject.GetComponent<MeteorScript>().damage);
            }
        }
        
        else if (col.gameObject.GetComponent<EnemyProjectileScript>() != null)
        {
            Explode();
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!IsOnScreen())
            return;
        if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            if (anomaly1 != null)
            {
                return;
            }
            else
            {
                if (!spawnProtection)
                    isHit(col.gameObject.GetComponent<MeteorScript>().damage);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsOnScreen())
            return;

        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            if (col.gameObject.GetComponent<PlayerProjectileScript>().GravityDamage)
            {
                HIT_BY_GRAVITY_DMG = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                gameObject.GetComponent<Rigidbody2D>().gravityScale += col.gameObject.GetComponent<PlayerProjectileScript>().gravityDmgAmount;
            }

            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
        }

        else if (col.gameObject.GetComponent<PUBombScript>() != null)
        {
            if (col.gameObject.GetComponent<PUBombScript>().type == PUBombs.PUBombType.Gravity)
            {
                HIT_BY_GRAVITY_DMG = true;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                col.gameObject.GetComponent<PUBombScript>().HitEffect(transform.position);
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else if (col.gameObject.GetComponent<PUBombScript>().type == PUBombs.PUBombType.Kinetic)
            {
                HIT_BY_KINECTIC_DMG = true;
                gameObject.GetComponent<Rigidbody2D>().velocity *= 0;
                gameObject.GetComponent<MeteorScript>().hitPoints = 1;
                col.gameObject.GetComponent<PUBombScript>().HitEffect(transform.position);
                gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }
    }

    public void isHit(int Damage)
    {

        hitPoints -= Damage;
        if (hitPoints <= 0)
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (GameControlScript.gameControl.AUDIO_SOUNDS)
            soundExplode.Play();

        GameControlScript.gameControl.ExperienceGained(XP);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<PolygonCollider2D>().enabled = false;
        Destroy(this.gameObject, 1);

        if (asteroidType == AsteroidType.Big || asteroidType == AsteroidType.Huge)
        {
            if (asteroidType == AsteroidType.Big)
                asteroidFragment = Resources.Load("medMeteor1") as GameObject;
            else if (asteroidType == AsteroidType.Huge)
                asteroidFragment = Resources.Load("bigMeteor1") as GameObject;

            GameObject fragment;

            for (int i = 0; i < meteorFragments; i++)
            {
                fragment = Instantiate(asteroidFragment, this.transform.position, this.transform.rotation) as GameObject;
                fragment.GetComponent<Rigidbody2D>().gravityScale = gameObject.GetComponent<Rigidbody2D>().gravityScale;
                if (HIT_BY_KINECTIC_DMG)
                {
                    fragment.GetComponent<SpriteRenderer>().color = Color.gray;
                    fragment.GetComponent<MeteorScript>().hitPoints = 1;
                    fragment.GetComponent<MeteorScript>().HIT_BY_KINECTIC_DMG = true;
                }
                if (HIT_BY_GRAVITY_DMG)
                {
                    fragment.GetComponent<SpriteRenderer>().color = Color.green;
                    fragment.GetComponent<MeteorScript>().HIT_BY_GRAVITY_DMG = true;
                }
            }
        }
        
        
        asteroidFragment = Resources.Load("ScrapPiece") as GameObject;
        GameObject fragmentInstance = Instantiate(asteroidFragment, this.transform.position, this.transform.rotation) as GameObject;
        if (Random.Range(1, 1001) >= 1000 - GameControlScript.gameControl.currentLevel / 10)
        {
            fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.ResearchMaterial;
        }
        
    }

    public bool IsOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(gameObject.transform.position);
        if (screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1)
            return true;
        else
            return false;
    }

    public void FindAnomaly(int anomalyNumber)
    {
        switch (anomalyNumber)
        {
            case 1:
                anomaly1 = GameObject.FindWithTag("Anomaly1");
                break;
            case 2:
                anomaly2 = GameObject.FindWithTag("Anomaly2");
                break;
            default:
                break;
        }
    }
}
