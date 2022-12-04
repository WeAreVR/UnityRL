using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentLogic : Agent
{
    [SerializeField] private GameObject packageTransform;
    [SerializeField] private GameObject dropPointTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public bool gotPackage;
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0f,0,-3f);
        packageTransform.transform.localPosition = new Vector3(Random.Range(-9f, 9f), 0, Random.Range(-2f , 7f));
        gotPackage = false;
        packageTransform.SetActive(true);
        //packageTransform.localPosition = new Vector3(9,0,9);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        var dirToPackage = (transform.localPosition - packageTransform.transform.localPosition).normalized;
        var dirToDropPoint = (transform.localPosition - dropPointTransform.transform.localPosition).normalized;
        sensor.AddObservation(gotPackage);
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(packageTransform.transform.localPosition);
        //sensor.AddObservation(dirToPackage.z);
        sensor.AddObservation(dropPointTransform.transform.localPosition);
        //sensor.AddObservation(dirToDropPoint.z);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-1f / MaxStep);
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
            AddReward(1f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.5f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
}
