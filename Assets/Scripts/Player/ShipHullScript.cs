using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipHullScript : MonoBehaviour {


    private int hitPoints;
    public AudioSource soundHullHit;
    public GameObject hit_effect;
    private float destroyed_time;
    private float destroyed_interval = 0.3f;
    public Slider hullBar;
    private int maxHitPoints;

    private float repairBotsInterval = 1f;
    private float repairBotsTime;


    void Start () {
        hitPoints = 100;
        maxHitPoints = hitPoints;
        hullBar.maxValue = maxHitPoints;
        GameControl.gc.PLAYER_ALIVE = true;
        UpdateHullBar();
        repairBotsTime = Time.time;
	}
	
	void Update () {
	    if (!GameControl.gc.PLAYER_ALIVE)
        {
            if (Time.time - destroyed_time >= destroyed_interval)
            {
                Vector3 rngpos = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-2f, 2f), 0f);
                
                Instantiate(hit_effect, transform.position + rngpos, Quaternion.identity);
                destroyed_time = Time.time;
            }
            transform.position += new Vector3(0f, -0.01f, 0);
            if (transform.position.y < -5)
            {
                GameObject.Find("UIControl").GetComponent<UIControlScript>().ReturnToBaseClicked();
            }
        }
        else
        {
            if (GameControl.gc.ShipRepairBots)
            {
                if (Time.time - repairBotsTime >= repairBotsInterval)
                {
                    if (hitPoints < 100)
                    {
                        hitPoints += 1;
                        UpdateHullBar();
                    }
                    repairBotsTime = Time.time;
                }
            }
        }
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            MeteorScript asteroid = col.gameObject.GetComponent<MeteorScript>();
            isHit(asteroid.damage);
            if (GameControl.gc.ShipReactiveArmor)
            {
                asteroid.isHit(asteroid.hitPoints);
            }
        }

        if (col.gameObject.GetComponent<EnemyProjectileScript>() != null)
        {
            EnemyProjectileScript enemyBullet = col.gameObject.GetComponent<EnemyProjectileScript>();
            isHit(enemyBullet.damage);
        }
    }

    public void isHit(int Damage)
    {
        if (GameControl.gc.AUDIO_SOUNDS)
            soundHullHit.Play();

        if (hitPoints - Damage > 0)
        {
            hitPoints -= Damage;
        }
        else
        {
            hitPoints = 0;
        }

        if (hitPoints <= 0 && GameControl.gc.PLAYER_ALIVE)
        {
            destroyed_time = Time.time;
            GameControl.gc.PLAYER_ALIVE = false;
        }

        UpdateHullBar();
    }

    private void UpdateHullBar()
    {
        hullBar.value = hitPoints;
    }
}
