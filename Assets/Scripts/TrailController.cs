using UnityEngine;

public class TrailController : MonoBehaviour
{

    private TrailRenderer trailRenderer;
    public WeaponSwing weapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        UnityEngine.Debug.Log("Reference is: " + weapon.enableTrail);


    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log("Reference is: "+weapon.enableTrail);

        if (weapon.enableTrail)
        {
            trailRenderer.enabled = true;
        }
        else
        {
            trailRenderer.enabled = false;
        }

    }
}
