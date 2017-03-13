using UnityEngine;
using System.Collections;

public class PlayerProjectileScript : MonoBehaviour {

    private Camera mainCamera;
    public GameObject hit_effect;
    private GameObject bullet_shrapnel;
    private GameObject hugeMeteor;
    private GameObject playerTurret;
    private bool PIERCE = false;
    private bool GRAVITYDMG = false;
    private bool SHRAPNEL = false;
    private bool CRITICAL = false;
    private bool SPECIAL = false;
    private bool BOUNCE = false;
    private bool ROTATE = false;
    public int bounces = 0;

    public float critMultiplier;
    public float mass;
    public int damage;
    public float gravityDmgAmount = 0.1f;
    public float armorPierce;

    private GameObject floatingText;

        

    void Start () {
        mainCamera = Camera.main;
        hugeMeteor = GameObject.FindWithTag("hugeMeteor");
        playerTurret = GameObject.FindWithTag("PlayerTurret");
        gameObject.GetComponent<Rigidbody2D>().mass = mass;
        armorPierce = GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 1] * 0.2f;

        if (GameControl.gc.SelectedWeapon == 0)
            BOUNCE = true;



        if (SPECIAL)
        {
            switch (GameControl.gc.SelectedWeapon)
            {
                case 0:
                    
                    GRAVITYDMG = true;
                    
                    break;

                case 1:
                    PIERCE = true;
                    break;

                case 2:
                    SHRAPNEL = true;
                    break;

                case 3:
                    
                    SHRAPNEL = true;
                    break;


                default:
                    break;

            }
        }

        if (CRITICAL)
        {
            float newDamage = (float)damage * critMultiplier;
            damage = (int)newDamage;
        }


    }
	
	void Update () {

        //Destroy if "out of bounds"
        if (gameObject.transform.position.x > 9 || gameObject.transform.position.x < -9 || gameObject.transform.position.y < -5 || gameObject.transform.position.y > 20)
            Destroy(gameObject);

        //if (!IsOnScreen())
        //    Destroy(gameObject);

        if (ROTATE)
            GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 2f);

        if (hugeMeteor != null && hugeMeteor.GetComponent<MeteorScript>().IsOnScreen())
        {
            Vector2 forceVector = (hugeMeteor.transform.position - transform.position).normalized * gameObject.GetComponent<Rigidbody2D>().mass / 10;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, forceVector.y), ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {

        HitEffect();
        
        //DamageText();

        if (GameControl.gc.SelectedWeapon == 0)
        {
            bounces--;

            if (bounces < 0)
                BOUNCE = false;
        }

        if (SHRAPNEL)
        {
            ClusterBomb(1, 1, 0.1f, 0.1f);
            ClusterBomb(1, -1, 0.1f, -0.1f);
            ClusterBomb(-1, 1, -0.1f, 0.1f);
            ClusterBomb(-1, -1, -0.1f, -0.1f);

            //ClusterBomb(1, 0, 0.2f, 0.0f);
            //ClusterBomb(0, 1, 0.0f, 0.2f);
            //ClusterBomb(-1, 0, -0.2f, 0.0f);
            //ClusterBomb(0, -1, 0.0f, -0.2f);
        }

        if (!BOUNCE)
        {
                Destroy(this.gameObject);
        }
        else
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1f;

        if (col.gameObject.tag == "Collector")
            Destroy(this.gameObject);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name.Equals("EnemyShield"))
        {
            damage = 0;
            return;
        }

        HitEffect();
        //DamageText();

        /*if (col.GetComponent<NebulaScript>() != null)
        {
            Destroy(gameObject);
        }*/

        if (PIERCE)
        {
            float damageReduction = (float)damage / 5f;
            damage -= (int)damageReduction;
            //Debug.Log(damage.ToString());
        }
        else
        {
            if (!BOUNCE)
            {
                Destroy(this.gameObject);
            }
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

    private void HitEffect()
    {
        ParticleSystem.MainModule mm;
        GameObject hiteffect;
        hiteffect = Instantiate(hit_effect, transform.position, Quaternion.identity) as GameObject;
        mm = hiteffect.GetComponent<ParticleSystem>().main;
        mm.startColor = gameObject.GetComponent<SpriteRenderer>().color;
        //hiteffect.GetComponent<ParticleSystem>().main.startColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    private void DamageText(int newDamage)
    {
        GameObject ft;
        ft = Instantiate(floatingText, transform.position, Quaternion.identity) as GameObject;
        ft.GetComponent<FloatingTextScript>().text = newDamage.ToString();
        ft.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.PopUp;
        if (CRITICAL)
        {
            ft.GetComponent<TextMesh>().fontSize = 50;
            ft.GetComponent<TextMesh>().color = Color.yellow;
        }
    }

    private void ClusterBomb(float dirX, float dirY, float posX, float posY)
    {
        GameObject bulletInstance;
        Transform transform;
        
        bullet_shrapnel = gameObject;
        float speed = playerTurret.GetComponent<TurretScript>().GetTurret().Speed;
        transform = gameObject.transform;

        float angle = Mathf.Atan2(dirY, dirX) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
     

        bulletInstance = Instantiate(bullet_shrapnel, transform.position + new Vector3(posX, posY), this.gameObject.transform.rotation) as GameObject;

        bulletInstance.GetComponent<Rigidbody2D>().mass = mass;
        bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(dirX, dirY).normalized * speed;
        bulletInstance.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        //bulletInstance.GetComponent<Transform>().localScale /= 2;
        bulletInstance.GetComponent<PlayerProjectileScript>().damage = damage / 4;
    }

    public bool Piercing
    {
        get { return PIERCE; }
        set { PIERCE = value; }
    }

    public bool GravityDamage
    {
        get { return GRAVITYDMG; }
        set { GRAVITYDMG = value; }
    }

    public bool Shrapnel
    {
        get { return SHRAPNEL; }
        set { SHRAPNEL = value; }
    }

    public bool Critical
    {
        get { return CRITICAL; }
        set { CRITICAL = value; }
    }

    public bool Special
    {
        get { return SPECIAL; }
        set { SPECIAL = value; }
    }


}
