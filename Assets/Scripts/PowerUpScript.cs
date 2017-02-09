using UnityEngine;
using System.Collections;

public class PowerUpScript : MonoBehaviour {

    float speed;
    public int hitPoints = 3;
    public GameObject powerUp;
    public AudioSource soundBoom;
    public Sprite hit1, hit2;
    private Camera mainCamera;

    void Start()
    {
        speed = 1f;
        GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector3.left * speed);
        mainCamera = Camera.main;
    }

    void Update()
    {

        //Destroy if "out of bounds"
        if (gameObject.transform.position.x > 19 || gameObject.transform.position.x < -19 || gameObject.transform.position.y < -7 || gameObject.transform.position.y > 7)
            Destroy(this.gameObject);
        GetComponent<SpriteRenderer>().transform.Rotate(Vector3.forward);
    }

    void OnCollisionEnter2D(Collision2D col)
    {

        if (!IsOnScreen())
            return;

        switch (col.gameObject.tag)
        {
            case "Bullet":
                isHit();
                break;

            case "Bullet2":
                isHit();
                break;

            default:
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (!IsOnScreen())
            return;

        switch (col.gameObject.tag)
        {
            case "Bullet":
                isHit();
                break;

            case "Bullet2":
                isHit();
                break;

            default:
                break;
        }
    }



    void isHit()
    {
        hitPoints--;

        switch (hitPoints)
        {
            case 2:
                gameObject.GetComponent<SpriteRenderer>().sprite = hit1;
                break;
            case 1:
                gameObject.GetComponent<SpriteRenderer>().sprite = hit2;
                break;
            default:
                break;
        }
        
        if (hitPoints <= 0)
            Explode();
    }

    void Explode()
    {
        if (GameControlScript.gameControl.AUDIO_SOUNDS)
            soundBoom.Play();

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        Destroy(this.gameObject, 4);
        Instantiate(powerUp, this.transform.position, this.transform.rotation);
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
