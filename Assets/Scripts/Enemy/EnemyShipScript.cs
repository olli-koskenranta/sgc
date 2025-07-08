using UnityEngine;

public class EnemyShipScript : MonoBehaviour {

    public enum ShipType { Fighter, MissileCruiser, BattleShip };
    public ShipType sType = ShipType.Fighter;
    public float projectile_speed;
    public float missile_speed;
    public int hitPoints;
    public int maxHitPoints;
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

    public GameObject Bullet;
    public GameObject Missile;
    private bool switchFire;
    public AudioSource laserSound;

    private float projectileMass = 1;

    private bool HIT_BY_GRAVITY_DMG = false;

    public int XP = 0;

    private float destroyed_time;
    private float destroyed_interval = 0.3f;
    public bool ALIVE;

    private int enemyFighterMass = 30;
    private int enemyMissileCruiserMass = 500;
    private int enemyBattleShipMass = 1000000;

    private int enemyFighterHitPoints = 250;
    private int enemyMissileCruiserHitPoints = 1000;
    private int enemyBattleShipHitPoints = 3000;

    private int enemyFighterDamage = 6;
    private int enemyMissileCruiserDamage = 18;

    public float armor = 0f;
    public bool iscrit = false;
    private float damageStacks;

    public Transform trans;

    private float hitTime;
    private float critTime;

    void Awake()
    {
        trans = transform;
    }
    void Start () {

        rotateSpeed = 1f;
        ALIVE = true;

        switch (sType)
        {
            case ShipType.Fighter:
                XP = 6;
                projectile_speed = 2f;
                fire_interval = 2f;
                Bullet = Resources.Load("EnemyBullet1") as GameObject;
                GetComponent<Rigidbody2D>().mass = enemyFighterMass * GameControl.gc.currentLevel;
                damage = enemyFighterDamage;
                hitPoints = enemyFighterHitPoints * GameControl.gc.currentLevel;
                firingPositions = new Transform[2];
                firingPositions[0] = transform.Find("firingPosition1");
                firingPositions[1] = transform.Find("firingPosition2");
                break;
            case ShipType.MissileCruiser:
                XP = 12;
                transform.localRotation = Quaternion.Euler(0, 0, 180);
                projectile_speed = 0.5f;
                fire_interval = 2f;
                Bullet = Resources.Load("Missile") as GameObject;
                GetComponent<Rigidbody2D>().mass = enemyMissileCruiserMass * GameControl.gc.currentLevel;
                damage = enemyMissileCruiserDamage;
                hitPoints = enemyMissileCruiserHitPoints * GameControl.gc.currentLevel;
                firingPositions = new Transform[2];
                firingPositions[0] = transform.Find("firingPosition1");
                firingPositions[1] = transform.Find("firingPosition2");
                break;
            case ShipType.BattleShip:
                XP = 24;
                projectile_speed = 2f;
                missile_speed = 0.5f;
                fire_interval = 0.5f;
                Bullet = Resources.Load("EnemyBullet1") as GameObject;
                Missile = Resources.Load("Missile") as GameObject;
                hitPoints = enemyBattleShipHitPoints * GameControl.gc.currentLevel;
                GetComponent<Rigidbody2D>().mass = enemyBattleShipMass;
                firingPositions = new Transform[4];
                firingPositions[0] = transform.Find("LeftTurret");
                firingPositions[1] = transform.Find("RightTurret");
                firingPositions[2] = transform.Find("LeftMissileTurret");
                firingPositions[3] = transform.Find("RightMissileTurret");
                break;
            default:
                break;
        }


        
        switchFire = false;
        speed = 1.2f;
        
        booster_interval = 1.5f;
        playerShip = GameObject.FindWithTag("ShipHull");
        mainCamera = Camera.main;
        booster_time = Time.time;
        fire_time = Time.time;

        armor = 0.01f * GameControl.gc.currentLevel;

        //if (armor > 0.9f)
        //    armor = 0.9f;

        if (GameControl.gc.currentLevel > 30)
            hitPoints *= GameControl.gc.currentLevel / 10;

        if (GameControl.gc.currentLevel > 30)
            GetComponent<Rigidbody2D>().mass *= GameControl.gc.currentLevel / 10;

        maxHitPoints = hitPoints;
    }

    void FixedUpdate()
    {
        //Destroy if "out of bounds"
        if (trans.position.x > 19 || trans.position.x < -10 || trans.position.y < -7 || trans.position.y > 7)
            Destroy(gameObject);

        if (playerShip != null && sType == ShipType.Fighter)
        {
            if (Time.time - booster_time >= booster_interval && hitPoints > 0)
            {
                if (Vector3.Distance(playerShip.transform.position, trans.position) > 5)
                    GetComponent<Rigidbody2D>().linearVelocity = (playerShip.transform.position - trans.position).normalized * speed;
                else
                    GetComponent<Rigidbody2D>().linearVelocity = -1 * (playerShip.transform.position - trans.position).normalized * speed;
                booster_time = Time.time;
            }
        }

        if (trans.position.y > 4 && sType == ShipType.MissileCruiser)
        {
            Vector3 newpos = trans.position;
            newpos.y -= 0.01f;
            trans.position = newpos;
        }
            

        if (sType == ShipType.MissileCruiser && hitPoints > 0)
        {
            GetComponent<Rigidbody2D>().linearVelocity = trans.TransformDirection(Vector3.right * 0.5f);
        }

        if (sType == ShipType.BattleShip && hitPoints > 0)
        {
            GetComponent<Rigidbody2D>().linearVelocity = trans.TransformDirection(Vector3.right * 0.2f);
        }

        //Rotate towards target
        if (hitPoints > 0)
        {
            if (sType == ShipType.Fighter)
            {
                if (playerShip != null)
                    RotateTowards(playerShip.transform.position);
            }
            else if (sType == ShipType.MissileCruiser)
            {
                Vector3 target = trans.position;
                target.x -= 1;
                RotateTowards(target);
            }
            else if (sType == ShipType.BattleShip)
            {
                RotateTowards(playerShip.transform.position);
            }
        }
    }

