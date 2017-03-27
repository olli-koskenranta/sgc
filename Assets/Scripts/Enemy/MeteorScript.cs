using UnityEngine;
using Asteroids;

public class MeteorScript : MonoBehaviour {

    private float spawnTime;
    float speed = 1.5f;
    public int hitPoints;
    public int maxHitPoints;
    public int meteorFragments = 3;
    public GameObject scrapFragment;
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

    private float originalPitch;
    public bool tagged = false;
    public Transform trans;

    private float hitTime;
    private float critTime;

    void Start()
    {
        trans = transform;
        spawnTime = Time.time;
        originalPitch = soundExplode.pitch;

        GetComponent<Rigidbody2D>().velocity = trans.TransformDirection(Vector3.left * speed);
        

        mainCamera = Camera.main;

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

        shipHull = GameObject.FindWithTag("ShipHull");

        switch (asteroidType)
        {
            case AsteroidType.NONE:
                Debug.Log("No meteor type given! Meteor destroyed.");
                Destroy(gameObject);
                break;
            case AsteroidType.Medium:
                XP = 1;
                damage = medMeteorDamage;
                GetComponent<Rigidbody2D>().mass = medMeteorMass * GameControl.gc.currentLevel;
                hitPoints = medMeteorHitPoints * GameControl.gc.currentLevel; // + 100 * GameControlScript.gameControl.currentLevel / 10;
                break;
            case AsteroidType.Big:
                XP = 3;
                damage = bigMeteorDamage;
                GetComponent<Rigidbody2D>().mass = bigMeteorMass * GameControl.gc.currentLevel;
                hitPoints = bigMeteorHitPoints * GameControl.gc.currentLevel; // + 300 * GameControlScript.gameControl.currentLevel / 10; ;
                asteroidFragment = GameControl.gc.meteors[0];
                asteroidFragment.GetComponent<MeteorScript>().asteroidType = AsteroidType.Medium;
                break;
            case AsteroidType.Huge:
                XP = 9;
                damage = hugeMeteorDamage;
                GetComponent<Rigidbody2D>().mass = hugeMeteorMass * GameControl.gc.currentLevel;
                hitPoints = hugeMeteorHitPoints * GameControl.gc.currentLevel; // + 900 * GameControlScript.gameControl.currentLevel / 10; ;
                asteroidFragment = GameControl.gc.meteors[1];
                asteroidFragment.GetComponent<MeteorScript>().asteroidType = AsteroidType.Big;
                break;
            case AsteroidType.Golden:
                XP = 20;
                damage = hugeMeteorDamage;
                GetComponent<Rigidbody2D>().mass = hugeMeteorMass * 10 * GameControl.gc.currentLevel;
                hitPoints = medMeteorHitPoints * GameControl.gc.currentLevel;
                GetComponent<Rigidbody2D>().velocity = trans.TransformDirection(Vector3.left * speed * 2);
                break;
            default:
                break;
        }

        scrapFragment = GameControl.gc.scrapPiece;

        GetComponent<Rigidbody2D>().AddTorque(GetComponent<Rigidbody2D>().mass / 10, ForceMode2D.Impulse);

        if (HIT_BY_KINECTIC_DMG)
        {
            hitPoints = 1;
            GetComponent<Rigidbody2D>().mass = 1;
        }

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

    void FixedUpdate()
    {
        //Destroy if "out of bounds"
        if (trans.position.x > 19 || trans.position.x < -9 || trans.position.y < -7 || trans.position.y > 7)
            Destroy(gameObject);

        if (anomaly1 != null)
        {
            if (anomaly1.GetComponent<AnomalyScript>().ALIVE)
            {
                Vector2 forceVector = (anomaly1.transform.position - trans.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 10; // * GameControlScript.gameControl.currentLevel;
                GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
            }
        }

        if (anomaly2 != null)
        {
            if (anomaly2.GetComponent<AnomalyScript>().ALIVE)
            {

                Vector2 forceVector = (shipHull.transform.position - trans.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 100; // * GameControlScript.gameControl.currentLevel;
                GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
            }
        }

        if (anomaly5 != null)
        {
            if (anomaly5.GetComponent<AnomalyScript>().ALIVE)
            {


                if (anomaly5.GetComponent<AnomalyScript>().phase == 0)
                {
                    //armor = baseArmor + 1f;
                    gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                    Vector2 forceVector = (anomaly5.transform.position - trans.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 10; // * GameControlScript.gameControl.currentLevel;
                    GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
                }
                else if (anomaly5.GetComponent<AnomalyScript>().phase == 1)
                {
                    //armor = baseArmor;
                    if (tagged)
                    {

                        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                        Vector2 forceVector = (shipHull.transform.position - trans.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 10; // * GameControlScript.gameControl.currentLevel;
                        GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    Vector2 forceVector = (anomaly5.transform.position - trans.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 10; // * GameControlScript.gameControl.currentLevel;
                    GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
                }
            }
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
                isHit(col.gameObject.GetComponent<MeteorScript>().damage, true, false);
            }
        }
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

            if (col.gameObject.GetComponent<PlayerProjectileScript>().GravityDamage && asteroidType != AsteroidType.Golden)
            {
                HIT_BY_GRAVITY_DMG = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                gameObject.GetComponent<Rigidbody2D>().gravityScale += col.gameObject.GetComponent<PlayerProjectileScript>().gravityDmgAmount;
            }

            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage, false, true, col.gameObject.GetComponent<PlayerProjectileScript>().armorPierce);
        }

        else if (col.gameObject.GetComponent<PUBombScript>() != null)
        {
            if (col.gameObject.GetComponent<PUBombScript>().type == PUBombs.PUBombType.Gravity && asteroidType != AsteroidType.Golden)
            {
                HIT_BY_GRAVITY_DMG = true;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                col.gameObject.GetComponent<PUBombScript>().HitEffect(trans.position);
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else if (col.gameObject.GetComponent<PUBombScript>().type == PUBombs.PUBombType.Kinetic && asteroidType != AsteroidType.Golden)
            {
                HIT_BY_KINECTIC_DMG = true;
                gameObject.GetComponent<Rigidbody2D>().velocity *= 0;
                gameObject.GetComponent<Rigidbody2D>().mass = 1;
                gameObject.GetComponent<MeteorScript>().hitPoints = 1;
                col.gameObject.GetComponent<PUBombScript>().HitEffect(trans.position);
                gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }

        else if (col.gameObject.GetComponent<AnomalyScript>() != null)
        {
            if (col.gameObject.GetComponent<AnomalyScript>().anomalyNumber == 5)
            {
                armor = baseArmor + 0.1f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if ((col.gameObject.GetComponent<AnomalyScript>() != null))
        {
            if (col.gameObject.GetComponent<AnomalyScript>().anomalyNumber == 5)
            {
                armor = baseArmor;
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
            float totalArmor = armor - armor * armorPierce;
            if (totalArmor > 0.99f)
                totalArmor = 0.99f;

            newDamage -= newDamage * totalArmor;
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
        Destroy(GetComponent<Collider2D>());
        

        if (asteroidType == AsteroidType.Big || asteroidType == AsteroidType.Huge)
        {
            GameObject fragment;

            for (int i = 0; i < meteorFragments; i++)
            {
                fragment = Instantiate(asteroidFragment, trans.position, trans.rotation) as GameObject;
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
        
        
        
        GameObject fragmentInstance = Instantiate(scrapFragment, trans.position, trans.rotation) as GameObject;
        if (Random.Range(1, 1001) >= 1000 - GameControl.gc.currentLevel / 10)
        {
            fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.ResearchMaterial;
        }

        if (asteroidType == AsteroidType.Golden)
        {
            for (int i = 0; i <= 10; i++)
            {
                Vector3 rngpos = trans.position;
                rngpos.x += Random.Range(-0.5f, 0.5f);
                rngpos.y += Random.Range(-0.5f, 0.5f);
                fragmentInstance = Instantiate(scrapFragment, rngpos, trans.rotation) as GameObject;
                fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.Normal;
            }
        }

        Destroy(gameObject, 1);

    }

    public bool IsOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(trans.position);
        if (screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1)
            return true;
        else
            return false;
    }

    public GameObject FindAnomaly(int anomalyNumber)
    {
        string aname = "Anomaly" + anomalyNumber.ToString();
        if (anomalyNumber == 1)
        {
            if (GameObject.FindWithTag(aname) != null)
                anomaly1 = GameObject.FindWithTag(aname);
        }

        return GameObject.FindWithTag(aname);
    }

    private void DamageText(bool CRITICAL, int dmg)
    {
        if (!CRITICAL)
        {
            if (Time.time - hitTime < 0.5f)
                return;
            GameObject normalDamageTextInstance;
            normalDamageTextInstance = ObjectPool.pool.GetPooledObject(GameControl.gc.floatingText, 1);

            if (normalDamageTextInstance == null)
                return;

            normalDamageTextInstance.transform.position = trans.position;
            normalDamageTextInstance.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.PopUp;
            normalDamageTextInstance.GetComponent<FloatingTextScript>().text = dmg.ToString();
            normalDamageTextInstance.SetActive(true);
            hitTime = Time.time;
        }

        else 
        {
            if (Time.time - critTime < 0.5f)
                return;
            GameObject criticalDamageTextInstance;
            criticalDamageTextInstance = ObjectPool.pool.GetPooledObject(GameControl.gc.floatingText, 1);

            if (criticalDamageTextInstance == null)
                return;

            criticalDamageTextInstance.transform.position = trans.position;
            criticalDamageTextInstance.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.PopUp;
            criticalDamageTextInstance.GetComponent<FloatingTextScript>().text = dmg.ToString();
            criticalDamageTextInstance.GetComponent<FloatingTextScript>().isCrit = true;
            criticalDamageTextInstance.GetComponent<TextMesh>().color = Color.yellow;
            criticalDamageTextInstance.SetActive(true);
            critTime = Time.time;
        }
    }
}
