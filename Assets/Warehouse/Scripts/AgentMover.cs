using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System.Threading;
using Unity.VisualScripting;

public class AgentMover : Agent
{
    private Rigidbody m_AgentRb;
    private EnvironmentSettings m_EnvironmentSettings;
    private BehaviorParameters m_BehaviorParameters;
    public SpawnTable m_SpawnTable;

    [SerializeField] private GameObject setActivatePackage;

    [SerializeField] private CharacterController _controller;
    public bool gotPackage;
    public int whichPackage;
    private int numberOfVectorsInTablesWithPair;

    public GameObject tableTargetPrefab;
    public Material changeMaterial;
    private List<GameObject> listOfTables = new List<GameObject>();
    private List<GameObject> listOfTablesWithPackge = new List<GameObject>();
    private SpawnTable settings;
    private bool isColliding = false;
    public int agentSpeed = 1;
    public GameObject plane;
    public RayPerceptionOutput.RayOutput[] RayOutputs;

    void Update()
    {
        isColliding = false;
    }

    public override void Initialize()
    {

        _controller = gameObject.GetComponent<CharacterController>();
        _controller.center = new Vector3(0, 2.5f, 0);
        m_AgentRb = GetComponent<Rigidbody>();
        gotPackage = false;
        m_EnvironmentSettings = FindObjectOfType<EnvironmentSettings>();
        m_BehaviorParameters = gameObject.transform.GetComponent<BehaviorParameters>();
        //m_SpawnTable = FindObjectOfType<SpawnTable>();
        settings = transform.root.GetComponent<SpawnTable>(); ;
        //listOfTables = new List<GameObject>(settings.GetComponent<SpawnTable>().tables);
        //Get back with smart way to do this
        //vi får lidt fejl indtil videre fordi vi fjener fra listen så der er færre obsevationer end objecter, men det vil løse sig selv hvis vi adder borde når et bliver fjernet
        //Overstående er fixed men beholder kommentar fordi vi skal add borde
        //

        //3 for transform og 2 for de andre
        //NOTE: 1 FRAME MED 5 OBSERVATIONS!!! SANDSYNLIGVIS IKKE ET PROBLEM??
        numberOfVectorsInTablesWithPair = m_EnvironmentSettings.numberOfTables * 4;
        m_BehaviorParameters.BrainParameters.VectorObservationSize = numberOfVectorsInTablesWithPair + 5;


        //listOfTablesWithPackge.Add(table.GetComponent<SpawnPackage>().tableSpawned);
        //Delay because it tables are not spawned yet
        //Invoke("addToList",0.5f);

    }
    void addToList()
    {
        foreach (GameObject table in listOfTables)
        {
            listOfTablesWithPackge.Add(table.GetComponent<SpawnPackage>().tableSpawned);
        }
    }

    public override void OnEpisodeBegin()
    {
        //Package on top of the agent
        setActivatePackage.SetActive(false);
        //clear list so we we dont get double
        ClearAndDestoryList(listOfTablesWithPackge, listOfTables);
        //Instantiate all the tables
        m_SpawnTable.SpawnTables();

        //listOfTables = new List<GameObject>(settings.GetComponent<SpawnTable>().tables
        listOfTables = settings.tables;
        listOfTablesWithPackge = settings.packages;
        
        //trashy way to make sure tables are spawned before adding them to the list 
        //Invoke("addToList", 0.2f);
        _controller.enabled = false;
        _controller.transform.position = plane.transform.position;
        _controller.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        _controller.enabled = true;
        //transform.localPosition = plane.transform.localPosition;
        //GetComponent<Rigidbody>().velocity = Vector3.zero;
        //GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //targetTransform.localPosition = new Vector3(Random.Range(-1f, 5f), 0, Random.Range(4.2f, +7.7f));
        //targetTransform.localPosition = new Vector3(0, 2, -2);
        gotPackage = false;
        whichPackage = -1;


    }
    public override void CollectObservations(VectorSensor sensor)
    {
       
        //mlagent sorter kan måske være relevant
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(gotPackage);
        sensor.AddObservation(whichPackage);
        
        for (int i = 0; i < listOfTables.Count; i++)
        {
            /* No need for y since it is never used i think
            sensor.AddObservation(listOfTablesWithPackge[i].transform.position);
            sensor.AddObservation(listOfTables[i].transform.position);
            */
            var dirToTable = (listOfTables[i].transform.localPosition - transform.localPosition).normalized;
            var dirToPackage = (listOfTablesWithPackge[i].transform.localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirToPackage.x);
            sensor.AddObservation(dirToPackage.z);
            sensor.AddObservation(dirToTable.x);
            sensor.AddObservation(dirToTable.z);

        }
        /*
        
        foreach (var table in listOfTables)
        {
            sensor.AddObservation(table.transform.position);

            sensor.AddObservation(table.GetComponent<SpawnPackage>().tableSpawned.transform.position);
        }
        */
    }


    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        AddReward(-1f / MaxStep);
        MoveAgent(actionBuffers.DiscreteActions);

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        
         if (Input.GetKey(KeyCode.W))
        {
            //Debug.Log(RayOutputs);
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 2;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 1;
        }
        
