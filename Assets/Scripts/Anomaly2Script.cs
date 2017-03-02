using ShipWeapons;
using Asteroids;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Anomaly2Script : MonoBehaviour {

    public int hitPoints;
    public int maxHitPoints;
    private Transform standingPosition;
    private float mass;
    private GameObject shipHull;
    private int XP = 2000;
    public bool ALIVE = false;
    public Slider BossHPBar;

    void Awake()
    {
        ALIVE = true;
    }

    void Start ()
    {
        hitPoints = 10000;
        maxHitPoints = hitPoints;
        GameObject temp = GameObject.Find("Canvas/SliderBossHP");
        BossHPBar = temp.GetComponent<Slider>();
        BossHPBar.maxValue = maxHitPoints;
        GameObject.Find("Music").GetComponent<MusicScript>().PlayTrack(2);
        shipHull = GameObject.FindWithTag("ShipHull");
        standingPosition = GameObject.Find("BossStandsHere").GetComponent<Transform>();
        UpdateBossHPBar();

    }
	
	void Update ()
    {

        GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 0.2f);
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

    void isHit(int amount)
    {
        if (!ALIVE)
            return;
        hitPoints -= amount;
        UpdateBossHPBar();
        if (hitPoints <= 0)
        {
            GameObject.Find("Music").GetComponent<MusicScript>().PlayTrack(1);
            GameControlScript.gameControl.ExperienceGained(XP);
            ALIVE = false;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_DESTROYED[1] = true;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_SPAWNED = false;
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(false);
            Destroy(gameObject);
        }
    }

    void UpdateBossHPBar()
    {
        BossHPBar.value = hitPoints;
    }
}
