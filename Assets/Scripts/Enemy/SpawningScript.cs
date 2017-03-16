using UnityEngine;
using Asteroids;
using System.Collections;

public class SpawningScript : MonoBehaviour {

    private float spawnTime = 0f;
    private float spawnInterval = 1f;

    public GameObject medMeteor1;
    public GameObject bigMeteor1;
    public GameObject hugeMeteor1;
    public GameObject Nebula;
    public GameObject powerUpContainer;
    
    public GameObject announcer;
    public GameObject enemyFighter;
    public GameObject enemyMissileCruiser;
    public GameObject enemyBattleShip;

    public GameObject anomaly1;
    public GameObject anomaly2;
    public GameObject anomaly3;
    public GameObject anomaly4;
    public GameObject anomaly5;

    private float PUSpawnTime;

    private int PUChance;
    private int bigAsteroidChance;

    private float HugeAsteroidInterval;
    private float EnemyFighterInterval;
    private float EnemyMissileCruiserInterval;
    private float NebulaInterval;
    private float EnemyBattleShipInterval;

    private float HugeAsteroidSpawnTime;
    private float EnemyFighterSpawnTime;
    private float EnemyMissileCruiserSpawnTime;
    private float NebulaSpawnTime;
    private float EnemyBattleShipSpawnTime;

    public bool ANOMALY_SPAWNED = false;

    public enum EnemyType { medMeteor, bigMeteor, hugeMeteor, Nebula, Fighter, MissileCruiser, BattleShip }

    void Start () {

        GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(false);

        bigAsteroidChance = 30;
        //Define spawn chances here (%), maybe some increase per level? Ok, lets change these to times
        /*PUChance = 1;
        HugeAsteroidChance = 1;
        NebulaChance = 1;
        EnemyFighterChance = 2;
        EnemyMissileCruiserChance = 1;
        EnemyBattleShipChance = 1;*/

        HugeAsteroidInterval = 30;
        NebulaInterval = 60;
        EnemyFighterInterval = 15;
        EnemyMissileCruiserInterval = 45;
        EnemyBattleShipInterval = 90;



        PUSpawnTime = 0;
        medMeteor1 = Resources.Load("medMeteor1") as GameObject;
        bigMeteor1 = Resources.Load("bigmeteor1") as GameObject;
        hugeMeteor1 = Resources.Load("hugeMeteor1") as GameObject;
        powerUpContainer = Resources.Load("PowerUpContainer") as GameObject;
        anomaly1 = Resources.Load("Anomaly1") as GameObject;
        anomaly2 = Resources.Load("Anomaly2") as GameObject;
        anomaly3 = Resources.Load("Carrier") as GameObject;
        anomaly4 = Resources.Load("BattleStation") as GameObject;
        anomaly5 = Resources.Load("GasAnomaly") as GameObject;
        enemyFighter = Resources.Load("AlienShip1") as GameObject;
        enemyMissileCruiser = Resources.Load("MissileCruiser") as GameObject;
        Nebula = Resources.Load("Nebula") as GameObject;
        enemyBattleShip = Resources.Load("BattleShip") as GameObject;

	}
	
