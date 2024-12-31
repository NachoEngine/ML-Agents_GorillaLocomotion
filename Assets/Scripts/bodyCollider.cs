using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bodyCollider : MonoBehaviour
{
    public GorillaAgentToGoal mainAgent;  // Reference to the main agent script

    void OnTriggerEnter(Collider other)
    {
        // Call the main agent's method when a trigger event occurs
        mainAgent.bodyCollider(other, this);
    }
}
