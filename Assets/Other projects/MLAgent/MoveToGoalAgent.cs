using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRendere;
    
    public override void OnEpisodeBegin()
    {
        //Vector3 reset = gameObject.transform.position;
        transform.localPosition = new Vector3(Random.Range(-2f, +6f),0,Random.Range(0, +2f));
        targetTransform.localPosition = new Vector3(Random.Range(-1f, 5f),0,Random.Range(4.2f, +7.7f));

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float moveSpeed = 5f;
        transform.localPosition += new Vector3(moveX, 0, moveY) * Time.deltaTime*moveSpeed;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousAction = actionsOut.ContinuousActions;
        continousAction[0] = Input.GetAxisRaw("Horizontal");
        continousAction[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "goal") {
            SetReward(1f);
            floorMeshRendere.material = winMaterial;
            EndEpisode();
        }

        if (other.tag == "wall")
        {
            SetReward(-1f);
            floorMeshRendere.material = loseMaterial;

            EndEpisode();
        }
    }

    }

