using UnityEngine;
using UnityEngine.UI;
using FloatingText;

public class FloatingTextScript : MonoBehaviour {


    public string SortingLayerName = "OnTop";
    public int SortingOrder = 1;
    public FTType fttype = FTType.Normal;
    public string text = "EMPTY";
    private Color newColor;
    private bool finished = false;
    public float spawnTime;
    public float durationBeforeFading = 4f;
    private Transform trans;
    public bool isCrit = false;
    public bool KILL_ME = false;

    void Start()
    {
        OnEnable();
    }
	void OnEnable ()
    {
        //Invoke("EndLife", 2);
        trans = transform;
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
                GetComponent <TextMesh>().color = newColor;
                Vector3 posVector = trans.position;
                posVector.y -= 2;
                trans.position = posVector;
                break;
            case FTType.PowerUp:
                newColor.a = 1f;
                break;
            case FTType.Danger:
                newColor = Color.red;
                GetComponent<TextMesh>().color = newColor;
                GetComponent<TextMesh>().fontSize = 30;
                GetComponent<TextMesh>().text = GetComponent<TextMesh>().text;
                Vector3 newScale = trans.localScale;
                newScale.x += 0.1f;
                newScale.y += 0.1f;
                trans.localScale = newScale;
                break;
            default:
                break;
        }
        if (isCrit)
            GetComponent<TextMesh>().fontSize = 50;

        GetComponent<MeshRenderer>().sortingLayerName = SortingLayerName;
        GetComponent<MeshRenderer>().sortingOrder = SortingOrder;

    }

    void FixedUpdate()
    {
        //return;
        switch (fttype)

        {
            case FTType.Normal:
                Vector3 moveVector = trans.position;
                moveVector.y += 0.01f;
                trans.position = moveVector;
                if (Time.time - spawnTime > durationBeforeFading)
                {
                    EndLife();
                }
                break;
            case FTType.PopUp:
                if (trans.localScale.x > 0.05f)
                {
                    Vector3 newScale = trans.localScale;
                    newScale.x -= 0.005f;
                    newScale.y -= 0.005f;
                    trans.localScale = newScale;
                }
                else
                    EndLife();
                break;

            case FTType.Announcement:
                Vector3 moveVector2 = trans.position;
                moveVector2.y += 0.01f;
                trans.position = moveVector2;
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
                        EndLife();
                }
                GetComponent<TextMesh>().color = newColor;
                break;

            case FTType.PowerUp:
                if (Time.time - spawnTime > durationBeforeFading)
                {
                    EndLife();
                }
                break;

            case FTType.Danger:
                if (trans.localScale.x > 0.2f)
                {
                    Vector3 newScale = trans.localScale;
                    newScale.x -= 0.005f;
                    newScale.y -= 0.005f;
                    trans.localScale = newScale;
                }
                if (Time.time - spawnTime > durationBeforeFading)
                {
                    EndLife();
                }
                break;


            default:
                break;
        }
    }

    private void EndLife()
    {
        if (KILL_ME)
            Destroy(gameObject);
        GetComponent<TextMesh>().color = Color.white;
        GetComponent<TextMesh>().fontSize = 30;
        newColor = GetComponent<TextMesh>().color;
        Vector3 newScale = trans.localScale;
        newScale.x = 0.2f;
        newScale.y = 0.2f;
        trans.localScale = newScale;
        isCrit = false;
        gameObject.SetActive(false);
    }
}
