using UnityEngine;
using ShipWeapons;
using Asteroids;

using System.Collections;

public class MeteorScript : MonoBehaviour {

    private bool spawnProtection;
    private float spawnTime;
    float speed = 1.5f;
    public int hitPoints;
    public int maxHitPoints;
    public int meteorFragments = 3;
    public GameObject scrapMeteor;
    public GameObject asteroidFragment;
    public GameObject Meteor;
    public AudioSource soundExplode;
    private Camera mainCamera;
    private bool HIT_BY_GRAVITY_DMG = false;
    private bool HIT_BY_KINECTIC_DMG = false;
    public GameObject anomaly1, anomaly2, anomaly5;
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
    private int collisionCounter = 0;
    private int damageStacks = 0;

    public float armor = 0;
    public float baseArmor;
    public bool iscrit = false;

    private GameObject floatingText;

    private float originalPitch;
    public bool tagged = false;

    void Start()
    {

        floatingText = Resources.Load("FloatingText") as GameObject;
        spawnProtection = true;
        spawnTime = Time.time;
        originalPitch = soundExplode.pitch;

        //if (GameControlScript.gameControl.PowerUps[0])
        //    speed *= 0.5f;
        GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector3.left * speed);
        //GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward);
        

        mainCamera = Camera.main;
        if (asteroidType != AsteroidType.Anomaly1)
        {
            anomaly1 = FindAnomaly(1);
            if (anomaly1 != null)
                gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;

            anomaly2 = FindAnomaly(2);
            if (anomaly2 != null)
                gameObject.GetComponent<SpriteRenderer>().color = Color.red;

            anomaly5 = FindAnomaly(5);
            if (anomaly5 != null)
            {
                gameObject.GetComponent<SpriteRenderer>().color = anomaly5.GetComponent<SpriteRenderer>().color;
            }
            
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
                GetComponent<Rigidbody2D>().mass = medMeteorMass * GameControl.gc.currentLevel;
                hitPoints = medMeteorHitPoints * GameControl.gc.currentLevel; // + 100 * GameControlScript.gameControl.currentLevel / 10; ;
                break;
            case AsteroidType.Big:
                XP = 3;
                damage = bigMeteorDamage;
                GetComponent<Rigidbody2D>().mass = bigMeteorMass * GameControl.gc.currentLevel;
                hitPoints = bigMeteorHitPoints * GameControl.gc.currentLevel; // + 300 * GameControlScript.gameControl.currentLevel / 10; ;
                break;
            case AsteroidType.Huge:
                XP = 9;
                damage = hugeMeteorDamage;
                GetComponent<Rigidbody2D>().mass = hugeMeteorMass * GameControl.gc.currentLevel;
                hitPoints = hugeMeteorHitPoints * GameControl.gc.currentLevel; // + 900 * GameControlScript.gameControl.currentLevel / 10; ;
                break;
            default:
                break;
        }

        GetComponent<Rigidbody2D>().AddTorque(GetComponent<Rigidbody2D>().mass / 10, ForceMode2D.Impulse);

        if (HIT_BY_KINECTIC_DMG)
            hitPoints = 1;

        armor = 0.01f * GameControl.gc.currentLevel;
        baseArmor = armor;

        //if (armor > 0.9f)
        //    armor = 0.9f;

        if (GameControl.gc.currentLevel > 20)
            hitPoints *= GameControl.gc.currentLevel / 10;

        if (GameControl.gc.currentLevel > 20)
            GetComponent<Rigidbody2D>().mass *= GameControl.gc.currentLevel / 10;

