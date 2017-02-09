using UnityEngine;
using System.Collections;

public class PlayerShieldScript : MonoBehaviour {

    public int hitPoints = 0;
    private SpriteRenderer sr;
    private Color newColor;
    public AudioSource shieldSound;
    private int hpBoost = 0;
    public GameObject shieldBar;
    private float spawnTime;

    void Start () {
        sr = gameObject.GetComponent<SpriteRenderer>();
        newColor = sr.color;
        UpdateShieldBar();
        gameObject.SetActive(false);
        spawnTime = Time.time;
	}

    void Update()
    {
        if (hpBoost > 0)
        {
            if (hitPoints + 1 < 100)
                hitPoints++;

            UpdateShieldBar();
            hpBoost--;
            SetTransparency();

        }
    }

    void isHit(int amount)
    {
        if (Time.time - spawnTime < 2)
            return;

        int newHP = hitPoints - amount;
        if (newHP > 0)
            hitPoints = newHP;
        else
            hitPoints = 0;

        UpdateShieldBar();
        SetTransparency();

        if (hitPoints == 0)
            gameObject.SetActive(false);
    }

    public void ActivateShield(int hitPointsGained = 20)
    {

        spawnTime = Time.time;
        hpBoost = hitPointsGained;

        gameObject.SetActive(true);

        if (GameControlScript.gameControl.AUDIO_SOUNDS)
        {
            shieldSound.Play();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            isHit(col.gameObject.GetComponent<MeteorScript>().damage);
        }
        else if (col.gameObject.GetComponent<EnemyProjectileScript>() != null)
        {
            isHit(col.gameObject.GetComponent<EnemyProjectileScript>().damage);
        }
    }

    void SetTransparency()
    {
        newColor.a = hitPoints / 100f;
        //Debug.Log(newColor.a.ToString());
        sr.color = newColor;
    }

    void UpdateShieldBar()
    {
        shieldBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, hitPoints);
    }

}
