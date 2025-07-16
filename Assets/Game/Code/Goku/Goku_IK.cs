using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Goku_IK : MonoBehaviour
{
    
    public Transform raycastOrigin;                    // E.g. shoulder or chest
    public Transform handTarget;                       // IK target (empty object)
    public TwoBoneIKConstraint handIKConstraint;       // The constraint itself
    public float rayDistance = 1.5f;
    public LayerMask wallLayer;
    public float smoothSpeed = 5f;

    private float currentWeight = 0f;
    private float targetWeight = 0f;


    void Update()
    {
        // Cast a ray from the body forward
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, rayDistance, wallLayer))
        {
            // Move hand target slightly off the wall surface
            handTarget.position = hit.point + hit.normal * 0.02f;

            // We want IK active
            targetWeight = 1f;
        }
        else
        {
            // No wall detected, deactivate IK
            targetWeight = 0f;
        }

        // Smoothly interpolate the IK weight
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * smoothSpeed);
        handIKConstraint.weight = currentWeight;
    }

    void OnDrawGizmos()
    {
        if (raycastOrigin)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(raycastOrigin.position, raycastOrigin.forward * rayDistance);
        }
    }
    
}