        maxHitPoints = hitPoints;


    }

    void Update()
    {

        //Destroy if "out of bounds"
        if (gameObject.transform.position.x > 19 || gameObject.transform.position.x < -9 || gameObject.transform.position.y < -7 || gameObject.transform.position.y > 7)
            Destroy(gameObject);

        if (anomaly1 != null)
        {
            if (anomaly1.GetComponent<AnomalyScript>().ALIVE)
            {
                Vector2 forceVector = (anomaly1.transform.position - transform.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 100; // * GameControlScript.gameControl.currentLevel;
                GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
            }
        }

        if (anomaly2 != null)
        {
            if (anomaly2.GetComponent<AnomalyScript>().ALIVE)
            {
                
                Vector2 forceVector = (shipHull.transform.position - transform.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 100; // * GameControlScript.gameControl.currentLevel;
                GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
            }
        }

        if (anomaly5 != null)
        {
            if (anomaly5.GetComponent<AnomalyScript>().ALIVE)
            {
                

                if (anomaly5.GetComponent<AnomalyScript>().phase == 0)
                {
                    armor = baseArmor + 5f;
                    gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                    //GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    //transform.position += (anomaly5.transform.position - transform.position) * Time.deltaTime;
                    Vector2 forceVector = (anomaly5.transform.position - transform.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 10; // * GameControlScript.gameControl.currentLevel;
                    GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
                }
                else if (anomaly5.GetComponent<AnomalyScript>().phase == 1)
                {
                    armor = baseArmor;
                    if (tagged)
                    {
                        
                        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                        //GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                        //transform.position += (shipHull.transform.position - transform.position) * Time.deltaTime;
                        Vector2 forceVector = (shipHull.transform.position - transform.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 10; // * GameControlScript.gameControl.currentLevel;
                        GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    //GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    //transform.position += (anomaly5.transform.position - transform.position) * Time.deltaTime;
                    Vector2 forceVector = (anomaly5.transform.position - transform.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 10; // * GameControlScript.gameControl.currentLevel;
                    GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
                }
            }
        }

        if (Time.time - spawnTime > 1)
        {
            spawnProtection = false;
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        iscrit = false;

       

        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            if (col.gameObject.GetComponent<PlayerProjectileScript>().Critical)
                iscrit = true;
            else
                iscrit = false;

            if (col.gameObject.GetComponent<PlayerProjectileScript>().GravityDamage)
            {
                HIT_BY_GRAVITY_DMG = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                gameObject.GetComponent<Rigidbody2D>().gravityScale += col.gameObject.GetComponent<PlayerProjectileScript>().gravityDmgAmount;
            }
            if (col.gameObject.GetComponent<PlayerProjectileScript>().damageAccumulation > 0)
            {
                damageStacks += 1;
            }

            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage, false, true, col.gameObject.GetComponent<PlayerProjectileScript>().armorPierce);
        }

        else if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            if (!IsOnScreen())
                return;

            if (anomaly1 != null || anomaly5 != null)
            {
                return;

            }
            else
            {
                if (!spawnProtection)
                    isHit(col.gameObject.GetComponent<MeteorScript>().damage, true, false);
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
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        iscrit = false;

        //if (!IsOnScreen())
        //    return;

        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            if (col.gameObject.GetComponent<PlayerProjectileScript>().Critical)
                iscrit = true;
            else
                iscrit = false;

            if (col.gameObject.GetComponent<PlayerProjectileScript>().GravityDamage)
            {
                HIT_BY_GRAVITY_DMG = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                gameObject.GetComponent<Rigidbody2D>().gravityScale += col.gameObject.GetComponent<PlayerProjectileScript>().gravityDmgAmount;
            }

            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage, false, true, col.gameObject.GetComponent<PlayerProjectileScript>().armorPierce);
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

    public void isHit(int incomingDamage, bool ignoreArmor, bool showDmg, float armorPierce = 0f)
    {
        if (damageStacks > 0)
        {
            float nDamage = (float)incomingDamage * ( 1 + damageStacks * GameControl.gc.Weapons[2].DamageAccumulation);
            incomingDamage = (int)nDamage;
        }

        float newDamage = incomingDamage;

        if (!ignoreArmor)
        {
            newDamage -= (armor - armor * armorPierce) * (float)incomingDamage;
            incomingDamage = (int)(newDamage);

            if (incomingDamage <= 1)
                incomingDamage = 1;
        }
        hitPoints -= incomingDamage;
        if (hitPoints <= 0)
        {
            Explode();
        }
        if (showDmg)
            DamageText(iscrit, incomingDamage);
    }

    public void Explode()
    {
        float randomPitch = originalPitch + Random.Range(-0.05f, 0.05f);
        if (PlayerPrefs.GetInt(GameControl.gc.GetSoundKey(), 1) == 1 && !GameControl.gc.GetSceneName().Equals("MainMenu") && IsOnScreen())
        {
            soundExplode.pitch = randomPitch;
            soundExplode.Play();
        }

        GameControl.gc.ExperienceGained(XP);
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
        if (Random.Range(1, 1001) >= 1000 - GameControl.gc.currentLevel / 10)
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

    public GameObject FindAnomaly(int anomalyNumber)
    {
        string aname = "Anomaly" + anomalyNumber.ToString();
        return GameObject.FindWithTag(aname);
    }

    private void DamageText(bool CRITICAL, int dmg)
    {
        GameObject ft;
        ft = Instantiate(floatingText, transform.position, Quaternion.identity) as GameObject;
        ft.GetComponent<FloatingTextScript>().text = dmg.ToString();
        ft.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.PopUp;
        if (CRITICAL)
        {
            ft.GetComponent<TextMesh>().fontSize = 50;
            ft.GetComponent<TextMesh>().color = Color.yellow;
        }
    }
}
