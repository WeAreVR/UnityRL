using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentMover : Agent
{
    [SerializeField] private Transform targetTransform;
    //skal vi bruge rigidbody til at bevæge os eller bare ændre position
    private Rigidbody m_AgentRb;
    public bool gotPackage = false;
    private EnvironmentSettings m_EnvironmentSettings;
    [SerializeField] private GameObject setActivatePackage;
    public int? whichPackage;
    public GameObject tableTargetPrefab;
    public Material changeMaterial;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        gotPackage = false;
        m_EnvironmentSettings = FindObjectOfType<EnvironmentSettings>();

    }

    public override void OnEpisodeBegin()
    {
        //Vector3 reset = gameObject.transform.position;
        // transform.localPosition = new Vector3(Random.Range(-2f, +6f), 0, Random.Range(0, +2f));
        //targetTransform.localPosition = new Vector3(Random.Range(-1f, 5f), 0, Random.Range(4.2f, +7.7f));
        targetTransform.localPosition = new Vector3(0, 2, -2);

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(targetTransform.localPosition);

    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        MoveAgent(actionBuffers.DiscreteActions);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Table" && gotPackage == true)
        {
            if (other.GetComponent<SpawnPackage>().packageNumber == whichPackage)
            {
                SetReward(1f);
                whichPackage = null;
                other.GetComponent<SpawnPackage>().ItemDelivered(tableTargetPrefab);
                gotPackage = false;
                setActivatePackage.SetActive(false);

                EndEpisode();
            }

        }

        if (other.tag == "wall" || other.tag == "agent")
        {
            AddReward(-1f);
            EndEpisode();
        }
        if (other.tag == "TablePackage" && gotPackage == false)
        {

            tableTargetPrefab = other.gameObject;
            AddReward(1f);
            setActivatePackage.SetActive(true);
            whichPackage = other.GetComponent<TableCollisonCheck>().packageNumber;


            //changeMaterial = other.GetComponentsInChildren<MeshRenderer>().material;
            //super trashy måde at gøre det på føler jeg men det virker
            GameObject childGameObject1 = other.transform.GetChild(0).gameObject;
            changeMaterial = childGameObject1.GetComponent<MeshRenderer>().material;


            gotPackage = true;
            setActivatePackage.GetComponent<Renderer>().material = changeMaterial;


        }


    }
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];

        //testede det her og det virkede ikke særlig godt, men det ville nok være det rigtige at bruge se om vi kan gøre bedre
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * m_EnvironmentSettings.agentRunSpeed,
            ForceMode.VelocityChange);
        
    }

}