	void Update () {

        if (Time.time - spawnTime >= spawnInterval)
        {
            SpawnMeteor();
            spawnTime = Time.time;
        }

        if (Time.time - PUSpawnTime >= 30)
        {
            SpawnPowerUp();
        }

        if ((GameControl.gc.currentLevel == 10 || (GameControl.gc.currentLevel - 10) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(1);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        if ((GameControl.gc.currentLevel == 20 || (GameControl.gc.currentLevel - 20) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(2);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        if ((GameControl.gc.currentLevel == 30 || (GameControl.gc.currentLevel - 30) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(3);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        if ((GameControl.gc.currentLevel == 40 || (GameControl.gc.currentLevel - 40) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(4);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        if ((GameControl.gc.currentLevel == 50 || (GameControl.gc.currentLevel - 50) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(5);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        if (!FindAnomaly(5))
        {
            if (GameControl.gc.currentLevel >= 5)
            {
                if (Time.time - HugeAsteroidSpawnTime >= HugeAsteroidInterval)
                {
                    SpawnEnemy(EnemyType.hugeMeteor);
                }
            }

            if (GameControl.gc.currentLevel >= 11)
            {
                if (Time.time - NebulaSpawnTime >= NebulaInterval)
                {
                    SpawnEnemy(EnemyType.Nebula);
                }
            }

            if (GameControl.gc.currentLevel >= 21)
            {
                if (Time.time - EnemyFighterSpawnTime >= EnemyFighterInterval)
                {
                    SpawnEnemy(EnemyType.Fighter);
                }
            }

            if (GameControl.gc.currentLevel >= 31)
            {
                if (Time.time - EnemyMissileCruiserSpawnTime >= EnemyMissileCruiserInterval)
                {
                    SpawnEnemy(EnemyType.MissileCruiser);
                }
            }

            if (GameControl.gc.currentLevel >= 41)
            {
                if (Time.time - EnemyBattleShipSpawnTime >= EnemyBattleShipInterval)
                {
                    SpawnEnemy(EnemyType.BattleShip);
                }
            }
        }

    }

    public void SpawnEnemy(EnemyType type)
    {
        Transform spawnPosition = transform;

        switch (type)
        {
            case EnemyType.hugeMeteor:
                spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(-3f, 4f), 0f);
                hugeMeteor1.GetComponent<MeteorScript>().asteroidType = AsteroidType.Huge;
                Instantiate(hugeMeteor1, spawnPosition.position, spawnPosition.rotation);
                HugeAsteroidSpawnTime = Time.time;
                break;
            case EnemyType.Nebula:
                spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(-3f, 2f), 0f);
                GameObject nebulaInstance = Instantiate(Nebula, spawnPosition.position, spawnPosition.rotation) as GameObject;
                NebulaSpawnTime = Time.time;
                break;
            case EnemyType.Fighter:
                enemyFighter.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.Fighter;
                spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(0f, 4f), 0f);
                Instantiate(enemyFighter, spawnPosition.position, spawnPosition.rotation);
                EnemyFighterSpawnTime = Time.time;
                break;
            case EnemyType.MissileCruiser:
                enemyMissileCruiser.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.MissileCruiser;
                spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(0f, 4f), 0f);
                Instantiate(enemyMissileCruiser, spawnPosition.position, spawnPosition.rotation);
                EnemyMissileCruiserSpawnTime = Time.time;
                break;
            case EnemyType.BattleShip:
                enemyBattleShip.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.BattleShip;
                spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(4f, 5f), 0f);
                Instantiate(enemyBattleShip, spawnPosition.position, spawnPosition.rotation);
                EnemyBattleShipSpawnTime = Time.time;
                break;
            default:
                break;
        }
    }

    void SpawnMeteor()
    {

        //if (FindAnomaly(3) || FindAnomaly(4))
        //    return;

        Transform spawnPosition = transform;
        float randomScaleFactor;

        //Big Asteroid
        if (RollDice(100) <= bigAsteroidChance)
        {
            spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(-3f, 4f), 0f);
            GameObject bigMeteorInstance = Instantiate(bigMeteor1, spawnPosition.position, spawnPosition.rotation) as GameObject;
            bigMeteorInstance.GetComponent<MeteorScript>().asteroidType = AsteroidType.Big;
            randomScaleFactor = Random.Range(0, 0.5f);
            bigMeteorInstance.GetComponent<Transform>().localScale += new Vector3(randomScaleFactor, randomScaleFactor, 0);
        }

        //Medium asteroid
        else
        {
            spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(-3f, 4f), 0f);
            GameObject medMeteorInstance = Instantiate(medMeteor1, spawnPosition.position, spawnPosition.rotation) as GameObject;
            medMeteorInstance.GetComponent<MeteorScript>().asteroidType = AsteroidType.Medium;
            randomScaleFactor = Random.Range(0, 0.5f);
            medMeteorInstance.GetComponent<Transform>().localScale += new Vector3(randomScaleFactor, randomScaleFactor, 0);
        }


        

        spawnInterval = 1.1f - (GameControl.gc.currentLevel % 10) / 10f;
        if (spawnInterval < 0.3f || GameControl.gc.currentLevel % 10 == 0)
            spawnInterval = 0.3f;
    }

    void SpawnPowerUp()
    {
        Transform spawnPosition = transform;
        spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(0f, 4f), 0f);
        Instantiate(powerUpContainer, spawnPosition.position, spawnPosition.rotation);
        PUSpawnTime = Time.time;
    }

    void SpawnAnomaly(int anomalyNumber)
    {
        Transform spawnPosition = transform;
        spawnPosition.position = new Vector3(15f, 3f, 0f);
        GameObject anomaly;
        switch (anomalyNumber)
        {
            case 1:
                
                anomaly = Instantiate(anomaly1, spawnPosition.position, spawnPosition.rotation);
                anomaly.GetComponent<AnomalyScript>().anomalyNumber = anomalyNumber;
                break;
            case 2:
                anomaly = Instantiate(anomaly2, spawnPosition.position, spawnPosition.rotation);
                anomaly.GetComponent<AnomalyScript>().anomalyNumber = anomalyNumber;
                break;
            case 3:
                anomaly = Instantiate(anomaly3, spawnPosition.position, spawnPosition.rotation);
                anomaly.GetComponent<AnomalyScript>().anomalyNumber = anomalyNumber;
                break;
            case 4:
                anomaly = Instantiate(anomaly4, spawnPosition.position, spawnPosition.rotation);
                anomaly.GetComponent<AnomalyScript>().anomalyNumber = anomalyNumber;
                break;
            case 5:
                anomaly = Instantiate(anomaly5, spawnPosition.position, spawnPosition.rotation);
                anomaly.GetComponent<AnomalyScript>().anomalyNumber = anomalyNumber;
                break;
            default:
                break;
        }

        ANOMALY_SPAWNED = true;

    }

    public bool FindAnomaly(int anomalyNumber)
    {
        string name = "Anomaly" + anomalyNumber.ToString();
        if (GameObject.FindWithTag(name) != null)
            return true;
        else
            return false;
    }

    //Roll a number between 1 and maxValue
    int RollDice(int maxValue)
    {
        return Random.Range(1, maxValue + 1);
    }

    float RollDice(float maxValue)
    {
        return Random.Range(1f, maxValue + 1f);
    }
}
