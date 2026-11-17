using UnityEngine;

public class TrailController : MonoBehaviour
{

    private TrailRenderer trailRenderer;
    public WeaponSwing weapon;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        // Automatically find WeaponSwing on this object or its parent/children if unassigned
        if (weapon == null)
        {
            weapon = GetComponentInParent<WeaponSwing>();
            if (weapon == null)
            {
                weapon = FindAnyObjectByType<WeaponSwing>();
            }
        }
    }

    void Update()
    {
        if (weapon != null)
        {
            trailRenderer.enabled = weapon.enableTrail;
        }
    }
}
