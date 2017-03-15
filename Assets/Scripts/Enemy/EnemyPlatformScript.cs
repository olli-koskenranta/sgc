using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlatformScript : MonoBehaviour {

    private float gravityHitTime;
    private float gravityHitResetTime = 2f;
    private int hitPoints;

    private float destroyed_time;
    private float destroyed_interval = 0.3f;
    public bool ALIVE;
    public int XP = 0;
    private GameObject hit_effect;
    private GameObject floatingText;
    private bool iscrit = false;
    private int damageStacks;

    void Start ()
    {
        floatingText = Resources.Load("FloatingText") as GameObject;
        ALIVE = true;
        hitPoints = 100000;
        gravityHitTime = Time.time;
        hit_effect = Resources.Load("Explosion") as GameObject;
    }
	
	void Update ()
    {
        if (Time.time - gravityHitTime >= gravityHitResetTime && ALIVE)
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }

        if (Time.time - destroyed_time >= destroyed_interval && hitPoints <= 0)
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
            float nDamage = (float)Damage * (damageStacks * GameControl.gc.Weapons[2].DamageAccumulation);
            Damage = (int)nDamage;
        }

        hitPoints -= Damage;
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
        //Destroy(this.gameObject, 1);
        gameObject.GetComponent<Rigidbody2D>().gravityScale += 1.5f;
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        HitEffect();
        HitEffect();
        HitEffect();

    }

    private void HitEffect()
    {
        ParticleSystem.MainModule mm;
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, transform.position, Quaternion.identity) as GameObject;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
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
