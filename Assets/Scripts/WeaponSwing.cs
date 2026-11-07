using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
//using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WeaponSwing : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator animator;
    Vector2 position = new Vector2(1f, 2f);
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerRenderer;

    public GameObject slashUp;
    public GameObject slashUpRight;
    public GameObject slashRight;
    public GameObject slashDownRight;
    public GameObject slashDown;
    public GameObject slashDownLeft;
    public GameObject slashLeft;
    public GameObject slashUpLeft;

    

    public UnityEngine.Transform slashSpawnPoint;

    private Vector2 lastMoveDirection;

    private bool isSwinging = false;
    private bool mouseDown = false;
    public bool startSpecial = false;
    private float mouseTimer = 0f;
    public bool enableTrail = false;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private UnityEngine.Transform playerTransform;
    [SerializeField] private float rotationIntensity = 90f;
    [SerializeField] private float pushSword = 0.5f;
    [SerializeField] private float circleRadiusX = 1;
    [SerializeField] private float circleRadiusY = 2;

    [SerializeField] private float stepDistance = 18f;
    [SerializeField] private float swingAngle = 180f; // Total arc of the swing
    [SerializeField] private float swingDuration = 0.15f; // Duration of swing in seconds
    [SerializeField] private Vector3 posOffset = new Vector3(0, 10f, 0); // Y offset of 10
    [SerializeField] private double offsetAngle = 100;
    [SerializeField] private float setAngleOffset = 180f; // Y offset of 10
    [SerializeField] private float swordGravityStart = 0.1f;
    [SerializeField] private float swordGravitySpeed = 0.1f;
    

    

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


    public (double, double) CalculatePosition(float mouseX = 0, float mouseY = 0, float playerX = 0, float playerY = 0) { 
         

      // Go Behind Player

        //Go to player and move down
        //transform.position = playerTransform.position;
        float  dx = mouseX - playerX;
        float dy = mouseY - playerY;
        double angle_radians = Math.Atan2(dy, dx);
        //double offsetAngle = 100;
        double offset_angle_radians = angle_radians + offsetAngle*(Math.PI - 2.0);
        double angle_degrees = angle_radians * (180 / Math.PI);

        //Change y
        //transform.position = new Vector2(position.x, position.y+1);

        //Point Towards Mouse
        //float direction = PointToMouse();

        // Rotate -90
        //Rotate90(direction);
        

        //Move Forward 0.5
        //transform.Translate(Vector3.right * pushSword, Space.Self);
        double targetX = playerX + (pushSword * Math.Cos(offset_angle_radians));
        double targetY = playerY + (pushSword * Math.Sin(offset_angle_radians));
        return (targetX,targetY);
    }

    private void SwapLayers(float weaponY, float playerY) {

        if (weaponY > playerY) {
            spriteRenderer.sortingOrder = -1;
        }
        else
        {
            spriteRenderer.sortingOrder = 2;
        }
    
    }

    private void GoToPlayer() {
        Vector3 mouseVector = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float mouseX = mouseVector.x;
        float mouseY = mouseVector.y; 
        float playerX = playerTransform.position.x;
        float playerY = playerTransform.position.y;

        float direction = PointToMouse();
        transform.Rotate(0, 0, rotationIntensity);

        var (targetX,targetY) = CalculatePosition(mouseX,mouseY,playerX,playerY);
        SwapLayers((float)targetY, playerY);
        //UnityEngine.Debug.Log("Expected X: "+targetX + " Expected Y:" + targetY);
        transform.position = new Vector2((float)targetX, (float)targetY);



        

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



        //bool CanPointRight = weaponX > maxX;
        //bool CanPointUp = weaponY > maxY;
        bool xOverLow = -0.3f > weaponX;
        bool xOverMed = 0.0f > weaponX;
        bool xOverHigh = 0.3f > weaponX;
        bool yOverLow = -0.3f > weaponY;
        bool yOverMed = 0.0f > weaponY;
        bool yOverHigh = 0.3f > weaponY;



        //UnityEngine.Debug.Log(weaponX + " and is " + weaponY);
        if (xOverLow && yOverMed)
        {
            // Up from Left
            return slashUp;
        }
        else if (xOverLow && !yOverMed)
        {
            // Up Right from Up Left
            return slashUpRight;
        }
        else if (xOverMed && !yOverHigh)
        {
            // Right from Up
            return slashRight;
        }
        else if (!xOverMed && !yOverMed)
        {
            // Down Right from Up Right
            return slashDownRight;
        }
        else if (!xOverHigh && !yOverMed)
        {
            // Down from Right
            return slashDown;
        }
        else if (!xOverHigh && yOverMed)
        {
            // Down Left from Down Right
            return slashDownLeft;
        }
        else if (xOverHigh && yOverLow)
        {
            // Left from Down
            return slashLeft;
        }
        else if (xOverMed && yOverMed) {
            // Up Left from Down Left
            return slashUpLeft;
        }
        return slashDownRight;
        
        
        
        /*
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
        */
    }