    void Update () {

        

        if (Time.time - fire_time >= fire_interval && hitPoints > 0)
            Shoot();

        if (Time.time - destroyed_time >= destroyed_interval && hitPoints <= 0)
        {
            HitEffect();
            destroyed_time = Time.time;
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

            if (col.gameObject.GetComponent<PlayerProjectileScript>().damageAccumulation > 0)
            {
                damageStacks += 1;
            }

            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage, false, true, col.gameObject.GetComponent<PlayerProjectileScript>().armorPierce);
        }

        if (col.gameObject.GetComponent<CollectorScript>() != null)
        {
            if (sType == ShipType.Fighter)
            {
                GetComponent<Rigidbody2D>().gravityScale = 1f;
            }
            if (!ALIVE)
            {
                
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<CollectorScript>() != null)
        {
            int dmg = maxHitPoints / 100;
            isHit(dmg, true, true);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        iscrit = false;

        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            if (col.gameObject.GetComponent<PlayerProjectileScript>().Critical)
                iscrit = true;
            else
                iscrit = false;
            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage, false, true, col.gameObject.GetComponent<PlayerProjectileScript>().armorPierce);
        }
    }

    void isHit(int incomingDamage, bool ignoreArmor, bool showDmg, float armorPierce = 0f)
    {


        if (hitPoints <= 0)
            hitPoints = 0;

        else
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
                incomingDamage = (int)newDamage;
                if (incomingDamage <= 1)
                    incomingDamage = 1;
            }
            hitPoints -= incomingDamage;

            if (hitPoints <= 0)
            {
                ALIVE = false;
                Explode();
                destroyed_time = Time.time;
            }
        }

        if (showDmg)
            DamageText(iscrit, incomingDamage);
    }

    void Explode()
    {
        //if (GameControlScript.gameControl.AUDIO_SOUNDS)
        //    soundExplode.Play();

        GameControl.gc.ExperienceGained(XP);
        //GetComponent<SpriteRenderer>().enabled = false;
        //GetComponent<PolygonCollider2D>().enabled = false;
        //Destroy(this.gameObject, 1);

        foreach (Collider2D collider in GetComponents<Collider2D>())
            Destroy(collider);

        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;

        gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
        HitEffect();
        HitEffect();
        HitEffect();

        GameObject asteroidFragment = GameControl.gc.scrapPiece;
        GameObject fragmentInstance = Instantiate(asteroidFragment, trans.position, trans.rotation) as GameObject;
        fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.Normal;
        if (Random.Range(1, 1001) >= 1000 - GameControl.gc.currentLevel / 10)
        {
            fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.ResearchMaterial;
        }

    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - trans.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        trans.rotation = Quaternion.Lerp(trans.rotation, q, rotateSpeed * Time.deltaTime);
    }

    private void HitEffect()
    {
        GameObject hiteffect;
        hiteffect = ObjectPool.pool.GetPooledObject(GameControl.gc.hit_effect, 1);
        if (hiteffect == null)
            return;

        Vector3 rngpos = trans.position;
        rngpos.x += Random.Range(-0.1f, 0.1f);
        rngpos.y += Random.Range(-0.1f, 0.1f);

        ParticleSystem.MainModule mm;
        hiteffect.transform.position = rngpos;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
        hiteffect.SetActive(true);
    }

    private bool IsOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(trans.position);
        if (screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1)
            return true;
        else
            return false;
    }

    private void Shoot()
    {
        //if (GameControlScript.gameControl.AUDIO_SOUNDS)
        //    laserSound.Play();
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

        if (sType == ShipType.Fighter || sType == ShipType.MissileCruiser)
        {
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
            bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = firingPositions[firepos].TransformDirection(Vector3.right * projectile_speed);
        }
        else if (sType == ShipType.BattleShip)
        {
            GameObject bulletInstance = Instantiate(Bullet, firingPositions[firepos].position, firingPositions[firepos].rotation) as GameObject;
            bulletInstance.GetComponent<EnemyProjectileScript>().pType = EnemyProjectileScript.ProjectileType.Bullet;
            bulletInstance.GetComponent<EnemyProjectileScript>().mass = projectileMass;
            bulletInstance.GetComponent<EnemyProjectileScript>().damage = enemyFighterDamage;
            bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = firingPositions[firepos].TransformDirection(Vector3.right * projectile_speed);

            GameObject missileInstance = Instantiate(Missile, firingPositions[firepos + 2].position, firingPositions[firepos + 2].rotation) as GameObject;
            bulletInstance.GetComponent<EnemyProjectileScript>().pType = EnemyProjectileScript.ProjectileType.Missile;
            missileInstance.GetComponent<EnemyProjectileScript>().mass = projectileMass;
            missileInstance.GetComponent<EnemyProjectileScript>().damage = enemyMissileCruiserDamage;
            missileInstance.GetComponent<Rigidbody2D>().linearVelocity = firingPositions[firepos].TransformDirection(Vector3.right * missile_speed);

        }
        fire_time = Time.time;
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
