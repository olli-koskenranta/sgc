using UnityEngine;
using ShipWeapons;
using System.Collections;

public class Anomaly1Script : MonoBehaviour {

    public int hitPoints;
    private float speed = 0.3f;
    private float mass;
    private GameObject shipHull;
    public bool ALIVE = false;
    private GameObject[] medMeteors;
    private GameObject[] bigMeteors;
    private GameObject[] hugeMeteors;
    private Turret playerTurret;
    private int XP = 100;
    public GameObject BossHPBar;


    void Awake()
    {
        ALIVE = true;
    }

	void Start () {
        hitPoints = 1000;
        BossHPBar = GameObject.Find("Canvas/BossHP");
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

        playerTurret = GameObject.FindWithTag("PlayerTurret").GetComponent<TurretScript>().GetTurret();
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
        switch (col.gameObject.tag)
        {
            case "Bullet2":
                isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
                break;
            default:
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Bullet2":
                isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
                break;
            default:
                break;
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
            GameControlScript.gameControl.ExperienceGained(XP);
            ALIVE = false;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_DESTROYED[0] = true;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_SPAWNED = false;
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(false);
            Destroy(gameObject);
        }
    }

    void UpdateBossHPBar()
    {
        BossHPBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, hitPoints/10);
    }
}