void PlaySlashAnimation(){
        GameObject prefabToSpawn = slashUp;
        prefabToSpawn = ChooseDirection();
        Instantiate(prefabToSpawn, slashSpawnPoint.position, Quaternion.identity);
        //Destroy(Instantiate(prefabToSpawn, slashSpawnPoint.position, Quaternion.identity), 0.3f);
    

}




    void Update()
    {
        GoToPlayer();
        if (Input.GetMouseButtonDown(0) && !isSwinging && !mouseDown)
        {
            mouseDown = true;
            mouseTimer = 0f;
            
        }
        if (Input.GetMouseButton(0) && mouseDown) {
            mouseTimer += Time.deltaTime;
            if (mouseTimer > 0.2f) {
                startSpecial = true;
            }
            if (mouseTimer > 1) {
                StartCoroutine(SpecialAttack());
                mouseTimer = 0f;
                mouseDown = false;

            }
            

        }
        if (Input.GetMouseButtonUp(0)) {
            if (mouseDown)
            {
                
                StartCoroutine(NormalAttack());
            }
        }


        //StartCoroutine(NormalAttack());
        if (!Input.GetMouseButton(0)) {
            mouseDown = false;
            startSpecial = false;
        }




    }


    private IEnumerator NormalAttack()
    {
        UnityEngine.Debug.Log("Normal Attack!");
        isSwinging = true;
        enableTrail = true;
        //mouseDown = true;
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

        enableTrail = false;
        isSwinging = false;
    }

    private IEnumerator SpecialAttack()
    {
        UnityEngine.Debug.Log("Special Atttack!");
        isSwinging = true;
        enableTrail = true;
        //mouseDown = true;
        PlaySlashAnimation();

    float elapsed = 0f;
    // Start the swing 90 degrees IN FRONT, lerp to 90 degrees BEHIND
    float startAngleOffset = (swingAngle*3) / 2f;
    float endAngleOffset = (-swingAngle*3) / 2f;

    // Cache original alignment offset calculated in step 5/7
    float baseScratchOffset = setAngleOffset;
    float swordGravity = swordGravityStart;



    while (elapsed < swingDuration)
    {

        elapsed += Time.deltaTime;
        float progress = elapsed / swingDuration;
        swordGravity += swordGravitySpeed*2;


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

        enableTrail = false;
        isSwinging = false;
        startSpecial = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object we hit has the "Enemy" tag (make sure your enemy has the tag "Enemy" in Unity!)
        if (collision.CompareTag("Enemy") && isSwinging)
        {
            // Try to get the Enemy component and deal damage
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                if (startSpecial) {
                    UnityEngine.Debug.Log("Special Attack Damage!");
                    enemy.TakeDamage(25, transform.position);
                }
                else
                {
                    UnityEngine.Debug.Log("Normal Attack Damage!");
                    enemy.TakeDamage(5, transform.position);
                }
                    
            }
        }
    }







}




