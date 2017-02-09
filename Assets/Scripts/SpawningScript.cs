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

    public GameObject anomaly1;
    public GameObject anomaly2;
    public GameObject anomaly3;

    private float PUSpawnTime;

    private int PUChance;
    private int bigAsteroidChance;
    private int HugeAsteroidChance;
    private int EnemyFighterChance;
    private int EnemyMissileCruiserChance;
    private int NebulaChance;

    public bool ANOMALY_SPAWNED = false;
    public bool[] ANOMALY_DESTROYED;

    void Start () {

        GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(false);
        ANOMALY_DESTROYED = new bool[3];
        for (int i = 0; i < ANOMALY_DESTROYED.Length; i++)
            ANOMALY_DESTROYED[i] = false;

        //Define spawn chances here (%), maybe some increase per level?
        PUChance = 1;
        bigAsteroidChance = 30;
        HugeAsteroidChance = 1;
        NebulaChance = 1;
        EnemyFighterChance = 1;
        EnemyMissileCruiserChance = 1;
        

        PUSpawnTime = 0;
        medMeteor1 = Resources.Load("medMeteor1") as GameObject;
        bigMeteor1 = Resources.Load("bigmeteor1") as GameObject;
        hugeMeteor1 = Resources.Load("hugeMeteor1") as GameObject;
        powerUpContainer = Resources.Load("PowerUpContainer") as GameObject;
        anomaly1 = Resources.Load("Anomaly1") as GameObject;
        anomaly3 = Resources.Load("Carrier") as GameObject;
        enemyFighter = Resources.Load("AlienShip1") as GameObject;
        enemyMissileCruiser = Resources.Load("MissileCruiser") as GameObject;
        Nebula = Resources.Load("Nebula") as GameObject;

	}
	
	void Update () {

        if (Time.time - spawnTime >= spawnInterval)
        {
            SpawnMeteor();
        }

        /*if (Time.time - PUSpawnTime > 60)
        {
            SpawnPowerUp();
        }*/

        if (GameControlScript.gameControl.currentLevel == 10 && !ANOMALY_SPAWNED && !ANOMALY_DESTROYED[0])
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(1);
            announcer.GetComponent<AnnouncerScript>().Announce("ANOMALY DETECTED", FloatingText.FTType.Danger);
        }

        if (GameControlScript.gameControl.currentLevel == 20 && !ANOMALY_SPAWNED && !ANOMALY_DESTROYED[1])
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(2);
            announcer.GetComponent<AnnouncerScript>().Announce("ANOMALY DETECTED", FloatingText.FTType.Danger);
        }

        if (GameControlScript.gameControl.currentLevel == 30 && !ANOMALY_SPAWNED && !ANOMALY_DESTROYED[2])
        {
            GameObject.Find("UIControl").GetComponent<UIControlScript>().SetBossBarsActive(true);
            SpawnAnomaly(3);
            announcer.GetComponent<AnnouncerScript>().Announce("ANOMALY DETECTED", FloatingText.FTType.Danger);
        }

    }

    void SpawnMeteor()
    {
        Transform spawnPosition = transform;
        float randomScaleFactor;

        if (RollDice(100) <= PUChance )
            SpawnPowerUp();


        //Huge Asteroid
        if (GameControlScript.gameControl.currentLevel >= 1)
        {
            if (RollDice(100) <= HugeAsteroidChance)
            {
                spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(-3f, 4f), 0f);
                hugeMeteor1.GetComponent<MeteorScript>().asteroidType = AsteroidType.Huge;
                Instantiate(hugeMeteor1, spawnPosition.position, spawnPosition.rotation);
            }
        }

        //Big Asteroid
        if (RollDice(100) <= bigAsteroidChance)
        {
            spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(-3f, 4f), 0f);
            GameObject bigMeteorInstance = Instantiate(bigMeteor1, spawnPosition.position, spawnPosition.rotation) as GameObject;
            bigMeteorInstance.GetComponent<MeteorScript>().asteroidType = AsteroidType.Big;
            randomScaleFactor = Random.Range(0, 0.5f);
            bigMeteorInstance.GetComponent<Transform>().localScale += new Vector3(randomScaleFactor, randomScaleFactor, 0);
            
            
            
        }

        //Nebula
        if (GameControlScript.gameControl.currentLevel >= 11)
        {
            if (RollDice(100) <= NebulaChance)
            {
                spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(-3f, 4f), 0f);
                GameObject nebulaInstance = Instantiate(Nebula, spawnPosition.position, spawnPosition.rotation) as GameObject;
            }
        }

        if (GameControlScript.gameControl.currentLevel >= 21)
        {
            //Enemy Fighter
            if (RollDice(100) <= EnemyFighterChance)
            {
                
                enemyFighter.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.Fighter;
                spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(0f, 4f), 0f);
                Instantiate(enemyFighter, spawnPosition.position, spawnPosition.rotation);
                

            }
        }

        //Enemy Missile Cruiser
        if (GameControlScript.gameControl.currentLevel >= 31)
        if (RollDice(100) <= EnemyMissileCruiserChance)
        {

            enemyMissileCruiser.GetComponent<EnemyShipScript>().sType = EnemyShipScript.ShipType.MissileCruiser;
            spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(0f, 4f), 0f);
            Instantiate(enemyMissileCruiser, spawnPosition.position, spawnPosition.rotation);

        }

        //Always spawn medium asteroid
        spawnPosition.position = new Vector3(Random.Range(14f, 18f), Random.Range(-3f, 4f), 0f);
        GameObject medMeteorInstance = Instantiate(medMeteor1, spawnPosition.position, spawnPosition.rotation) as GameObject;
        medMeteorInstance.GetComponent<MeteorScript>().asteroidType = AsteroidType.Medium;
        randomScaleFactor = Random.Range(0, 0.5f);
        medMeteorInstance.GetComponent<Transform>().localScale += new Vector3(randomScaleFactor, randomScaleFactor, 0);


        spawnTime = Time.time;

        spawnInterval = 1.1f - (GameControlScript.gameControl.currentLevel % 10) / 10f;
        if (spawnInterval < 0.3f || GameControlScript.gameControl.currentLevel % 10 == 0)
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
        switch (anomalyNumber)
        {
            case 1:
                //anomaly1.GetComponent<Anomaly1Script>().hitPoints = 1000;
                Instantiate(anomaly1, spawnPosition.position, spawnPosition.rotation);
                break;
            case 2:
                Instantiate(anomaly2, spawnPosition.position, spawnPosition.rotation);
                break;
            case 3:
                Instantiate(anomaly3, spawnPosition.position, spawnPosition.rotation);
                break;
            default:
                break;
        }

        ANOMALY_SPAWNED = true;

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
