using UnityEngine;
using UnityEngine.UI;

public class CollectorScript : MonoBehaviour
{

    public Text textScrap;
    public Text textRM;
    public Text textWeaponSkill;
    public Text textCurrentZone;
    public AudioSource ScrapCollectedSound;
    public AudioSource RMCollectedSound;
    private float powerUpDuration = 30;
    public GameObject announcer;
    public GameObject repelShield;
    public GameObject gBomb;

    private bool[] PowerUpActive;
    private float[] PowerUpStartTime;

    public int grinderDamage = 1;
    private float levelStartTime;
    private float levelUpInterval = 30f;

    private float shieldGenTime;
    private float shieldGenInterval = 10f;

    public bool ContGame = false;

    void Start()
    {
        shieldGenTime = Time.time;
        levelStartTime = Time.time;
        PowerUpActive = new bool[GameControl.gc.GetNumberOfPowerUps()];
        PowerUpStartTime = new float[GameControl.gc.GetNumberOfPowerUps()];
        for (int i = 0; i < PowerUpActive.Length; i++)
        {
            PowerUpActive[i] = false;
            PowerUpStartTime[i] = 0;
        }
        UpdateInfoText();
    }

    void Update()
    {
        if (GameControl.gc.currentLevel == 201 && !ContGame)
            return;

        if (Time.time - levelStartTime >= levelUpInterval && !IsBossPresent())
        {
            LevelUp();
        }

        if (GameControl.gc.ShipShieldGenerator)
        {
            if (Time.time - shieldGenTime >= shieldGenInterval && GameControl.gc.PLAYER_ALIVE)
            {
                repelShield.GetComponent<PlayerShieldScript>().ActivateShield(10);
                shieldGenTime = Time.time;
            }
        }
    }

