using UnityEngine;

public class NebulaScript : MonoBehaviour {

    private float movementSpeed = -0.005f;
    private int consumedObjects = 0;
    public int hitPoints = 150;
    public int XP = 50;
    private Camera mainCamera;
    private GameObject asteroidFragment;
    private int dmgCounter = 0;
    private bool ALIVE = true;
    private Vector3 preferredScale;
    private GameObject floatingText;
    private bool iscrit;
    private float damageStacks;
    public Transform trans;

    // Use this for initialization
    void Start () {
        trans = transform;
        floatingText = GameControl.gc.floatingText;
        mainCamera = Camera.main;
        preferredScale = transform.localScale;
        hitPoints *= GameControl.gc.currentLevel;
    }
	
	// Update is called once per frame
	void Update () {
        //Destroy if "out of bounds"
        if (trans.position.x > 19 || trans.position.x < -19 || trans.position.y < -7 || trans.position.y > 7)
            Destroy(gameObject);

        Vector3 newVector;
        newVector = trans.position;
        newVector.x += movementSpeed;
        trans.position = newVector;
        GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward * 0.2f);
        //counter++;
        if (!ALIVE)
        {
            Vector3 newScale = trans.localScale;
            newScale.x -= 0.005f;
            newScale.y -= 0.005f;
            trans.localScale = newScale;
            if (trans.localScale.x < 0.05)
                Destroy(gameObject);
            return;
        }
        else if (trans.localScale.x < preferredScale.x)
        {
            Vector3 newScale = trans.localScale;
            newScale.x += 0.005f;
            newScale.y += 0.005f;
            trans.localScale = newScale;
        }


    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!ALIVE)
            return;
        if (col.gameObject.GetComponent<PlayerProjectileScript>() != null)
        {
            if (col.gameObject.GetComponent<PlayerProjectileScript>().Critical)
                iscrit = true;
            else
                iscrit = false;

            if (col.gameObject.GetComponent<PlayerProjectileScript>().damageAccumulation > 0)
            {
                damageStacks += 1;
            }

            isHit(col.gameObject.GetComponent<PlayerProjectileScript>().damage, true);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {

        if (col.GetComponent<ShipHullScript>() != null)
        {
            //Debug.Log("Ship hull taking damage!");
            dmgCounter++;
            if (dmgCounter % 10 == 0)
            {
                col.GetComponent<ShipHullScript>().isHit(1);
            }
        }

        if (!IsOnScreen())
            return;

        if (!ALIVE)
            return;

        if (col.GetComponent<MeteorScript>() != null)
        {
            {
                //if (counter % 10 == 0)
                //{
                col.attachedRigidbody.velocity = (trans.position - col.transform.position).normalized;
                Vector3 newScale = col.transform.localScale;
                newScale.x -= 0.02f;
                newScale.y -= 0.02f;

                col.transform.localScale -= newScale;
                //}
                if (col.transform.localScale.x < 0.1)
                {
                    Consume(col.gameObject, col.GetComponent<MeteorScript>().hitPoints);
                }
            }
        }

        


    }

    public void isHit(int incomingDamage,  bool showDmg)
    {
        if (damageStacks > 0)
        {
            float newDamage = (float)incomingDamage * ( 1 + damageStacks * GameControl.gc.Weapons[2].DamageAccumulation);
            incomingDamage = (int)newDamage;
        }
        hitPoints -= incomingDamage;
        if (hitPoints <= 0)
        {
            Explode();
        }
        if (showDmg)
            DamageText(iscrit, incomingDamage);
    }

    private void Consume(GameObject gameobj, int hp)
    {
        hitPoints += 3 * hp;
        Destroy(gameobj);
        if (preferredScale.x < 1f)
        {
            preferredScale.x += 0.05f;
            preferredScale.y += 0.05f;
        }
        consumedObjects++;
    }

    private void Explode()
    {
        if (!ALIVE)
            return;
        GameControl.gc.ExperienceGained(XP);
        asteroidFragment = Resources.Load("ScrapPiece") as GameObject;
        for (int i = 0; i <= consumedObjects; i++)
        {
            Vector3 rngpos = trans.position;
            rngpos.x += Random.Range(-1f, 1f);
            rngpos.y += Random.Range(-1f, 1f);
            GameObject fragmentInstance = Instantiate(asteroidFragment, rngpos, trans.rotation) as GameObject;
            fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.Normal;
            if (Random.Range(1, 1001) >= 1000 - GameControl.gc.currentLevel / 10)
            {
                fragmentInstance.GetComponent<ScrapPieceScript>().type = Scrap.ScrapType.ResearchMaterial;
            }
        }

        ALIVE = false;
        
        
    }

    private bool IsOnScreen()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(trans.position);
        if (screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < 1 && screenPoint.y < 1)
            return true;
        else
            return false;
    }

    private void DamageText(bool CRITICAL, int dmg)
    {
        GameObject ft;
        ft = Instantiate(floatingText, trans.position, Quaternion.identity) as GameObject;
        ft.GetComponent<FloatingTextScript>().text = dmg.ToString();
        ft.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.PopUp;
        if (CRITICAL)
        {
            ft.GetComponent<TextMesh>().fontSize = 50;
            ft.GetComponent<TextMesh>().color = Color.yellow;
        }
    }
}
