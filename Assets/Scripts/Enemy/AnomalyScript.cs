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
    public Transform trans;
    private float hitTime;
    private float critTime;

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
    private int generatorMaxHitPoints;

    //5
    public int phase;
    private float phaseStartTime;
    private float phaseInterval;
    private int consumedObjects = 0;
    private Vector3 preferredScale;
    private int impactCounter = 0;



    void Awake()
    {
        ALIVE = true;
    }

    void Start()
    {
        trans = transform;
        XP = 100 * GameControl.gc.currentLevel;
        armor = 0.01f * GameControl.gc.currentLevel;

        MusicScript.music.PlayTrack(3);
        shipHull = GameObject.FindWithTag("ShipHull");
        mainCamera = Camera.main;
        spawnTime = Time.time;
        standingPosition = GameObject.Find("BossStandsHere").GetComponent<Transform>();
        hit_effect = GameControl.gc.hit_effect;
        floatingText = GameControl.gc.floatingText;
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
                spawnPosition = transform.Find("SpawnPosition");
                break;
            case 4:
                hitPoints = 100000;
                shield = GameObject.FindWithTag("EShield");
                generator = GameObject.FindWithTag("EGenerator");
                generatorHitPoints = 24;
                generatorMaxHitPoints = generatorHitPoints;

                break;
            case 5:
                hitPoints = 1000000;
                phaseStartTime = Time.time;
                phaseInterval = 6;
                GetComponent<SpriteRenderer>().color = Color.cyan;
                preferredScale = trans.localScale;
                break;
        }

        if (GameControl.gc.currentLevel > 50)
        {
            hitPoints += GameControl.gc.currentLevel / 10 * 500000;
        }

        maxHitPoints = hitPoints;

        BossHPBar = GameObject.Find("Canvas/SliderBossHP").GetComponent<Slider>();
        BossHPBar.maxValue = maxHitPoints;

        UpdateBossHPBar();
    }

    void FixedUpdate()
    {
        switch (anomalyNumber)
        {
            case 1:
                GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 0.5f);
                if (shipHull != null && ALIVE)
                {
                    GetComponent<Rigidbody2D>().linearVelocity = (shipHull.transform.position - trans.position).normalized * 0.3f;
                }
                break;

            case 2:
                GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 0.2f);
                if (shipHull != null && ALIVE)
                {
                    GetComponent<Rigidbody2D>().linearVelocity = (standingPosition.position - trans.position).normalized * 0.3f;
                }
                break;

            case 3:
                Vector3 target = trans.position;
                target.x -= 1;
                RotateTowards(target);

                if (shipHull != null && ALIVE)
                {
                    GetComponent<Rigidbody2D>().linearVelocity = (standingPosition.position - trans.position).normalized * 0.3f;
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
                GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * 0.2f);

                if (shipHull != null && ALIVE)
                {
                    GetComponent<Rigidbody2D>().linearVelocity = (standingPosition.position - trans.position).normalized * 0.3f;
                    //Debug.Log("Children: " + GetComponentsInChildren<EnemyPlatformScript>().Length.ToString());
                    if (GetComponentsInChildren<EnemyPlatformScript>().Length == 0)
                    {
                        
                        isHit(maxHitPoints, true, false);
                    }
                }
                break;
            case 5:
                if (shipHull != null && ALIVE)
                {
                    trans.position += (standingPosition.position - trans.position) * Time.deltaTime * 0.1f;
                }
                if (Time.time - phaseStartTime >= phaseInterval && phase != 2)
                {
                    if (phase == 0)
                    {
                        GetComponent<SpriteRenderer>().color = Color.red;
                        phase++;
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().color = Color.cyan;
                        phase--;
                    }
                    phaseStartTime = Time.time;
                }
                if (phase == 2)
                {
                    if (GetComponent<SpriteRenderer>().color != Color.white)
                        GetComponent<SpriteRenderer>().color = Color.white;
                    if (!ALIVE)
                    {
                        Vector3 newScale = trans.localScale;
                        newScale.x -= 0.005f;
                        newScale.y -= 0.005f;
                        trans.localScale = newScale;
                        if (trans.localScale.x < 0.05)
                            Explode();
                    }
                    else if (trans.localScale.x < preferredScale.x)
                    {
                        Vector3 newScale = trans.localScale;
                        newScale.x += 0.002f;
                        newScale.y += 0.002f;
                        trans.localScale = newScale;
                    }
                }
                break;
        }
    }
	
	void Update ()
    {
		

        if (Time.time - destroyed_time >= destroyed_interval && !ALIVE)
        {
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
        if (col.gameObject.GetComponent<MeteorScript>() != null && anomalyNumber == 5)
        {
            if (!col.gameObject.GetComponent<MeteorScript>().tagged)
            {
                col.gameObject.GetComponent<MeteorScript>().tagged = true;
                col.gameObject.GetComponent<Rigidbody2D>().linearVelocity *= 0.1f;
            }
        }
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
    }

    public void OnTriggerStay2D(Collider2D col)
    {
        if (anomalyNumber == 5 && ALIVE && phase == 2 && col.gameObject.GetComponent<MeteorScript>() != null)
        {
            col.attachedRigidbody.linearVelocity = (trans.position - col.transform.position).normalized;
            Vector3 newScale = col.GetComponent<Transform>().localScale;
            newScale.x -= 0.02f;
            newScale.y -= 0.02f;
            col.GetComponent<Transform>().localScale = newScale;
            
            if (col.GetComponent<Transform>().localScale.x < 0.1)
            {
                Consume(col.gameObject, col.gameObject.GetComponent<MeteorScript>().hitPoints);
            }
        }
        if (col.GetComponent<ShipHullScript>() != null && anomalyNumber == 5)
        {
            impactCounter++;
            if (impactCounter >= 5)
            {
                col.GetComponent<ShipHullScript>().isHit(2);
                impactCounter = 0;
            }
        }
    }

    private void Consume(GameObject gameobj, int hp)
    {
        Destroy(gameobj);
        hitPoints += hp;
        preferredScale.x += 0.05f;
        preferredScale.y += 0.05f;
        consumedObjects++;
        UpdateBossHPBar();
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
            float totalArmor = armor - armor * armorPierce;
            if (totalArmor > 0.99f)
                totalArmor = 0.99f;

            newDamage -= newDamage * totalArmor;
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

        if (anomalyNumber == 5)
        {
            if (hitPoints <= (int)((float)maxHitPoints * 0.25f))
            {
                phase = 2;
            }
        }

        UpdateBossHPBar();

        if (hitPoints <= 0)
        {
            MusicScript.music.PlayTrack(2);
            GameControl.gc.ExperienceGained(XP);
            ALIVE = false;
            //GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_SPAWNED = false;
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(false);
            if (anomalyNumber != 5)
            {
                gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                gameObject.GetComponent<Rigidbody2D>().mass = 1000000;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
            }
            if (GameControl.gc.firstBossDefeated == false)
            {
                GameControl.gc.firstBossDefeated = true;
                GameObject.Find("Announcer").GetComponent<AnnouncerScript>().Announce("Boosts in base Unlocked!", FloatingText.FTType.PowerUp);
            }

            
        }
    }

    private void HitEffect()
    {
        GameObject hiteffect;
        hiteffect = ObjectPool.pool.GetPooledObject(GameControl.gc.hit_effect, 1);
        if (hiteffect == null)
            return;

        Vector3 rngpos = trans.position;
        rngpos.x += Random.Range(-1f, 1f);
        rngpos.y += Random.Range(-1f, 1f);
        
        ParticleSystem.MainModule mm;
        hiteffect.transform.position = rngpos;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
        hiteffect.SetActive(true);
    }

    private void SpawnFighter(float dirX = -1f, float dirY = 0f)
    {
        GameObject fighterInstance;
        fighter.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.Fighter;
        fighterInstance = Instantiate(fighter, spawnPosition.position, spawnPosition.rotation) as GameObject;
        Vector2 velocityVector = fighterInstance.GetComponent<Rigidbody2D>().linearVelocity;
        velocityVector.x = dirX * 2;
        velocityVector.y = dirY * 2;
        fighterInstance.GetComponent<Rigidbody2D>().linearVelocity = velocityVector;
        fighterCounter--;
        if (fighterCounter == 0)
        {
            startReloading = true;
            reloadTime = Time.time;
        }
    }

    private bool IsOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(trans.position);
        if (screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1)
            return true;
        else
            return false;
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - trans.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        trans.rotation = Quaternion.Lerp(trans.rotation, q, rotateSpeed * Time.deltaTime);
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
            Vector3 rngpos = trans.position;
            rngpos.x += Random.Range(-1f, 1f);
            rngpos.y += Random.Range(-1f, 1f);
            GameObject fragmentInstance = Instantiate(asteroidFragment, rngpos, trans.rotation) as GameObject;
            fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.ResearchMaterial;
        }
        if (anomalyNumber == 5)
        {
            for (int i = 0; i <= consumedObjects; i++)
            {
                Vector3 rngpos = trans.position;
                rngpos.x += Random.Range(-1f, 1f);
                rngpos.y += Random.Range(-1f, 1f);

                GameObject fragmentInstance = Instantiate(asteroidFragment, rngpos, trans.rotation) as GameObject;
                fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.Normal;
                if (Random.Range(1, 1001) >= 1000 - GameControl.gc.currentLevel / 10)
                {
                    fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.ResearchMaterial;
                }
            }
        }
        Destroy(gameObject);
    }
}
