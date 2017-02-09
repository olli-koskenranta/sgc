using UnityEngine;
using System.Collections;

public class NebulaScript : MonoBehaviour {

    private float movementSpeed = -0.005f;
    private int consumedObjects = 0;
    public int hitPoints = 500;
    public int XP = 50;
    private Camera mainCamera;
    private GameObject asteroidFragment;
    private int dmgCounter = 0;
    private bool ALIVE = true;
    private Vector3 preferredScale;
	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;
        preferredScale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
        //Destroy if "out of bounds"
        if (gameObject.transform.position.x > 19 || gameObject.transform.position.x < -9 || gameObject.transform.position.y < -7 || gameObject.transform.position.y > 7)
            Destroy(gameObject);

        transform.position += new Vector3(movementSpeed, 0, 0);
        GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 0.2f);
        //counter++;
        if (!ALIVE)
        {
            transform.localScale -= new Vector3(0.01f, 0.01f, 0);
            if (transform.localScale.x < 0.05)
                Destroy(gameObject);
            return;
        }
        else if (transform.localScale.x < preferredScale.x)
        {
            transform.localScale += new Vector3(0.01f, 0.01f, 0);
        }


    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!ALIVE)
            return;
        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (!IsOnScreen())
            return;

        if (!ALIVE)
            return;

        if (col.GetComponent<MeteorScript>() != null)
        {
            {
                //if (counter % 10 == 0)
                //{
                col.attachedRigidbody.velocity = (transform.position - col.transform.position).normalized;
                col.GetComponent<Transform>().localScale -= new Vector3(0.02f, 0.02f, 0f);
                //}
                if (col.GetComponent<Transform>().localScale.x < 0.1)
                {
                    Consume(col.gameObject, col.GetComponent<MeteorScript>().hitPoints);
                }
            }
        }

        if (col.GetComponent<ShipHullScript>() != null)
        {
            //Debug.Log("Ship hull taking damage!");
            dmgCounter++;
            if (dmgCounter % 10 == 0)
            {
                col.GetComponent<ShipHullScript>().isHit(1);
            }
        }


    }

    void OnCollisionStay2D(Collision2D col)
    {
        
    }

    void isHit(int Damage)
    {

        hitPoints -= Damage;
        if (hitPoints <= 0)
        {
            Explode();
        }
    }

    private void Consume(GameObject gameobj, int hp)
    {
        hitPoints += 2 * hp;
        Destroy(gameobj);
        if (preferredScale.x < 1f)
        {
            preferredScale += new Vector3(0.1f, 0.1f, 0);
        }
        consumedObjects++;
    }

    private void Explode()
    {
        if (!ALIVE)
            return;
        GameControlScript.gameControl.ExperienceGained(XP);
        asteroidFragment = Resources.Load("ScrapPiece") as GameObject;
        for (int i = 0; i <= consumedObjects; i++)
        {
            Vector3 rngpos = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 0.5f), 0f);
            GameObject fragmentInstance = Instantiate(asteroidFragment, this.transform.position + rngpos, this.transform.rotation) as GameObject;
            fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.Normal;
            if (Random.Range(1, 1001) >= 1000 - GameControlScript.gameControl.currentLevel / 10)
            {
                fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.ResearchMaterial;
            }
        }

        ALIVE = false;
        
        
    }

    private bool IsOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(gameObject.transform.position);
        if (screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1)
            return true;
        else
            return false;
    }
}
