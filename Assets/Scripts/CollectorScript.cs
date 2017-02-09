using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollectorScript : MonoBehaviour
{

    //private int sessionScrapCount;
    private int scrapForNextLevel;
    public Text totalScrapCountText;
    public AudioSource ScrapCollectedSound;
    public AudioSource RMCollectedSound;
    private float powerUpDuration = 30;
    public GameObject announcer;
    public GameObject repelShield;
    public GameObject gBomb;

    private bool[] PowerUpActive;
    private float[] PowerUpStartTime;

    void Start()
    {
        scrapForNextLevel = 0;
        //sessionScrapCount = 0;

        PowerUpActive = new bool[GameControlScript.gameControl.GetNumberOfPowerUps()];
        PowerUpStartTime = new float[GameControlScript.gameControl.GetNumberOfPowerUps()];
        for (int i = 0; i < PowerUpActive.Length; i++)
        {
            PowerUpActive[i] = false;
            PowerUpStartTime[i] = 0;
        }
        SetScrapCountText();
    }

    void Update()
    {
        //Handle Power Up Duration, ***DEPRECATED***
        /*for (int i = 0; i < PowerUpActive.Length; i++)
        {
            if (PowerUpActive[i])
            {
                if (Time.time - PowerUpStartTime[i] > powerUpDuration)
                {
                    GameControlScript.gameControl.PowerUps[i] = false;

                    //announcer.GetComponent<AnnouncerTextScript>().Announce(GameControlScript.gameControl.PowerUpNames[i] + " expired!");
                    //Debug.Log("Power up " + i.ToString() + " expired!");
                    PowerUpActive[i] = false;


                }
            }
        }*/
    }

    private void SetScrapCountText()
    {
        //int scrapforlevelup = GameControlScript.gameControl.scrapRequiredForNextLevel * GameControlScript.gameControl.currentLevel;
        totalScrapCountText.text = "Scrap: " + GameControlScript.gameControl.scrapCount.ToString() + "\n"
            + "Research Material: " + GameControlScript.gameControl.researchMaterialCount.ToString() + "\n"
            + "Weapon power: " + GameControlScript.gameControl.WeaponSkill[GameControlScript.gameControl.SelectedWeapon].ToString() + "/100\nZone: " + GameControlScript.gameControl.currentLevel.ToString();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<MeteorScript>() != null)
        {
            col.gameObject.GetComponent<MeteorScript>().Explode();
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
        if (GameControlScript.gameControl.AUDIO_SOUNDS)
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
            GameControlScript.gameControl.researchMaterialCount += amount;
        else
            GameControlScript.gameControl.scrapCount += amount;
        //sessionScrapCount += amount;

        if (FindAnomaly(1) == null && FindAnomaly(2) == null && FindAnomaly(3) == null) //Do not allow level up if a "boss" is present
            scrapForNextLevel += amount;

        SetScrapCountText();

        if (scrapForNextLevel >= GameControlScript.gameControl.scrapRequiredForNextLevel * GameControlScript.gameControl.currentLevel)
        {
            scrapForNextLevel -= GameControlScript.gameControl.scrapRequiredForNextLevel * GameControlScript.gameControl.currentLevel;
            GameControlScript.gameControl.currentLevel += 1;
            SetScrapCountText();
            announcer.GetComponent<AnnouncerScript>().Announce("Zone " + GameControlScript.gameControl.currentLevel.ToString() + "!", FloatingText.FTType.Announcement);
        }
    }

    public void GotPowerUp()
    {
        //Randomize powerup

        int PowerUpNumber = Random.Range(0, GameControlScript.gameControl.GetNumberOfPowerUps());

        announcer.GetComponent<AnnouncerScript>().Announce(GameControlScript.gameControl.PowerUpNames[PowerUpNumber] + " gained!", FloatingText.FTType.PowerUp);
        //Debug.Log("Power up " + PowerUpNumber.ToString() + " gained!");

        //GameControlScript.gameControl.PowerUps[PowerUpNumber] = true;
        //PowerUpStartTime[PowerUpNumber] = Time.time;
        //PowerUpActive[PowerUpNumber] = true;

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

    }

    public void LaunchBomb(PUBombs.PUBombType bombtype)
    {
        GameObject bombInstance = Instantiate(gBomb, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
        bombInstance.GetComponent<PUBombScript>().type = bombtype;
    }

    public GameObject FindAnomaly(int anomalyNumber)
    {
        //Shit code
        GameObject anomaly;
        switch (anomalyNumber)
        {
            case 1:
                anomaly = GameObject.FindWithTag("Anomaly1");
                break;
            case 2:
                anomaly = GameObject.FindWithTag("Anomaly2");
                break;
            case 3:
                anomaly = GameObject.FindWithTag("Anomaly3");
                break;
            default:
                anomaly = null;
                break;
        }
        return anomaly;
    }
}
