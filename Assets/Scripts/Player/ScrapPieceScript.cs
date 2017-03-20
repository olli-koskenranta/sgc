using UnityEngine;
using Scrap;
using System.Collections;

public class ScrapPieceScript : MonoBehaviour {

    public int scrapAmount;
    public int researchMaterialAmount;
    public ScrapType type = ScrapType.Normal;

	// Use this for initialization
	void Start () {
        scrapAmount = 3 + GameControl.gc.currentLevel * 3;
        if (GameControl.gc.ScrapBoostActive)
            scrapAmount *= 2;
        researchMaterialAmount = 1;
        if (type == ScrapType.ResearchMaterial)
            GetComponent<SpriteRenderer>().color = Color.cyan;
    }
	
    void FixedUpdate()
    {
        //Destroy gameobject if "out of bounds"
        if (gameObject.transform.position.x > 19 || gameObject.transform.position.x < -10 || gameObject.transform.position.y < -7 || gameObject.transform.position.y > 7)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Collector"))
        {
            
            GameObject floatingText = ObjectPool.pool.GetPooledObject(GameControl.gc.floatingText, 1);
            if (type == ScrapType.ResearchMaterial)
            {
                floatingText.GetComponent<TextMesh>().color = Color.cyan;
                floatingText.GetComponent<FloatingTextScript>().text = "+" + researchMaterialAmount.ToString();
            }
            else
            {
                floatingText.GetComponent<FloatingTextScript>().text = "+" + scrapAmount.ToString();
            }
            floatingText.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.PopUp;
            floatingText.transform.position = transform.position;
            floatingText.SetActive(true);
            Destroy(this.gameObject);
        }
    }

}
