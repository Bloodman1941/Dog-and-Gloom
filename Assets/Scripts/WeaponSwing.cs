using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class WeaponSwing : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator animator;
    Vector2 position = new Vector2(1f, 2f);
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerRenderer;

    public GameObject slashEffectSmallTopLeft;
    public GameObject slashEffectLargeDownRight;
    public GameObject slashEffectLargeUpLeft;
    public GameObject slashEffectSmallUp;
    public GameObject slashEffectSmallDown;

    public Transform slashSpawnPoint;

    private Vector2 lastMoveDirection;


    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float rotationIntensity = 90f;
    [SerializeField] private float pushSword = 0.5f;
    [SerializeField] private float circleRadiusX = 1;
    [SerializeField] private float circleRadiusY = 2;

    [SerializeField] private float stepDistance = 18f;
    [SerializeField] private float swingAngle = 180f; // Total arc of the swing
    [SerializeField] private float swingDuration = 0.15f; // Duration of swing in seconds
    [SerializeField] private Vector3 posOffset = new Vector3(0, 10f, 0); // Y offset of 10
    [SerializeField] private float setAngleOffset = 180f; // Y offset of 10
    [SerializeField] private float swordGravityStart = 0.1f;
    [SerializeField] private float swordGravitySpeed = 0.1f;

    private bool isSwinging = false;

    float baseAngle = -43.1f;
    string swingDirection = "None";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame

    private float PointToMouse() {
        /*
        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 2. Calculate the direction from the object to the mouse
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );

        // 3. Set the object's right (X-axis) vector to point in that direction
        transform.right = direction;
        return direction;*/

        Vector3 mouseScreenPos = Input.mousePosition;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 direction = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        return angle;
    }

    private void Rotate90(Vector2 direction) {
        Vector2 offsetDirection = new Vector2(direction.y, -direction.x);
    }



    private void GoToPlayer() {
        Vector3 mouseVector = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float mouseX = mouseVector.x;
        float mouseY = mouseVector.y;
        float playerX = playerTransform.position.x;
        float playerY = playerTransform.position.y;


        // Go Behind Player

        //Go to player and move down
        transform.position = playerTransform.position;

        //Change y
        //transform.position = new Vector2(position.x, position.y+1);

        //Point Towards Mouse
        float direction = PointToMouse();

        // Rotate -90
        //Rotate90(direction);
        transform.Rotate(0, 0, rotationIntensity);

        //Move Forward 18
        transform.Translate(Vector3.right * pushSword, Space.Self);

        //Rotate Back
        //Rotate90(direction)
        transform.Rotate(0, 0, rotationIntensity);
        transform.Rotate(0, 0, (rotationIntensity * 2));

        //UnityEngine.Debug.Log(weaponX + " and a " + weaponY + " and direction: " + swingDirection);
        //transform.position = new Vector2(playerX, playerY);
        //PointToMouse(mouseVector);


    }


GameObject ChooseDirection() {
        float maxX = 0.0f;
        float maxY = 0.0f;
        float  weaponX = transform.position.x - playerTransform.position.x;
        float  weaponY = transform.position.y - playerTransform.position.y;
        bool CanPointRight = weaponX > maxX;
        bool CanPointUp = weaponY > maxY;
        
        UnityEngine.Debug.Log(weaponX + " and is " + weaponY);

        if (CanPointRight && CanPointUp) {
            // Point UP Right
            return slashEffectLargeDownRight;


        }
        else if (!CanPointRight && CanPointUp) {
            //Point UP Left
            return slashEffectSmallUp;

        }
        else if (CanPointRight && !CanPointUp) {
            // Point DOWN Right
            return slashEffectSmallDown;
        }
        else {
            //Point DOWN Left
            return slashEffectLargeUpLeft;
        
        }
    }


void PlaySlashAnimation(){
        GameObject prefabToSpawn = slashEffectLargeDownRight;
        prefabToSpawn = ChooseDirection();
        Destroy(Instantiate(prefabToSpawn, slashSpawnPoint.position, Quaternion.identity), 0.3f);
    

}




    void Update()
    {

        GoToPlayer();
        if (Input.GetMouseButton(0) && !isSwinging)
        {
            StartCoroutine(SwingSword());
        }
        if (!Input.GetMouseButton(0)) {
            isSwinging = false;
        }




    }


    private IEnumerator SwingSword()
    {
        isSwinging = true;
        PlaySlashAnimation();

        float elapsed = 0f;
        // Start the swing 90 degrees IN FRONT, lerp to 90 degrees BEHIND
        float startAngleOffset = swingAngle / 2f;
        float endAngleOffset = -swingAngle / 2f;

        // Cache original alignment offset calculated in step 5/7
        float baseScratchOffset = setAngleOffset;
        float swordGravity = swordGravityStart;



        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / swingDuration;
            swordGravity += swordGravitySpeed;


            // Calculate target position and direction toward mouse during current frame
            Vector3 basePosition = playerTransform.position + posOffset;
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPos - basePosition;
            float mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Interpolate current angle in the opposite direction
            float currentSwingOffset = Mathf.Lerp(startAngleOffset, endAngleOffset, progress);
            float currentTotalAngle = mouseAngle + baseScratchOffset + currentSwingOffset + swordGravity;

            // Apply rotation and offset position smoothly
            transform.rotation = Quaternion.Euler(0, 0, currentTotalAngle);
            transform.position = basePosition + (transform.right * stepDistance);

            yield return null; // Wait for next frame
        }

        elapsed = 0f;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / swingDuration;
            swordGravity += swordGravitySpeed * -1;

            // Calculate target position and direction toward mouse during current frame
            Vector3 basePosition = playerTransform.position + posOffset;
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPos - basePosition;
            float mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Interpolate current angle in the opposite direction
            float currentSwingOffset = Mathf.Lerp(endAngleOffset, startAngleOffset, progress);
            float currentTotalAngle = mouseAngle + baseScratchOffset + currentSwingOffset + swordGravity;

            // Apply rotation and offset position smoothly
            transform.rotation = Quaternion.Euler(0, 0, currentTotalAngle);
            transform.position = basePosition + (transform.right * stepDistance);

            yield return null; // Wait for next frame
        }



        //isSwinging = false;
    }


}


