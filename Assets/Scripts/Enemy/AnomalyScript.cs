using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyScript : MonoBehaviour {

    //Basic variables
    public int hitPoints;
    public int maxHitPoints;
    private GameObject shipHull;
    public bool ALIVE = false;
    private int XP;
    public Slider BossHPBar;
    private Camera mainCamera;
    public GameObject hit_effect;
    public int anomalyNumber;
    public float armor;
    public bool iscrit = false;
    private GameObject floatingText;
    private float destroyed_time;
    private float destroyed_interval = 0.1f;
    private float damageStacks;


    //Boss spesific variables

    //1
    private GameObject[] medMeteors;
    private GameObject[] bigMeteors;
    private GameObject[] hugeMeteors;
    public int collisionDamage = 100;

    //2
    private Transform standingPosition;

    //3
    private GameObject fighter;
    private Transform spawnPosition;
    private float spawnInterval = 1.5f;
    private float reloadInterval = 10f;
    private float reloadTime;
    private float spawnTime;
    private float rotateSpeed = 0.2f;
    private int fighterCounter = 6;
    private bool startReloading = false;

    //4
    private GameObject shield;
    private GameObject generator;
    private int generatorHitPoints;
    


    void Awake()
    {
        ALIVE = true;
    }

    void Start ()
    {

        XP = 100 * GameControl.gc.currentLevel;
        armor = 0.01f * GameControl.gc.currentLevel;

        MusicScript.music.PlayTrack(3);
        shipHull = GameObject.FindWithTag("ShipHull");
        mainCamera = Camera.main;
        spawnTime = Time.time;
        standingPosition = GameObject.Find("BossStandsHere").GetComponent<Transform>();
        hit_effect = Resources.Load("Explosion") as GameObject;
        floatingText = Resources.Load("FloatingText") as GameObject;
        destroyed_time = Time.time;

        //1
        switch (anomalyNumber)
        {
            case 1:
                
                hitPoints = 1000;
                medMeteors = GameObject.FindGameObjectsWithTag("medMeteor");
                bigMeteors = GameObject.FindGameObjectsWithTag("bigMeteor");
                hugeMeteors = GameObject.FindGameObjectsWithTag("hugeMeteor");

                foreach (GameObject meteor in medMeteors)
                {
                    meteor.GetComponent<MeteorScript>().FindAnomaly(1);
                    meteor.GetComponent<SpriteRenderer>().color = Color.cyan;
                }
                foreach (GameObject meteor in bigMeteors)
                {
                    meteor.GetComponent<MeteorScript>().FindAnomaly(1);
                    meteor.GetComponent<SpriteRenderer>().color = Color.cyan;
                }

                foreach (GameObject meteor in hugeMeteors)
                {
                    meteor.GetComponent<MeteorScript>().FindAnomaly(1);
                    meteor.GetComponent<SpriteRenderer>().color = Color.cyan;
                }
                break;
            case 2:
                hitPoints = 10000;
                break;

            case 3:
                hitPoints = 100000;
                fighter = Resources.Load("AlienShip1") as GameObject;
                spawnPosition = transform.FindChild("SpawnPosition");
                break;
            case 4:
                hitPoints = 100000;
                shield = GameObject.FindWithTag("EShield");
                generator = GameObject.FindWithTag("EGenerator");
                generatorHitPoints = 10;
                break;
        }
        maxHitPoints = hitPoints;

        BossHPBar = GameObject.Find("Canvas/SliderBossHP").GetComponent<Slider>(); 
        BossHPBar.maxValue = maxHitPoints;

        UpdateBossHPBar();
    }
	
	void Update ()
    {
		switch (anomalyNumber)
        {
            case 1:
                GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 0.5f);
                if (shipHull != null && ALIVE)
                {
                    GetComponent<Rigidbody2D>().velocity = (shipHull.transform.position - transform.position).normalized * 0.3f;
                }
                break;

            case 2:
                GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 0.2f);
                if (shipHull != null && ALIVE)
                {
                    GetComponent<Rigidbody2D>().velocity = (standingPosition.position - transform.position).normalized * 0.3f;
                }
                break;

            case 3:
                Vector3 target = transform.position;
                target.x -= 1;
                RotateTowards(target);

                if (shipHull != null && ALIVE)
                {
                    GetComponent<Rigidbody2D>().velocity = (standingPosition.position - transform.position).normalized * 0.3f;
                }

                //if (!IsOnScreen())
                //    return;

                if (!ALIVE)
                    break;

                if (Time.time - spawnTime >= spawnInterval && fighterCounter > 0)
                {
                    SpawnFighter(-1f, Random.Range(-1f, 1f));
                    spawnTime = Time.time;
                }

                if (startReloading && Time.time - reloadTime >= reloadInterval)
                {
                    startReloading = false;
                    fighterCounter = 6;
                }
                break;

            case 4:
                if (GameControl.gc.GAME_PAUSED)
                    return;

                GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * 0.2f);

                if (shipHull != null && ALIVE)
                {
                    GetComponent<Rigidbody2D>().velocity = (standingPosition.position - transform.position).normalized * 0.3f;
                }
                break;
        }

        if (Time.time - destroyed_time >= destroyed_interval && !ALIVE)
        {
            Vector3 rngpos = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0f);

            HitEffect();
            destroyed_time = Time.time;
        }



    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            if (col.gameObject.GetComponent<PlayerProjectileScript>().Critical)
                iscrit = true;
            else
                iscrit = false;

            if (col.gameObject.GetComponent<PlayerProjectileScript>().damageAccumulation > 0)
            {
                damageStacks += 1;
            }

            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage, false, true, col.gameObject.GetComponent<PlayerProjectileScript>().armorPierce);
        }

        if (col.gameObject.GetComponent<ShipHullScript>() != null)
        {
            col.gameObject.GetComponent<ShipHullScript>().isHit(collisionDamage);
        }

        if (col.gameObject.GetComponent<CollectorScript>() != null && !ALIVE)
        {
            Explode();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            if (col.gameObject.GetComponent<PlayerProjectileScript>().Critical)
                iscrit = true;
            else
                iscrit = false;
            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage, false, true, col.gameObject.GetComponent<PlayerProjectileScript>().armorPierce);
        }
    }

    public void OnGeneratorCollisionEnter(Collision2D col)
    {
        if (col.gameObject.tag.Equals("EnemyTurretPlatform"))
        {
            if (generatorHitPoints > 0)
            {
                generatorHitPoints -= 1;
                HitEffect();
                //Debug.Log(generatorHitPoints.ToString());
            }
            else
            {
                Destroy(generator);
                Destroy(shield);
            }

        }
    }

    public void isHit(int incomingDamage, bool ignoreArmor, bool showDmg, float armorPierce = 0f)
    {
        if (!ALIVE)
            return;

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
        

        if (hitPoints - incomingDamage >= 0)
            hitPoints -= incomingDamage;
        else
            hitPoints = 0;

        if (showDmg)
            DamageText(iscrit, incomingDamage);

        UpdateBossHPBar();

        if (hitPoints <= 0)
        {
            MusicScript.music.PlayTrack(2);
            GameControl.gc.ExperienceGained(XP);
            ALIVE = false;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_DESTROYED[anomalyNumber - 1] = true;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_SPAWNED = false;
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(false);
            gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
            gameObject.GetComponent<Rigidbody2D>().mass = 1000000;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.1f;

            if (gameObject.tag.Equals("Anomaly4"))
            {
                RelativeJoint2D[] joints = gameObject.GetComponentsInChildren<RelativeJoint2D>();
                foreach (RelativeJoint2D joint in joints)
                {
                    //Destroy(joint);
                }
            }
            //Destroy(gameObject);
        }
    }

    private void HitEffect()
    {
        Vector3 rngpos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        ParticleSystem.MainModule mm;
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, transform.position + rngpos, Quaternion.identity) as GameObject;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    private void SpawnFighter(float dirX = -1f, float dirY = 0f)
    {
        GameObject fighterInstance;
        fighter.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.Fighter;
        fighterInstance = Instantiate(fighter, spawnPosition.position, spawnPosition.rotation) as GameObject;
        fighterInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(dirX, dirY) * 2;
        fighterCounter--;
        if (fighterCounter == 0)
        {
            startReloading = true;
            reloadTime = Time.time;
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

    private void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, rotateSpeed * Time.deltaTime);
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

    void UpdateBossHPBar()
    {
        BossHPBar.value = hitPoints;
    }

    private void Explode()
    {
        GameObject asteroidFragment = Resources.Load("ScrapPiece") as GameObject;
        int amount = GameControl.gc.currentLevel / 10;

        for (int i = 0; i < amount; i++)
        {
            Vector3 rngpos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            GameObject fragmentInstance = Instantiate(asteroidFragment, this.transform.position + rngpos, this.transform.rotation) as GameObject;
            fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.ResearchMaterial;
        }
        Destroy(gameObject);
    }
}
