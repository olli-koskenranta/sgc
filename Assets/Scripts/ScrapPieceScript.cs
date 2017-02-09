using UnityEngine;
using Scrap;
using System.Collections;

public class ScrapPieceScript : MonoBehaviour {

    public int scrapAmount;
    public int researchMaterialAmount;
    public ScrapType type = ScrapType.Normal;

	// Use this for initialization
	void Start () {
        scrapAmount = 3 + GameControlScript.gameControl.currentLevel * 3;
        researchMaterialAmount = 1;
        if (type == ScrapType.ResearchMaterial)
            GetComponent<SpriteRenderer>().color = Color.cyan;
    }
	
	// Update is called once per frame
	void Update () {

        //Destroy gameobject if "out of bounds"
        if (gameObject.transform.position.x > 19 || gameObject.transform.position.x < -10 || gameObject.transform.position.y < -7 || gameObject.transform.position.y > 7 )
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Collector"))
        {
            GameObject ft;
            GameObject floatingText = Resources.Load("FloatingText") as GameObject;
            ft = Instantiate(floatingText, transform.position, Quaternion.identity) as GameObject;
            if (type == ScrapType.ResearchMaterial)
            {
                ft.GetComponent<FloatingTextScript>().text = "+" + researchMaterialAmount.ToString();
                ft.GetComponent<TextMesh>().color = Color.cyan;
            }
            else
            {
                ft.GetComponent<FloatingTextScript>().text = "+" + scrapAmount.ToString();
            }
            ft.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.PopUp;
            Destroy(this.gameObject);
        }
        /*
        switch (col.gameObject.tag)
        {
            case "Collector":
                GameObject ft;
                GameObject floatingText = Resources.Load("FloatingText") as GameObject;
                ft = Instantiate(floatingText, transform.position, Quaternion.identity) as GameObject;
                ft.GetComponent<FloatingTextScript>().text = "+" + scrapAmount.ToString();
                ft.GetComponent<FloatingTextScript>().fttype = FloatingText.FTType.Damage;
                Destroy(this.gameObject);
                break;

            default:
                break;

        }
        */
    }

}
