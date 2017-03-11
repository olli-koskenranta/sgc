using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Anomaly4Script : MonoBehaviour {

    private GameObject shipHull;
    private Transform standingPosition;

    private GameObject shield;
    private GameObject generator;

    public int hitPoints;
    public int maxHitPoints;
    private int generatorHitPoints;

    private int XP = 3000;
    public bool ALIVE = false;
    public Slider BossHPBar;

    public GameObject hit_effect;

    void Awake()
    {
        ALIVE = true;
    }

    void Start () {
        shipHull = GameObject.FindWithTag("ShipHull");
        standingPosition = GameObject.Find("BossStandsHere").GetComponent<Transform>();
        shield = GameObject.FindWithTag("EShield");
        generator = GameObject.FindWithTag("EGenerator");
        hit_effect = Resources.Load("Explosion") as GameObject;
        generatorHitPoints = 10;
        hitPoints = 1000000;
        maxHitPoints = hitPoints;

        GameObject temp = GameObject.Find("Canvas/SliderBossHP");
        BossHPBar = temp.GetComponent<Slider>();
        BossHPBar.maxValue = maxHitPoints;
        GameObject.Find("Music").GetComponent<MusicScript>().PlayTrack(2);
        UpdateBossHPBar();
    }
	
	void Update () {
        if (GameControl.gc.GAME_PAUSED)
            return;

        GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * 0.2f);

        if (shipHull != null)
        {
            GetComponent<Rigidbody2D>().velocity = (standingPosition.position - transform.position).normalized * 0.3f;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
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
                Debug.Log(generatorHitPoints.ToString());
            }
            else
            {
                Destroy(generator);
                Destroy(shield);
            }
            
        }
    }

    public void OnGeneratorTriggerEnter(Collider2D col)
    {

    }

    void isHit(int amount)
    {
        if (!ALIVE)
            return;
        hitPoints -= amount;
        UpdateBossHPBar();
        if (hitPoints <= 0)
        {
            GameObject.Find("Music").GetComponent<MusicScript>().PlayTrack(1);
            GameControl.gc.ExperienceGained(XP);
            ALIVE = false;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_DESTROYED[3] = true;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_SPAWNED = false;
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(false);
            Destroy(gameObject);
        }
    }

    private void HitEffect()
    {
        ParticleSystem.MainModule mm;
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, transform.position, Quaternion.identity) as GameObject;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    void UpdateBossHPBar()
    {
        BossHPBar.value = hitPoints;
    }
}
