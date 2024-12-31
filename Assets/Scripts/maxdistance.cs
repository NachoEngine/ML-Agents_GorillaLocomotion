using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maxdistance : MonoBehaviour
{
    public Transform origin;
    public float maxDistance = 1.1f;

    // Define min and max boundaries for each axis
    public Vector3 minPosition;
    public Vector3 maxPosition;

    void Update()
    {
        float currentDistance = Vector3.Distance(origin.position, transform.position);

        // Restrict the object's distance from the origin
        if (currentDistance > maxDistance)
        {
            Vector3 direction = (transform.position - origin.position).normalized;
            transform.position = origin.position + direction * maxDistance;
        }

        
    }
}
