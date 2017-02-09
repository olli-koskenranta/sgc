using UnityEngine;
using ShipWeapons;
using System.Collections;

public class TurretScript : MonoBehaviour {

    

    private Vector3 mousePos;
    private GameObject Bullet;
    public GameObject Laser;
    
    public AudioSource BlasterSound;
    public AudioSource LaserSound;
    public AudioSource MassDriverSound;
    
    private Transform firingPosition;
    private bool firing = false;
    private Turret turret;
    private GameObject ft;

    //private float gravityBombTime = 0f;
    //private float gravityBombInterval = 20f;

    void Start()
    {
        firingPosition = transform.FindChild("FiringPosition");
        ft = Resources.Load("FloatingText") as GameObject;
        //Set up weapon here
        //Weapon types
        //0 = Basic Cannon
        //1 = Laser Cannon
        //2 = Mass Driver
        //3 = Plasma Cannon
        switch (GameControlScript.gameControl.SelectedWeapon)
        {
            case 0:
                turret = GameControlScript.gameControl.Weapons[0];
                Bullet = Resources.Load("Bullet2") as GameObject;
                break;
            case 1:
                turret = GameControlScript.gameControl.Weapons[1];
                Bullet = Resources.Load("ProjectileLaser") as GameObject;
                break;
            case 2:
                turret = GameControlScript.gameControl.Weapons[2];
                Bullet = Resources.Load("MassDriverBullet") as GameObject;
                break;
            case 3:
                turret = GameControlScript.gameControl.Weapons[3];
                Bullet = Resources.Load("PlasmaBall") as GameObject;
                break;
            default:
                break;
        }

    }

    void Update()
    {

        //Look at mouse cursor
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float AngleRad = Mathf.Atan2(mousePos.y - this.transform.position.y, mousePos.x - this.transform.position.x);
        // Get Angle in Degrees
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        // Rotate Object
        this.transform.rotation = Quaternion.Euler(0, 0, AngleDeg);


        if (Input.GetMouseButtonDown(0))
        {
            firing = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            firing = false;
        }

        if (firing)
        {

                if (Time.time - turret.FireTime >= turret.RateOfFire)
                {
                    Shoot();
                }
        }

    }

    private void Shoot()
    {
        
        bool crit = false;
        bool special = false;
        //Roll for crit
        int roll = Random.Range(1, 101);
        if (roll <= turret.CriticalChance)
            crit = true;
        //Roll for special
        roll = Random.Range(1, 101);
        if (roll <= turret.SpecialChance)
            special = true;

        GameObject bulletInstance;
        PlayerProjectileScript bulletScript;
        float randomPitch = 1f + Random.Range(-0.05f, 0.05f); //+ Random.Range(-0.5f, 0.5f);

        switch (GameControlScript.gameControl.SelectedWeapon)
        {
            case 0:

                if (GameControlScript.gameControl.AUDIO_SOUNDS)
                {
                    BlasterSound.pitch = randomPitch;
                    BlasterSound.Play();
                }

                bulletInstance = Instantiate(Bullet, firingPosition.position, firingPosition.rotation) as GameObject;

                if (special)
                    bulletInstance.GetComponent<SpriteRenderer>().color = Color.green;

                
                break;

            case 1:
                if (GameControlScript.gameControl.AUDIO_SOUNDS)
                {

                    LaserSound.pitch = randomPitch;
                    LaserSound.Play();
                }

                bulletInstance = Instantiate(Bullet, firingPosition.position, firingPosition.rotation) as GameObject;

                if (special)
                    bulletInstance.GetComponent<SpriteRenderer>().color = Color.yellow;

                if (crit)
                    bulletInstance.GetComponent<Transform>().localScale = new Vector3(1f, 3f, 1f);




                break;

            case 2:
                if (GameControlScript.gameControl.AUDIO_SOUNDS)
                {
                    MassDriverSound.pitch = randomPitch;
                    MassDriverSound.Play();
                }

                bulletInstance = Instantiate(Bullet, firingPosition.position, firingPosition.rotation) as GameObject;

                if (special)
                    bulletInstance.GetComponent<SpriteRenderer>().color = Color.magenta;


                break;

            case 3:
                if (GameControlScript.gameControl.AUDIO_SOUNDS)
                {
                    LaserSound.pitch = randomPitch;
                    LaserSound.Play();
                }

                bulletInstance = Instantiate(Bullet, firingPosition.position, firingPosition.rotation) as GameObject;


                if (special)
                    bulletInstance.GetComponent<SpriteRenderer>().color = Color.magenta;


                break;

            default:
                bulletInstance = Instantiate(Bullet, firingPosition.position, firingPosition.rotation) as GameObject;
                break;
        }

        
        bulletInstance.GetComponent<Rigidbody2D>().velocity = firingPosition.TransformDirection(Vector3.right * turret.Speed);
        bulletScript = bulletInstance.GetComponent<PlayerProjectileScript>();

        if (bulletInstance.GetComponent<TrailRenderer>() != null)
        {
            if (crit)
            {
                bulletInstance.GetComponent<TrailRenderer>().material.SetColor("_TintColor", bulletInstance.GetComponent<SpriteRenderer>().color);


            }
            else
                bulletInstance.GetComponent<TrailRenderer>().enabled = false;
        }
        


        bulletScript.Critical = crit;
        bulletScript.Special = special;
        bulletScript.mass = turret.Mass;
        bulletScript.damage = turret.Damage;
        bulletScript.critMultiplier = turret.CriticalMultiplier;
        bulletScript.bounces = turret.Bounces; 
        //Debug.Log("Bounces: " + bulletScript.bounces.ToString());
        turret.FireTime = Time.time;


    }



    public Turret GetTurret()
    {
        return turret;
    }

    public void WeaponSkillGained()
    {
        GameObject ftInstance = Instantiate(ft, transform.position, Quaternion.identity) as GameObject;
        ftInstance.GetComponent<Transform>().position += new Vector3(1f, 0f, 0f);
        ftInstance.GetComponent<FloatingTextScript>().text = "Power +1";
        ftInstance.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.Normal;
        ftInstance.GetComponent<FloatingTextScript>().durationBeforeFading = 2;
        ftInstance.GetComponent<TextMesh>().fontSize = 15;
    }



    
}


