using UnityEngine;
using ShipWeapons;
using System.Collections;

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
    public GameObject hit_effect;

    public GameObject Bullet;
    public GameObject Missile;
    private bool switchFire;
    public AudioSource laserSound;

    private float projectileMass = 1;

    private bool HIT_BY_GRAVITY_DMG = false;
    private bool spawnProtection = false;

    public int XP = 0;

    private float destroyed_time;
    private float destroyed_interval = 0.3f;
    public bool ALIVE;

    private int enemyFighterMass = 30;
    private int enemyMissileCruiserMass = 500;
    private int enemyBattleShipMass = 1000000;

    private int enemyFighterHitPoints = 250;
    private int enemyMissileCruiserHitPoints = 500;
    private int enemyBattleShipHitPoints = 5000;

    private int enemyFighterDamage = 6;
    private int enemyMissileCruiserDamage = 18;

    public float armor = 0f;
    public bool iscrit = false;
    private float damageStacks;

    private GameObject floatingText;

    void Start () {

        floatingText = Resources.Load("FloatingText") as GameObject;
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
                firingPositions[0] = transform.FindChild("firingPosition1");
                firingPositions[1] = transform.FindChild("firingPosition2");
                break;
            case ShipType.MissileCruiser:
                XP = 12;
                transform.localRotation = Quaternion.Euler(0, 0, 180);
                projectile_speed = 0.5f;
                fire_interval = 1f;
                Bullet = Resources.Load("Missile") as GameObject;
                GetComponent<Rigidbody2D>().mass = enemyMissileCruiserMass * GameControl.gc.currentLevel;
                damage = enemyMissileCruiserDamage;
                hitPoints = enemyMissileCruiserHitPoints * GameControl.gc.currentLevel;
                firingPositions = new Transform[2];
                firingPositions[0] = transform.FindChild("firingPosition1");
                firingPositions[1] = transform.FindChild("firingPosition2");
                break;
            case ShipType.BattleShip:
                XP = 24;
                projectile_speed = 2f;
                missile_speed = 0.5f;
                fire_interval = 0.5f;
                Bullet = Resources.Load("EnemyBullet1") as GameObject;
                Missile = Resources.Load("Missile") as GameObject;
                hitPoints = enemyBattleShipHitPoints * GameControl.gc.currentLevel;
                firingPositions = new Transform[4];
                firingPositions[0] = transform.FindChild("LeftTurret");
                firingPositions[1] = transform.FindChild("RightTurret");
                firingPositions[2] = transform.FindChild("LeftMissileTurret");
                firingPositions[3] = transform.FindChild("RightMissileTurret");
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
        hit_effect = Resources.Load("Explosion") as GameObject;

        armor = 0.01f * GameControl.gc.currentLevel;

        //if (armor > 0.9f)
        //    armor = 0.9f;

        if (GameControl.gc.currentLevel > 30)
            hitPoints *= GameControl.gc.currentLevel / 10;

        if (GameControl.gc.currentLevel > 30)
            GetComponent<Rigidbody2D>().mass *= GameControl.gc.currentLevel / 10;

        maxHitPoints = hitPoints;
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

        if (sType == ShipType.BattleShip && hitPoints > 0)
        {
            GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector3.right * 0.2f);
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
            else if (sType == ShipType.BattleShip)
            {
                RotateTowards(playerShip.transform.position);
            }
        }

        //if (!IsOnScreen())
        //    return;

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

        if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            if (!IsOnScreen())
                return;
            int divider;
            if (sType == ShipType.Fighter)
            {
                divider = 20;
                int dmg = maxHitPoints / divider;
                isHit(dmg, true, true);
            }
        }

        if (col.gameObject.GetComponent<CollectorScript>() != null)
        {
            if (!ALIVE)
            {
                GameObject asteroidFragment = Resources.Load("ScrapPiece") as GameObject;
                GameObject fragmentInstance = Instantiate(asteroidFragment, this.transform.position, this.transform.rotation) as GameObject;
                fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.Normal;
                if (Random.Range(1, 1001) >= 1000 - GameControl.gc.currentLevel / 10)
                {
                    fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.ResearchMaterial;
                }
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<CollectorScript>() != null)
        {
            if (!ALIVE)
            {
                Destroy(gameObject);
            }
            else
            {
                int dmg = maxHitPoints / 100;
                isHit(dmg, true, true);
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
                newDamage -= (armor - armor * armorPierce)  * (float)incomingDamage;
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
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        gameObject.GetComponent<Rigidbody2D>().mass = 1000000;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
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
        ParticleSystem.MainModule mm;
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, transform.position, Quaternion.identity) as GameObject;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
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
            bulletInstance.GetComponent<Rigidbody2D>().velocity = firingPositions[firepos].TransformDirection(Vector3.right * projectile_speed);
        }
        else if (sType == ShipType.BattleShip)
        {
            GameObject bulletInstance = Instantiate(Bullet, firingPositions[firepos].position, firingPositions[firepos].rotation) as GameObject;
            bulletInstance.GetComponent<EnemyProjectileScript>().pType = EnemyProjectileScript.ProjectileType.Bullet;
            bulletInstance.GetComponent<EnemyProjectileScript>().mass = projectileMass;
            bulletInstance.GetComponent<EnemyProjectileScript>().damage = enemyFighterDamage;
            bulletInstance.GetComponent<Rigidbody2D>().velocity = firingPositions[firepos].TransformDirection(Vector3.right * projectile_speed);

            GameObject missileInstance = Instantiate(Missile, firingPositions[firepos + 2].position, firingPositions[firepos + 2].rotation) as GameObject;
            bulletInstance.GetComponent<EnemyProjectileScript>().pType = EnemyProjectileScript.ProjectileType.Missile;
            missileInstance.GetComponent<EnemyProjectileScript>().mass = projectileMass;
            missileInstance.GetComponent<EnemyProjectileScript>().damage = enemyMissileCruiserDamage;
            missileInstance.GetComponent<Rigidbody2D>().velocity = firingPositions[firepos].TransformDirection(Vector3.right * missile_speed);

        }
        fire_time = Time.time;
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
