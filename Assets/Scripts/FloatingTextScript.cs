using UnityEngine;
using FloatingText;
using System.Collections;

public class FloatingTextScript : MonoBehaviour {


    public string SortingLayerName = "OnTop";
    public int SortingOrder = 0;
    public FTType fttype = FTType.Normal;
    public string text = "EMPTY";
    private Color newColor;
    private bool finished = false;
    public float spawnTime;
    public float durationBeforeFading = 4f;
    //Default font size is 30

	void Start () {
        GetComponent<TextMesh>().text = text;
        newColor = GetComponent<TextMesh>().color;
        spawnTime = Time.time;
        switch (fttype)
        {
            case FTType.PopUp:
                newColor.a = 1f;
                break;
            case FTType.Announcement:
                newColor.a = 0f;
                transform.position += new Vector3(0f, -2f, 0);
                break;
            case FTType.PowerUp:
                newColor.a = 1f;
                break;
            case FTType.Danger:
                newColor = Color.red;
                GetComponent<TextMesh>().color = newColor;
                GetComponent<TextMesh>().fontSize = 50;
                GetComponent<TextMesh>().text = "!" + GetComponent<TextMesh>().text + "!";
                break;
            default:
                break;
        }
        GetComponent<MeshRenderer>().sortingLayerName = SortingLayerName;
        GetComponent<MeshRenderer>().sortingOrder = SortingOrder;

    }
	
	void Update ()
    {
        switch (fttype)

        {
            case FTType.Normal:
                transform.position += new Vector3(0f, 0.01f, 0);
                if (Time.time - spawnTime > durationBeforeFading)
                {
                    Destroy(gameObject);
                }
                break;
            case FTType.PopUp:
                newColor.a -= 0.0075f;
                if (GetComponent<TextMesh>().fontSize > 1)
                    GetComponent<TextMesh>().fontSize--;
                if (newColor.a <= 0 || GetComponent<TextMesh>().fontSize == 1)
                    Destroy(gameObject);
                GetComponent<TextMesh>().color = newColor;
                break;

            case FTType.Announcement:
                transform.position += new Vector3(0f, 0.01f, 0);
                if (!finished)
                {
                    newColor.a += 0.005f;
                    if (newColor.a >= 1)
                        finished = true;
                }
                if (finished)
                {
                    newColor.a -= 0.005f;
                    if (newColor.a <= 0 && finished)
                        Destroy(gameObject);
                }
                GetComponent<TextMesh>().color = newColor;
                break;

            case FTType.PowerUp:
                if (Time.time - spawnTime > durationBeforeFading)
                {
                    Destroy(gameObject);
                }
                break;

            case FTType.Danger:
                if (GetComponent<TextMesh>().fontSize > 30)
                {
                    GetComponent<TextMesh>().fontSize--;
                }
                if (Time.time - spawnTime > durationBeforeFading)
                {
                    Destroy(gameObject);
                }
                break;


            default:
                break;
        }
            



	
	}
}
