using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    //public Sprite Player;
    public Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, 0, -10); // Camera follows the player but 6 to the right
        //transform.position = new Vector3(player.position.x + 6, 0, -10); // Camera follows the player but 6 to the right
    }
}
