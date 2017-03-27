using UnityEngine;
using Asteroids;

public class SpawningScript : MonoBehaviour {

    private float spawnTime = 0f;
    private float spawnInterval = 1f;

    public GameObject medMeteor1;
    public GameObject bigMeteor1;
    public GameObject hugeMeteor1;
    public GameObject goldenMeteor;
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
    private int goldenAsteroidChance;

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

    public bool EARTH_SPAWNED = false;
    public GameObject earthInstance;

    private Vector2 topLeft;
    private Vector2 bottomRight;
    Vector2[] spawnAreaCoordinates;

    public enum EnemyType { medMeteor, bigMeteor, hugeMeteor, Nebula, Fighter, MissileCruiser, BattleShip }

    void Start () {

        GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(false);

        bigAsteroidChance = 30;
        goldenAsteroidChance = 100;
        
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

        topLeft = new Vector2();
        bottomRight = new Vector2();
        spawnAreaCoordinates = new Vector2[2];

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
        goldenMeteor = Resources.Load("GoldenMeteor") as GameObject;

	}
	
	void Update () {

        if (GameControl.gc.currentLevel == 201)
        {
            if (!EARTH_SPAWNED)
            {
                GameObject earth = Resources.Load("Earth") as GameObject;
                earthInstance = Instantiate(earth, new Vector3(15, 0, 0), Quaternion.identity);
                announcer.GetComponent<AnnouncerScript>().Announce("EARTH DETECTED!", FloatingText.FTType.Danger);
                EARTH_SPAWNED = true;
            }
            return;
        }

        if ((GameControl.gc.currentLevel == 10 || (GameControl.gc.currentLevel - 10) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(1);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        else if ((GameControl.gc.currentLevel == 20 || (GameControl.gc.currentLevel - 20) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(2);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        else if ((GameControl.gc.currentLevel == 30 || (GameControl.gc.currentLevel - 30) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(3);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        else if ((GameControl.gc.currentLevel == 40 || (GameControl.gc.currentLevel - 40) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(4);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        else if ((GameControl.gc.currentLevel == 50 || (GameControl.gc.currentLevel - 50) % 50 == 0) && !ANOMALY_SPAWNED)
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(5);
            announcer.GetComponent<AnnouncerScript>().Announce("!ANOMALY DETECTED!", FloatingText.FTType.Danger);
        }

        if (Time.time - spawnTime >= spawnInterval && !FindAnomaly(4))
        {
            SpawnMeteor();
            spawnTime = Time.time;
            return;
        }

        if (Time.time - PUSpawnTime >= 30)
        {
            SpawnPowerUp();
            return;
        }

        if (GameControl.gc.GetSceneName().Equals("MainMenu"))
            return; 

        if (!FindAnomaly(5) && !FindAnomaly(3) && !FindAnomaly(4))
        {
            if (GameControl.gc.currentLevel >= 5)
            {
                if (Time.time - HugeAsteroidSpawnTime >= HugeAsteroidInterval)
                {
                    SpawnEnemy(EnemyType.hugeMeteor);
                    return;
                }
            }

            if (GameControl.gc.currentLevel >= 11)
            {
                if (Time.time - NebulaSpawnTime >= NebulaInterval)
                {
                    SpawnEnemy(EnemyType.Nebula);
                    return;
                }
            }

            if (GameControl.gc.currentLevel >= 21)
            {
                if (Time.time - EnemyFighterSpawnTime >= EnemyFighterInterval)
                {
                    SpawnEnemy(EnemyType.Fighter);
                    return;
                }
            }

            if (GameControl.gc.currentLevel >= 31)
            {
                if (Time.time - EnemyMissileCruiserSpawnTime >= EnemyMissileCruiserInterval)
                {
                    SpawnEnemy(EnemyType.MissileCruiser);
                    return;
                }
            }

            if (GameControl.gc.currentLevel >= 41)
            {
                if (Time.time - EnemyBattleShipSpawnTime >= EnemyBattleShipInterval)
                {
                    SpawnEnemy(EnemyType.BattleShip);
                    return;
                }
            }
        }

    }

    public void SpawnEnemy(EnemyType type)
    {
        Transform spawnPosition = transform;
        Vector3 rngpos;

        switch (type)
        {
            case EnemyType.hugeMeteor:
                rngpos = spawnPosition.position;
                rngpos.x = Random.Range(14f, 18f);
                rngpos.y = Random.Range(-3f, 4f);


                spawnPosition.position = rngpos;
                hugeMeteor1.GetComponent<MeteorScript>().asteroidType = AsteroidType.Huge;
                Instantiate(hugeMeteor1, spawnPosition.position, spawnPosition.rotation);
                HugeAsteroidSpawnTime = Time.time;
                break;
            case EnemyType.Nebula:
                rngpos = spawnPosition.position;
                rngpos.x = Random.Range(14f, 18f);
                rngpos.y = Random.Range(-3f, 1f);
                spawnPosition.position = rngpos;
                GameObject nebulaInstance = Instantiate(Nebula, spawnPosition.position, spawnPosition.rotation) as GameObject;
                NebulaSpawnTime = Time.time;
                break;
            case EnemyType.Fighter:
                rngpos = spawnPosition.position;
                rngpos.x = Random.Range(14f, 18f);
                rngpos.y = Random.Range(0f, 4f);
                enemyFighter.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.Fighter;
                spawnPosition.position = rngpos;
                Instantiate(enemyFighter, spawnPosition.position, spawnPosition.rotation);
                EnemyFighterSpawnTime = Time.time;
                break;
            case EnemyType.MissileCruiser:
                rngpos = spawnPosition.position;
                rngpos.x = Random.Range(14f, 18f);
                rngpos.y = Random.Range(0f, 4f);
                enemyMissileCruiser.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.MissileCruiser;
                spawnPosition.position = rngpos;
                Instantiate(enemyMissileCruiser, spawnPosition.position, spawnPosition.rotation);
                EnemyMissileCruiserSpawnTime = Time.time;
                break;
            case EnemyType.BattleShip:
                rngpos = spawnPosition.position;
                rngpos.x = Random.Range(14f, 18f);
                rngpos.y = Random.Range(4f, 5f);
                enemyBattleShip.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.BattleShip;
                spawnPosition.position = rngpos;
                Instantiate(enemyBattleShip, spawnPosition.position, spawnPosition.rotation);
                EnemyBattleShipSpawnTime = Time.time;
                break;
            default:
                break;
        }
    }

    void SpawnMeteor()
    {
        Transform spawnPosition = transform;
        Vector3 rngpos;
        float randomScaleFactor;
        int roll = RollDice(100);
        //Big Asteroid
        if (roll <= bigAsteroidChance)
        {
            rngpos = spawnPosition.position;
            rngpos.x = Random.Range(14f, 18f);
            rngpos.y = Random.Range(-3f, 4f);

            spawnPosition.position = rngpos;
            GameObject bigMeteorInstance = Instantiate(bigMeteor1, spawnPosition.position, spawnPosition.rotation) as GameObject;
            bigMeteorInstance.GetComponent<MeteorScript>().asteroidType = AsteroidType.Big;
            randomScaleFactor = Random.Range(0, 0.5f);
            bigMeteorInstance.GetComponent<Transform>().localScale += new Vector3(randomScaleFactor, randomScaleFactor, 0);
        }

        //Golden asteroid
        else if (roll == goldenAsteroidChance)
        {
            rngpos = spawnPosition.position;
            rngpos.x = Random.Range(14f, 18f);
            rngpos.y = Random.Range(0f, 4f);

            spawnPosition.position = rngpos;
            GameObject goldenMeteorInstance = Instantiate(goldenMeteor, spawnPosition.position, spawnPosition.rotation) as GameObject;
            goldenMeteorInstance.GetComponent<MeteorScript>().asteroidType = AsteroidType.Golden;
        }

        //Medium asteroid
        else
        {
            rngpos = spawnPosition.position;
            rngpos.x = Random.Range(14f, 18f);
            rngpos.y = Random.Range(-3f, 4f);

            spawnPosition.position = rngpos;
            GameObject medMeteorInstance = Instantiate(medMeteor1, spawnPosition.position, spawnPosition.rotation) as GameObject;
            medMeteorInstance.GetComponent<MeteorScript>().asteroidType = AsteroidType.Medium;
            randomScaleFactor = Random.Range(0, 0.5f);
            medMeteorInstance.GetComponent<Transform>().localScale += new Vector3(randomScaleFactor, randomScaleFactor, 0);
        }




        spawnInterval = 1.1f - (GameControl.gc.currentLevel % 10) / 10f;
        if (spawnInterval < 0.3f || GameControl.gc.currentLevel % 10 == 0)
            spawnInterval = 0.3f;
    }

    /*
    private Vector2[] GetSpawnArea(GameObject gObject, Vector3 spawnPos)
    {
        topLeft.x = gObject.GetComponent<Collider2D>().bounds.center.x - gObject.GetComponent<Collider2D>().bounds.extents.x;
        topLeft.y = gObject.GetComponent<Collider2D>().bounds.center.y + gObject.GetComponent<Collider2D>().bounds.extents.y;
        bottomRight.x = gObject.GetComponent<Collider2D>().bounds.center.x + gObject.GetComponent<Collider2D>().bounds.extents.x;
        bottomRight.y = gObject.GetComponent<Collider2D>().bounds.center.y - gObject.GetComponent<Collider2D>().bounds.extents.y;

        topLeft.x += spawnPos.x;
        topLeft.y += spawnPos.y;
        bottomRight.x += spawnPos.x;
        bottomRight.y += spawnPos.y;
        spawnAreaCoordinates[0] = topLeft;
        spawnAreaCoordinates[1] = bottomRight;
        return spawnAreaCoordinates;
    }
    */

    void SpawnPowerUp()
    {
        Transform spawnPosition = transform;
        Vector3 rngpos;
        rngpos = spawnPosition.position;
        rngpos.x = Random.Range(14f, 18f);
        rngpos.y = Random.Range(0f, 4f);
        spawnPosition.position = rngpos;
        Instantiate(powerUpContainer, spawnPosition.position, spawnPosition.rotation);
        PUSpawnTime = Time.time;
    }

    void SpawnAnomaly(int anomalyNumber)
    {
        Transform spawnPosition = transform;
        Vector3 pos;
        pos = spawnPosition.position;
        pos.x = 15f;
        pos.y = 3f;
        spawnPosition.position = pos;
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
