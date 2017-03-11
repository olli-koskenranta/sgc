using UnityEngine;
using UnityEngine.UI;
using ShipWeapons;
using System.Collections;

public class Anomaly1Script : MonoBehaviour {

    public int hitPoints;
    public int maxHitPoints;
    private float mass;
    private GameObject shipHull;
    public bool ALIVE = false;
    private GameObject[] medMeteors;
    private GameObject[] bigMeteors;
    private GameObject[] hugeMeteors;
    private int XP = 1000;
    public Slider BossHPBar;
    public int damage = 100;


    void Awake()
    {
        ALIVE = true;
    }

	void Start () {
        hitPoints = 1000;
        maxHitPoints = hitPoints;
        GameObject temp = GameObject.Find("Canvas/SliderBossHP");
        BossHPBar = temp.GetComponent<Slider>();
        BossHPBar.maxValue = maxHitPoints;
        GameObject.Find("Music").GetComponent<MusicScript>().PlayTrack(2);
        shipHull = GameObject.FindWithTag("ShipHull");
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

        UpdateBossHPBar();


    }
	
	void Update () {

        GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 0.5f);
        if (shipHull != null)
        {
            GetComponent<Rigidbody2D>().velocity = (shipHull.transform.position - transform.position).normalized * 0.3f;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
        }

        if (col.gameObject.GetComponent<ShipHullScript>() != null)
        {
            col.gameObject.GetComponent<ShipHullScript>().isHit(damage);
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

        if (hitPoints - amount >= 0)
            hitPoints -= amount;
        else
            hitPoints = 0;

        UpdateBossHPBar();

        if (hitPoints <= 0)
        {
            GameObject.Find("Music").GetComponent<MusicScript>().PlayTrack(1);
            GameControl.gc.ExperienceGained(XP);
            ALIVE = false;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_DESTROYED[0] = true;
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