    public void UpdateInfoText()
    {
        textScrap.text = GameControl.gc.scrapCount.ToString();
        textRM.text = GameControl.gc.researchMaterialCount.ToString();
        textWeaponSkill.text = "Weapon Skill: " + GameControl.gc.WeaponSkill[GameControl.gc.SelectedWeapon].ToString() + "/" + GameControl.gc.Weapons[GameControl.gc.SelectedWeapon].SkillCap.ToString();
        textCurrentZone.text ="Zone: " + GameControl.gc.currentLevel.ToString();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            col.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1f;
        }
        else if (col.gameObject.GetComponent<ScrapPieceScript>() != null)
        {
            if (col.gameObject.GetComponent<ScrapPieceScript>().type == Scrap.ScrapType.Normal)
                ScrapCollected(col.gameObject.GetComponent<ScrapPieceScript>().scrapAmount);

            else if (col.gameObject.GetComponent<ScrapPieceScript>().type == Scrap.ScrapType.ResearchMaterial)
                ScrapCollected(col.gameObject.GetComponent<ScrapPieceScript>().researchMaterialAmount, true);
            Destroy(col.gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            int grinderdmg = col.gameObject.GetComponent<MeteorScript>().maxHitPoints / 100;
            if (grinderdmg == 0) grinderdmg = 1;
            col.gameObject.GetComponent<MeteorScript>().isHit(grinderdmg, true, false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<ScrapPieceScript>() != null)
        {
            if (col.gameObject.GetComponent<ScrapPieceScript>().type == Scrap.ScrapType.Normal)
                ScrapCollected(col.gameObject.GetComponent<ScrapPieceScript>().scrapAmount);

            else if (col.gameObject.GetComponent<ScrapPieceScript>().type == Scrap.ScrapType.ResearchMaterial)
                ScrapCollected(col.gameObject.GetComponent<ScrapPieceScript>().researchMaterialAmount, true);
            Destroy(col.gameObject);
        }

        switch (col.gameObject.tag)
        {
            case "PowerUp":
                GotPowerUp();
                Destroy(col.gameObject);
                break;
            default:
                break;
        }
    }

    private void ScrapCollected(int amount, bool isResearchMaterial = false)
    {
        if (PlayerPrefs.GetInt(GameControl.gc.GetSoundKey(), 1) == 1)
        {
            float randomPitch = 1f + Random.Range(-0.05f, 0.05f);
            if (isResearchMaterial)
            {
                RMCollectedSound.pitch = randomPitch;
                RMCollectedSound.Play();
            }
            else
            {
                ScrapCollectedSound.pitch = randomPitch;
                ScrapCollectedSound.Play();
            }
        }



        if (isResearchMaterial)
            GameControl.gc.researchMaterialCount += amount;
        else
        {
            GameControl.gc.scrapCount += amount;
        }


        UpdateInfoText();


    }

    public void LevelUp()
    {

        GameControl.gc.currentLevel += 1;
        GameObject.Find("MeteorSpawning").GetComponent<SpawningScript>().ANOMALY_SPAWNED = false;
        if (GameControl.gc.highestLevelAchieved < GameControl.gc.currentLevel)
            GameControl.gc.highestLevelAchieved = GameControl.gc.currentLevel;
        UpdateInfoText();
        if (GameControl.gc.currentLevel == 5 || (GameControl.gc.currentLevel - 1) % 10 == 0)
        {
            if (GameControl.gc.StartZoneUnlocked.Find(i => i == GameControl.gc.currentLevel) == 0)
            {
                GameControl.gc.StartZoneUnlocked.Add(GameControl.gc.currentLevel);
                announcer.GetComponent<AnnouncerScript>().Announce("Zone " + GameControl.gc.currentLevel.ToString() + "!"
                    + "\nNew Start Zone Unlocked!", FloatingText.FTType.Announcement);
                levelStartTime = Time.time;
                return;
            }
        }

        announcer.GetComponent<AnnouncerScript>().Announce("Zone " + GameControl.gc.currentLevel.ToString() + "!", FloatingText.FTType.Announcement);
        levelStartTime = Time.time;
    }

    public bool IsBossPresent()
    {
        if (FindAnomaly(1) || FindAnomaly(2) || FindAnomaly(3) || FindAnomaly(4) || FindAnomaly(5))
            return true;
        else
            return false;
    }

    public void GotPowerUp()
    {
        //Randomize powerup

        int PowerUpNumber = Random.Range(0, GameControl.gc.GetNumberOfPowerUps());

        if (GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 6] == 90)
        {
            while (PowerUpNumber == 3)
            {
                PowerUpNumber = Random.Range(0, GameControl.gc.GetNumberOfPowerUps());
            }
        }

        

        if (PowerUpNumber == 0)
        {
            LaunchBomb(PUBombs.PUBombType.Kinetic);
        }

        else if (PowerUpNumber == 1)
        {
            repelShield.GetComponent<PlayerShieldScript>().ActivateShield(50);
        }

        else if (PowerUpNumber == 2)
        {
            LaunchBomb(PUBombs.PUBombType.Gravity);
        }

        else if (PowerUpNumber == 3)
        {
            if (GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 6] < 90)
                GameControl.gc.WeaponUpgrades[GameControl.gc.SelectedWeapon, 6] += 1;
            GameObject.FindWithTag("PlayerTurret").GetComponent<TurretScript>().GetTurret().UpdateValues(GameControl.gc.SelectedWeapon);
            UpdateInfoText();
            announcer.GetComponent<AnnouncerScript>().Announce(GameControl.gc.PowerUpNames[PowerUpNumber] + " increased!", FloatingText.FTType.PowerUp);
        }

        if (PowerUpNumber != 3)
            announcer.GetComponent<AnnouncerScript>().Announce(GameControl.gc.PowerUpNames[PowerUpNumber] + " gained!", FloatingText.FTType.PowerUp);

    }

    public void LaunchBomb(PUBombs.PUBombType bombtype)
    {
        GameObject bombInstance = Instantiate(gBomb, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
        bombInstance.GetComponent<PUBombScript>().type = bombtype;
    }

    public GameObject FindAnomaly(int anomalyNumber)
    {
        string name = "Anomaly" + anomalyNumber.ToString();
        return GameObject.FindWithTag(name);
    }
}
