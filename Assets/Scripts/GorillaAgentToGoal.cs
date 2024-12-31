using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using GorillaLocomotion;

public class GorillaAgentToGoal : Agent
{
    [SerializeField] private SkinnedMeshRenderer playermesh;
    [SerializeField] private Transform lefthand;
    [SerializeField] private Transform righthand;
    [SerializeField] private Transform head;
    [SerializeField] private Transform target;
    [SerializeField] private Player_Mutli playermovement;

    [SerializeField] private MeshRenderer meshrenderer;

    Color modelcolor;
    Color floorcolor;
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        modelcolor = playermesh.material.color;
        floorcolor = meshrenderer.material.color;

    }
    float repos = 2f;
    public override void OnEpisodeBegin()
    {
        playermovement.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        head.localPosition = new Vector3(0f,1f,2f);//new Vector3(Random.Range(-repos, repos), 1f, Random.Range(-repos, repos));
        target.localPosition = new Vector3(0f, .5f, 0f);// new Vector3(Random.Range(-repos, repos), 0.3f, Random.Range(-repos, repos));
        
        lefthand.localPosition = new Vector3(1f,0f,0f);
        righthand.localPosition = new Vector3(-1f, 0f, 0f);

        playermovement.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        onstart = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(lefthand.localPosition);
        sensor.AddObservation(righthand.localPosition);
        sensor.AddObservation(head.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(playermovement.wasLeftHandTouching ? 1:0);
        sensor.AddObservation(playermovement.wasRightHandTouching ? 1 : 0);

    }
    public override void OnActionReceived(ActionBuffers action)
    {
        Vector3 leftmove = new Vector3(action.ContinuousActions[0], action.ContinuousActions[1], action.ContinuousActions[2]);
        Vector3 rightmove = new Vector3(action.ContinuousActions[3], action.ContinuousActions[4], action.ContinuousActions[5]);

        float moveSpeed = 8f;
        lefthand.localPosition += leftmove * Time.deltaTime * moveSpeed;
        righthand.localPosition += rightmove * Time.deltaTime * moveSpeed;
    }

    float distanceScore;
    float currentDistance;
    float previousDistance;

    bool onstart = true;
    private float checkInterval = 0.3f;  // Time between distance checks (1 second)
    private float timer;
    public void Update()
    {
        return;
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            currentDistance = Vector3.Distance(head.position, target.position);

            if (onstart)
            {
                distanceScore = currentDistance;
                previousDistance = distanceScore;
                onstart = false;
            }

            if (currentDistance < distanceScore)
            {
                distanceScore = currentDistance;
            }
            float distanceReward = currentDistance - distanceScore;
            if (currentDistance > distanceScore)
            {
                SetReward(-1f);
                //Debug.Log("The objects are getting farther apart.");
                playermesh.material.color = Color.red;
                //EndEpisode();
            }
            else if (distanceReward >= 0.1f)
            {
                SetReward(+1f);
                //Debug.Log("The objects are getting closer.");
                playermesh.material.color = Color.green;
            }

            previousDistance = currentDistance;

            timer = 0f;
        }

    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //continuousActions[0] = Input.GetAxisRaw("Horizontal");
        //continuousActions[1] = Input.GetAxisRaw("Vertical");
    }



    public void HandleCollision(Collider other, HandCollider hand)
    {
        //Debug.Log("Collision detected by: " + hand.name + " with " + other.name);

        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(+1f);
            meshrenderer.material.color = Color.green;
            Debug.Log("WIN!!");
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            meshrenderer.material.color = Color.red;
            Debug.Log("WALL!!");
            EndEpisode();
        }
    }

    public void bodyCollider(Collider other, bodyCollider hand)
    {
        SetReward(-1f);
        Debug.Log("Body has collided");
        meshrenderer.material.color = Color.red;
        EndEpisode();
    }



}
