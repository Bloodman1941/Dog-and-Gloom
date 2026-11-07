using UnityEngine;

public class SwordFX_Controller : MonoBehaviour
{

       float speed = 5f;
       float lifetime = 0.1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void moveForward(string name) {
        switch (name) { 
            //Up
            case "up(clone)":
                transform.Translate(Vector2.up * speed * Time.deltaTime);
                break;
            //Down
            case "down(clone)":
                transform.Translate(Vector2.down * speed * Time.deltaTime);
                break;
            //Right
            case "right(clone)":
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                break;
            //Left
            case "left(clone)":
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                break;
            case "upright(clone)":
                transform.Translate(Vector2.up * speed * Time.deltaTime);
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                break;
            case "upleft(clone)":
                transform.Translate(Vector2.up * speed * Time.deltaTime);
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                break;
            case "downright(clone)":
                transform.Translate(Vector2.down * speed * Time.deltaTime);
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                break;
            case "downleft(clone)":
                transform.Translate(Vector2.down * speed * Time.deltaTime);
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log(gameObject.name.ToLower());
        moveForward(gameObject.name.ToLower());
    }
}
