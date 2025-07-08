using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour {

    private Camera mainCamera;
    private GameObject bullet_shrapnel;
    private GameObject hugeMeteor;
    private GameObject playerTurret;
    private bool PIERCE = false;
    private bool GRAVITYDMG = false;
    private bool SHRAPNEL = false;
    private bool CRITICAL = false;
    private bool SPECIAL = false;
    private bool BOUNCE = false;
    private bool ROTATE = false;
    public int bounces = 0;
    public float damageAccumulation;

    public float critMultiplier;
    public float mass;
    public int damage;
    public float gravityDmgAmount = 0.1f;
    public float armorPierce;



        

    void Start () {
        mainCamera = Camera.main;
        hugeMeteor = GameObject.FindWithTag("hugeMeteor");
        playerTurret = GameObject.FindWithTag("PlayerTurret");
        gameObject.GetComponent<Rigidbody2D>().mass = mass;
        armorPierce = GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 1] * 0.15f;

        if (GameControl.gc.SelectedWeapon == 0)
            BOUNCE = true;



        if (SPECIAL)
        {
            switch (GameControl.gc.SelectedWeapon)
            {
                case 0:
                    
                    GRAVITYDMG = true;
                    
                    break;

                case 1:
                    PIERCE = true;
                    break;

                case 2:
                    SHRAPNEL = true;
                    break;

                case 3:
                    
                    SHRAPNEL = true;
                    break;


                default:
                    break;

            }
        }

        if (CRITICAL)
        {
            float newDamage = (float)damage * critMultiplier;
            damage = (int)newDamage;
        }


    }

    void FixedUpdate()
    {
        //Destroy if "out of bounds"
        if (gameObject.transform.position.x > 9 || gameObject.transform.position.x < -9 || gameObject.transform.position.y < -5 || gameObject.transform.position.y > 20)
            Destroy(gameObject);

        //if (!IsOnScreen())
        //    Destroy(gameObject);

        if (ROTATE)
            GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 2f);

        if (hugeMeteor != null && hugeMeteor.GetComponent<MeteorScript>().IsOnScreen())
        {
            Vector2 forceVector = (hugeMeteor.transform.position - transform.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 10;
            forceVector.x = 0;
            GetComponent<Rigidbody2D>().AddForce(forceVector, ForceMode2D.Impulse);
        }
    }
	

    void OnCollisionEnter2D(Collision2D col)
    {

        HitEffect();
        
        //DamageText();

        if (GameControl.gc.SelectedWeapon == 0)
        {
            bounces--;

            if (bounces < 0)
                BOUNCE = false;
        }

        if (SHRAPNEL)
        {
            ClusterBomb(1, 1);
            ClusterBomb(1, -1);
            ClusterBomb(-1, 1);
            ClusterBomb(-1, -1);
        }

        if (!BOUNCE)
        {
                Destroy(this.gameObject);
        }
        else
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1f;

        if (col.gameObject.tag == "Collector")
            Destroy(this.gameObject);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name.Equals("EnemyShield"))
        {
            damage = 0;
            return;
        }

        HitEffect();

        if (SHRAPNEL)
        {
            ClusterBomb(1, 1);
            ClusterBomb(1, -1);
            ClusterBomb(-1, 1);
            ClusterBomb(-1, -1);
        }

        //DamageText();

        /*if (col.GetComponent<NebulaScript>() != null)
        {
            Destroy(gameObject);
        }*/

        if (PIERCE)
        {
            float damageReduction = (float)damage / 3f;
            damage -= (int)damageReduction;
            //Debug.Log(damage.ToString());
        }
        else
        {
            if (!BOUNCE)
            {
                Destroy(this.gameObject);
            }
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

    private void HitEffect()
    {
        GameObject hiteffect;
        hiteffect = ObjectPool.pool.GetPooledObject(GameControl.gc.hit_effect, 1);
        if (hiteffect == null)
            return;

        ParticleSystem.MainModule mm;
        hiteffect.transform.position = transform.position;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
        hiteffect.SetActive(true);
    }

    private void ClusterBomb(float dirX, float dirY)
    {
        GameObject bulletInstance;
        Transform transform;
        
        bullet_shrapnel = gameObject;
        float speed = playerTurret.GetComponent<TurretScript>().GetTurret().Speed;
        transform = gameObject.transform;

        float angle = Mathf.Atan2(dirY, dirX) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 newPos = transform.position;
        newPos.x -= 0.1f;
        bulletInstance = Instantiate(bullet_shrapnel, newPos, gameObject.transform.rotation) as GameObject;

        bulletInstance.GetComponent<Rigidbody2D>().mass = mass;
        Vector2 newVelocity;
        newVelocity.x = dirX;
        newVelocity.y = dirY;
        bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = newVelocity.normalized * speed;
        bulletInstance.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        //bulletInstance.GetComponent<Transform>().localScale /= 2;
        bulletInstance.GetComponent<PlayerProjectileScript>().damageAccumulation = damageAccumulation;
        bulletInstance.GetComponent<PlayerProjectileScript>().damage = damage / 2;
    }

    public bool Piercing
    {
        get { return PIERCE; }
        set { PIERCE = value; }
    }

    public bool GravityDamage
    {
        get { return GRAVITYDMG; }
        set { GRAVITYDMG = value; }
    }

    public bool Shrapnel
    {
        get { return SHRAPNEL; }
        set { SHRAPNEL = value; }
    }

    public bool Critical
    {
        get { return CRITICAL; }
        set { CRITICAL = value; }
    }

    public bool Special
    {
        get { return SPECIAL; }
        set { SPECIAL = value; }
    }


}
