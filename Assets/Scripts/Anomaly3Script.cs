using UnityEngine;
using System.Collections;

public class Anomaly3Script : MonoBehaviour {

    public int hitPoints;
    private Transform standingPosition;
    private float speed = 0.3f;
    private GameObject shipHull;
    private int XP = 3000;
    public bool ALIVE = false;
    public GameObject BossHPBar;
    private GameObject fighter;
    private Transform spawnPosition;
    private float spawnInterval = 1.5f;
    private float reloadInterval = 10f;
    private float reloadTime;
    private float spawnTime;
    private Camera mainCamera;
    private float rotateSpeed = 0.2f;
    private int fighterCounter = 6;
    private bool startReloading = false;

    void Awake()
    {
        ALIVE = true;
    }

    void Start ()
    {
        mainCamera = Camera.main;
        spawnTime = Time.time;
        hitPoints = 100000;
        BossHPBar = GameObject.Find("Canvas/BossHP");
        GameObject.Find("Music").GetComponent<MusicScript>().PlayTrack(2);
        shipHull = GameObject.FindWithTag("ShipHull");
        standingPosition = GameObject.Find("BossStandsHere").GetComponent<Transform>();
        fighter = Resources.Load("AlienShip1") as GameObject;
        spawnPosition = transform.FindChild("SpawnPosition");
        UpdateBossHPBar();
    }
	
	void Update ()
    {
        Vector3 target = transform.position;
        target.x -= 1;
        RotateTowards(target);

        if (shipHull != null)
        {
            GetComponent<Rigidbody2D>().velocity = (standingPosition.position - transform.position).normalized * 0.3f;
        }

        if (!IsOnScreen())
            return;

        if (!ALIVE)
            return;

        if (Time.time - spawnTime >= spawnInterval && fighterCounter > 0)
        {
            SpawnFighter(-1f, Random.Range(-1f, 1f));
            spawnTime = Time.time;
        }

        if (startReloading && Time.time - reloadTime >= reloadInterval)
        {
            startReloading = false;
            fighterCounter = 6;
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
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_DESTROYED[2] = true;
            GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_SPAWNED = false;
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(false);
            Destroy(gameObject);
        }
    }

    void UpdateBossHPBar()
    {
        BossHPBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, hitPoints / 1000);
    }

    private void SpawnFighter(float dirX = -1f, float dirY = 0f)
    {
        GameObject fighterInstance;
        fighter.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.Fighter;
        fighterInstance = Instantiate(fighter, spawnPosition.position, spawnPosition.rotation) as GameObject;
        fighterInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(dirX, dirY) * 2;
        fighterCounter--;
        if (fighterCounter == 0)
        {
            startReloading = true;
            reloadTime = Time.time;
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

    private void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, rotateSpeed * Time.deltaTime);
    }
}