        /*
        else if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[0] = 4;
        }
        */
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isColliding) return;
        isColliding = true;
        //Transform firstChild = other.transform.GetChild(0);

        if (other.tag == "wall" || other.tag == "agent")
        {

            //AddReward(-1f);
            //EndEpisode();
        }
        if (other.tag == "TablePackage" && gotPackage == false)
        {
            tableTargetPrefab = other.gameObject;
            AddReward(0.5f);
            setActivatePackage.SetActive(true);
            whichPackage = other.GetComponent<TableCollisonCheck>().packageNumber;
            //changeMaterial = other.GetComponentsInChildren<MeshRenderer>().material;
            //super trashy m�de at g�re det p� f�ler jeg men det virker
            GameObject childGameObject1 = other.transform.GetChild(0).gameObject;
            changeMaterial = childGameObject1.GetComponent<MeshRenderer>().material;
            gotPackage = true;
            setActivatePackage.GetComponent<Renderer>().material = changeMaterial;
        }

        if (other.tag == "Table" && gotPackage == true)
        {
            if (other.GetComponent<SpawnPackage>().packageNumber == whichPackage)
            {
                AddReward(1f);
                //whichPackage = other.GetComponent<SpawnPackage>().randomMaterials.Length+1;
                whichPackage = -1;
                /*
                //virker ikke fjener ikke fra liste
                listOfTablesWithPackge.Remove(other.gameObject);
                listOfTables.Remove(tableTargetPrefab);
                */
                //RemoveAt virker 
                RemoveFromList(listOfTables, other.gameObject);
                RemoveFromList(listOfTablesWithPackge, tableTargetPrefab);

                other.GetComponent<SpawnPackage>().ItemDelivered(tableTargetPrefab);
                gotPackage = false;
                setActivatePackage.SetActive(false);
                if (listOfTables.Count == 0)
                {
                    EndEpisode();
                }
            }
        }

    }
    public void MoveAgent(ActionSegment<int> act)
    {
        
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        var action1 = act[1];

        //testede det her og det virkede ikke s�rlig godt, men det ville nok v�re det rigtige at bruge se om vi kan g�re bedre
        //skal den kunne rotate up?
        switch (action)
        {
            case 1:
                _controller.Move(transform.forward*m_EnvironmentSettings.agentRunSpeed);
                //dirToGo = transform.forward * 1f;
                break;
            case 2:
                _controller.Move(transform.forward * (m_EnvironmentSettings.agentRunSpeed * -1));
                break;
        }
        switch (action1)
        {
            case 1:
                // _controller.Move(transform.forward * m_EnvironmentSettings.agentRunSpeed);
                //dirToGo = transform.forward * 1f;
                _controller.transform.Rotate(0, -m_EnvironmentSettings.agentRotationSpeed, 0);
                break;
            case 2:
                //_controller.Move(transform.forward * (m_EnvironmentSettings.agentRunSpeed * -1));
                _controller.transform.Rotate(0, m_EnvironmentSettings.agentRotationSpeed, 0);
                break;
        }
        //transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        //m_AgentRb.AddForce(dirToGo * m_EnvironmentSettings.agentRunSpeed,
        //  ForceMode.VelocityChange);

    }
    
    void RemoveFromList(List<GameObject> list,GameObject obj)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == obj)
            {
                list.RemoveAt(i);
                break;
            }
        }
    }


    void ClearAndDestoryList(List<GameObject> list, List<GameObject> list2)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i]);
            Destroy(list2[i]);
        }
        list.Clear();
        list2.Clear();

    }
}

