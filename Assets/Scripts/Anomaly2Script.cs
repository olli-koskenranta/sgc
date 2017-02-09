using ShipWeapons;
using Asteroids;
using UnityEngine;
using System.Collections;

public class Anomaly2Script : MonoBehaviour {

    public int hitPoints;
    private Transform standingPosition;
    private float speed = 0.3f;
    private float mass;
    private GameObject shipHull;
    private int XP = 1000;
    public bool ALIVE = false;
    public GameObject BossHPBar;

    void Awake()
    {
        ALIVE = true;
    }

    void Start ()
    {
        hitPoints = 100000;
        BossHPBar = GameObject.Find("Canvas/BossHP");
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
        BossHPBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, hitPoints / 1000);
    }
}
