using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentLogic : Agent
{
    [SerializeField] private GameObject packageTransform;
    [SerializeField] private Transform dropPointTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public bool gotPackage;
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(9,0,-9);
        packageTransform.transform.localPosition = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-8f , 8f));
        gotPackage = false;
        packageTransform.SetActive(true);
        //packageTransform.localPosition = new Vector3(9,0,9);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(packageTransform.transform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 5f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Package>(out Package package) && gotPackage == false)
        {
            AddReward(0.5f);
            gotPackage = true;
            packageTransform.SetActive(false);
        }
        if ((other.TryGetComponent<DropPoint>(out DropPoint dropPoint) && gotPackage))
        {
            AddReward(0.5f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
}
