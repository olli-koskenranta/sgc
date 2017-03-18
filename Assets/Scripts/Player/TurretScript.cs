using UnityEngine;
using ShipWeapons;

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
        switch (GameControl.gc.SelectedWeapon)
        {
            case 0:
                turret = GameControl.gc.Weapons[0];
                Bullet = Resources.Load("Bullet2") as GameObject;
                break;
            case 1:
                turret = GameControl.gc.Weapons[1];
                Bullet = Resources.Load("ProjectileLaser") as GameObject;
                break;
            case 2:
                turret = GameControl.gc.Weapons[2];
                Bullet = Resources.Load("MassDriverBullet") as GameObject;
                break;
            case 3:
                turret = GameControl.gc.Weapons[3];
                Bullet = Resources.Load("PlasmaBall") as GameObject;
                break;
            default:
                break;
        }

    }

    void Update()
    {
        if (GameControl.gc.GAME_PAUSED)
            return;
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

        /*if (Input.GetKeyDown(KeyCode.Keypad0))
            ChangeWeapon(0);
        if (Input.GetKeyDown(KeyCode.Keypad1))
            ChangeWeapon(1);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            ChangeWeapon(2);
        if (Input.GetKeyDown(KeyCode.R))
            GameControl.gc.ResetData();*/

    }

    private void Shoot()
    {
        
        bool crit = false;
        bool special = false;
        bool special2 = false;
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

        switch (GameControl.gc.SelectedWeapon)
        {
            case 0:

                if (PlayerPrefs.GetInt(GameControl.gc.GetSoundKey(), 1) == 1)
                {
                    BlasterSound.pitch = randomPitch;
                    BlasterSound.Play();
                }

                bulletInstance = Instantiate(Bullet, firingPosition.position, firingPosition.rotation) as GameObject;

                if (special)
                    bulletInstance.GetComponent<SpriteRenderer>().color = Color.green;

                
                break;

            case 1:
                if (PlayerPrefs.GetInt(GameControl.gc.GetSoundKey(), 1) == 1)
                {

                    LaserSound.pitch = randomPitch;
                    LaserSound.Play();
                }

                if (Random.Range(1, 101) <= turret.Special2Chance)
                {
                    special2 = true;
                }

                    bulletInstance = Instantiate(Bullet, firingPosition.position, firingPosition.rotation) as GameObject;

                if (special)
                    bulletInstance.GetComponent<SpriteRenderer>().color = Color.yellow;

                if (crit)
                    bulletInstance.GetComponent<Transform>().localScale = new Vector3(1f, 3f, 1f);




                break;

            case 2:
                if (PlayerPrefs.GetInt(GameControl.gc.GetSoundKey(), 1) == 1)
                {
                    MassDriverSound.pitch = randomPitch;
                    MassDriverSound.Play();
                }

                bulletInstance = Instantiate(Bullet, firingPosition.position, firingPosition.rotation) as GameObject;

                if (special)
                    bulletInstance.GetComponent<SpriteRenderer>().color = Color.magenta;


                break;

            case 3:
                if (PlayerPrefs.GetInt(GameControl.gc.GetSoundKey(), 1) == 1)
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
        bulletScript.damageAccumulation = turret.DamageAccumulation;
        //Debug.Log("Bounces: " + bulletScript.bounces.ToString());
        if (GameControl.gc.SelectedWeapon == 1 && special2)
        {
            
            //float distance = 1 - Input.mousePosition.x / (Screen.width - Screen.width / 10);
            float distance = 0.05f;
            /*if (distance > 0.5f)
                distance = 0.5f;
            else if (distance < 0.05f)
                distance = 0.05f;*/
            //Debug.Log(distance.ToString());
            BeamSplit(1, distance, 0f, 0f, bulletInstance, crit, special);
            BeamSplit(1, -distance, 0f, 0f, bulletInstance, crit, special);
        }
        turret.FireTime = Time.time;


    }

    private void BeamSplit(float dirX, float dirY, float posX, float posY, GameObject splitBeam, bool isCrit, bool isSpecial)
    {
        GameObject bulletInstance;
        Transf trans = new Transf(splitBeam.transform.localPosition, firingPosition.transform.localRotation, splitBeam.transform.localScale);
        


        float speed = turret.Speed;
        float angle = Mathf.Atan2(dirY, dirX) * Mathf.Rad2Deg;
        trans.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        trans.rotation *= splitBeam.transform.rotation;

        bulletInstance = Instantiate(splitBeam, trans.position + new Vector3(posX, posY), trans.rotation) as GameObject;
        PlayerProjectileScript bulletScript = bulletInstance.GetComponent<PlayerProjectileScript>();
        bulletScript.Critical = isCrit;
        bulletScript.Special = isSpecial;

        //bulletInstance.GetComponent<Rigidbody2D>().mass = turret.Mass;
        bulletInstance.GetComponent<Rigidbody2D>().velocity = bulletInstance.transform.TransformDirection(Vector3.right * turret.Speed);
        //bulletInstance.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        //bulletInstance.GetComponent<PlayerProjectileScript>().damage = turret.Damage;
    }



    public Turret GetTurret()
    {
        return turret;
    }

    public void WeaponSkillGained()
    {
        GameObject ftInstance = Instantiate(ft, transform.position, Quaternion.identity) as GameObject;
        ftInstance.GetComponent<Transform>().position += new Vector3(1f, 0f, 0f);
        ftInstance.GetComponent<FloatingTextScript>().text = "Skill +1";
        ftInstance.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.Normal;
        ftInstance.GetComponent<FloatingTextScript>().durationBeforeFading = 2;
        ftInstance.GetComponent<TextMesh>().fontSize = 15;
    }

    public void ChangeWeapon(int weaponNumber)
    {
        GameControl.gc.SelectedWeapon = weaponNumber;
        GameObject.Find("Collector").GetComponent<CollectorScript>().UpdateInfoText();
        Start();
    }

    
}


