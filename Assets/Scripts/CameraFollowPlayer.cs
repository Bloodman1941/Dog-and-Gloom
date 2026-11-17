using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            return;
        }

        // Lock camera to player position, keeping Z at -10
        transform.position = new Vector3(player.position.x, player.position.y, -10f);
    }
}
