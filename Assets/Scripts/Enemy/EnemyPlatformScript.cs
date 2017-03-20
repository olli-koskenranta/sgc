using UnityEngine;

public class EnemyPlatformScript : MonoBehaviour {

    private float gravityHitTime;
    private float gravityHitResetTime = 4f;
    private int hitPoints;

    private float destroyed_time;
    private float destroyed_interval = 0.3f;
    public bool ALIVE;
    public int XP = 0;
    private GameObject hit_effect;
    private GameObject floatingText;
    private bool iscrit = false;
    private int damageStacks;
    public Transform trans;

    GameObject battlestation;

    private float hitTime;
    private float critTime;


    void Start ()
    {
        battlestation = GameObject.FindWithTag("Anomaly4");
        floatingText = GameControl.gc.floatingText;
        trans = transform;
        ALIVE = true;
        hitPoints = battlestation.GetComponent<AnomalyScript>().maxHitPoints / 8;
        gravityHitTime = Time.time;
        hit_effect = GameControl.gc.hit_effect;
    }
	
    void FixedUpdate()
    {
        //Destroy if "out of bounds"
        if (trans.position.x > 19 || trans.position.x < -9 || trans.position.y < -7 || trans.position.y > 7)
            Destroy(gameObject);

        if (Time.time - gravityHitTime >= gravityHitResetTime && ALIVE)
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }

	void Update ()
    {
        if (Time.time - destroyed_time >= destroyed_interval && hitPoints <= 0)
        {
            HitEffect();
            destroyed_time = Time.time;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            if (col.gameObject.GetComponent<PlayerProjectileScript>().GravityDamage)
            {
                gameObject.GetComponent<Rigidbody2D>().gravityScale = col.gameObject.GetComponent<PlayerProjectileScript>().gravityDmgAmount * 11;
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                gravityHitTime = Time.time;
            }

            if (col.gameObject.GetComponent<PlayerProjectileScript>().Critical)
                iscrit = true;
            else
                iscrit = false;

            if (col.gameObject.GetComponent<PlayerProjectileScript>().damageAccumulation > 0)
            {
                damageStacks += 1;
            }

            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
        }

        if (col.gameObject.GetComponent<CollectorScript>() != null)
        {
            if (!ALIVE)
                Destroy(gameObject);
            else
                isHit(col.gameObject.GetComponent<CollectorScript>().grinderDamage);
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


            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
        }
    }

    void isHit(int Damage)
    {
        if (hitPoints <= 0)
            return;

        if (damageStacks > 0)
        {
            float nDamage = (float)Damage * ( 1 + damageStacks * GameControl.gc.Weapons[2].DamageAccumulation);
            Damage = (int)nDamage;
        }

        
        hitPoints -= Damage;
        if (hitPoints >= 0)
            battlestation.GetComponent<AnomalyScript>().isHit(Damage, true, false);
        else
        {
            Damage += hitPoints;
            battlestation.GetComponent<AnomalyScript>().isHit(Damage, true, false);
        }
        if (hitPoints <= 0)
        {
            
            ALIVE = false;
            Explode();
            destroyed_time = Time.time;
        }
        DamageText(iscrit, Damage);
    }

    void Explode()
    {
        //if (GameControlScript.gameControl.AUDIO_SOUNDS)
        //    soundExplode.Play();

        GameControl.gc.ExperienceGained(XP);
        //GetComponent<SpriteRenderer>().enabled = false;
        //GetComponent<PolygonCollider2D>().enabled = false;
        Destroy(GetComponent<Collider2D>());
        gameObject.GetComponent<Rigidbody2D>().gravityScale += 1.5f;
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        HitEffect();
        HitEffect();
        HitEffect();

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
