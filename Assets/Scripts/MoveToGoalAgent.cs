using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveToGoalAgent : Agent
{
    void Start()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;

    }
    [SerializeField] private Transform targetTransform;
    [SerializeField] private MeshRenderer meshrenderer;

    public override void OnEpisodeBegin()
    {
        //transform.localPosition = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f));
        //targetTransform.localPosition = new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers action)
    {
        float moveX = action.ContinuousActions[0];
        float moveZ = action.ContinuousActions[1];

        float moveSpeed = 3f;
        transform.localPosition += new Vector3(moveX,0,moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            Debug.Log("Win");
            SetReward(+1f);
            meshrenderer.material.color = Color.green;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            Debug.Log("Lose");
            SetReward(-1f);
            meshrenderer.material.color = Color.red;
            EndEpisode();
        }
        
    }

}
