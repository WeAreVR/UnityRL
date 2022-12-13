using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System.Threading;
using Unity.VisualScripting;
using System;
using Random = UnityEngine.Random;

public class AgentMover : Agent
{
    private Rigidbody m_AgentRb;
    private EnvironmentSettings m_EnvironmentSettings;
    private BehaviorParameters m_BehaviorParameters;
    public SpawnTable m_SpawnTable;
    BufferSensorComponent m_BufferSensor;

    [SerializeField] private GameObject setActivatePackage;

    [SerializeField] private CharacterController _controller;
    public bool gotPackage;
    public int whichPackage;
    private int numberOfVectorsInTablesWithPair;
    public GameObject m_enviroment;
    public GameObject otherAgent;

    public GameObject tableTargetPrefab;
    public Material changeMaterial;
    private List<GameObject> listOfTables = new List<GameObject>();
    private List<GameObject> listOfTablesWithPackge = new List<GameObject>();
    private List<GameObject> ports = new List<GameObject>();
    private SpawnTable settings;
    private bool isColliding = false;
    public int agentSpeed = 1;
    public GameObject plane;
    public RayPerceptionOutput.RayOutput[] RayOutputs;
    public GameObject winIndicator;
    public Material winMaterial;
    public Material LoseMaterial;
    [SerializeField] private int steps;
    //public Transform spawnobj;
    public List<Transform> AgentSpawnPoints = new List<Transform>();
    public List<int> timePerEpoch = new List<int>();
    //public GameObject plane;

    void FixedUpdate()
    {
        isColliding = false;
        steps++;
        if ( steps % MaxStep == 0)
        {
            //timePerEpoch.Add(steps);
            //winIndicator.GetComponent<MeshRenderer>().material = LoseMaterial;
            //EndEpisode();
            //Debug.Log(Academy.Instance.StepCount);
        }

    }
    void GetSpawnPoints()
    {
        for (int i = 0; i < settings.spawnobj.transform.childCount; i++)
        {
            AgentSpawnPoints.Add(settings.spawnobj.transform.GetChild(i));
        }

    }

    public override void Initialize()
    {
        // _controller = gameObject.GetComponent<CharacterController>();
        // _controller.center = new Vector3(0, 2.5f, 0);
        //m_BufferSensor = GetComponent<BufferSensorComponent>();
        m_AgentRb = GetComponent<Rigidbody>();
        gotPackage = false;
        m_EnvironmentSettings = FindObjectOfType<EnvironmentSettings>();
        m_BehaviorParameters = gameObject.transform.GetComponent<BehaviorParameters>();
        //m_SpawnTable = FindObjectOfType<SpawnTable>();
        settings = transform.root.GetComponent<SpawnTable>();
        //listOfTables = new List<GameObject>(settings.GetComponent<SpawnTable>().tables);
        //Get back with smart way to do this
        //vi får lidt fejl indtil videre fordi vi fjener fra listen så der er færre obsevationer end objecter, men det vil løse sig selv hvis vi adder borde når et bliver fjernet
        //Overstående er fixed men beholder kommentar fordi vi skal add borde
        //

        //3 for transform og 2 for de andre
        //numberOfVectorsInTablesWithPair = (m_EnvironmentSettings.numberOfTables * 2) + 8;
        //m_BehaviorParameters.BrainParameters.VectorObservationSize =  5+8;
        m_BehaviorParameters.BrainParameters.VectorObservationSize =  5;
        //m_BufferSensor.MaxNumObservables = (settings.rows.Count*2);

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
    void setStartPackage()
    {
        whichPackage = FindObjectOfType<TableCollisonCheck>().packageNumber;
        gotPackage = true;

        //Denne her linje vil skabe problemer fordi vi kun har 1 bord
        //ClearAndDestoryList(listOfTablesWithPackge);
        
        setActivatePackage.SetActive(true);

        var mat = FindObjectOfType<TableCollisonCheck>().transform.GetChild(0).GetComponent<Renderer>().material;
        setActivatePackage.GetComponent<Renderer>().material = mat;


    }
    public override void OnEpisodeBegin()
    {
        //Package on top of the agent
        if(steps != 0) 
        {
            timePerEpoch.Add(steps);
        }
        steps = 0;
    
        setActivatePackage.SetActive(false);
        m_AgentRb.velocity = Vector3.zero;
        m_AgentRb.angularVelocity = Vector3.zero;
        //clear list so we we dont get double
        //ClearAndDestoryList(listOfTablesWithPackge);
        //ClearAndDestoryList(listOfTables);
        //Instantiate all the tables
        m_SpawnTable.SpawnTables();
        //transform.localPosition = settings.spawnobj.GetChild(Random.Range(0, settings.spawnobj.childCount)).transform.localPosition;
        //transform.localPosition = settings.transform.localPosition + new Vector3(5f, 0.5f, 0.0f);

        //listOfTables = new List<GameObject>(settings.GetComponent<SpawnTable>().tables
        listOfTables = settings.tables;
        listOfTablesWithPackge = settings.packages;
        ports = settings.ports;
        
        //trashy way to make sure tables are spawned before adding them to the list 
        //Invoke("addToList", 0.2f);
        //_controller.enabled = false;
        //_controller.transform.position = plane.transform.position;
        //_controller.transform.position = plane.transform.position;
        //_controller.transform.rotation = Quaternion.Euler(new Vector3(0, 180, -10));
        //_controller.enabled = true;
        transform.localPosition = plane.transform.localPosition + new Vector3(5,0.5f,0);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        //GetComponent<Rigidbody>().velocity = Vector3.zero;
        //GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //targetTransform.localPosition = new Vector3(Random.Range(-1f, 5f), 0, Random.Range(4.2f, +7.7f));
        //targetTransform.localPosition = new Vector3(0, 2, -2);
        gotPackage = false;
        whichPackage = -1;
        
        //
        // Denne funktion skal kun køres når vi kun vil teste et bord
       // setStartPackage();
        //


    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //mlagent sorter kan måske være relevant
        // BufferSensorComponent.AppendObservation(5.0f);
        //sensor.AddObservation(transform.localPosition.x);
        //sensor.AddObservation(transform.localPosition.z);
        //sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
        sensor.AddObservation(gotPackage);
        sensor.AddObservation(whichPackage);
        //sensor.AddObservation(otherAgent.transform.localPosition);

        //for (int i = 0; i < listOfTablesWithPackge.Count; i++)
        //{
        //    /* No need for y since it is never used i think
        //    sensor.AddObservation(listOfTablesWithPackge[i].transform.position.x);
        //    sensor.AddObservation(listOfTablesWithPackge[i].transform.position.z);
        //    sensor.AddObservation(listOfTables[i].transform.position.x);
        //    sensor.AddObservation(listOfTables[i].transform.position.z);
        //    */

        //    var dirToPackage = (listOfTablesWithPackge[i].transform.localPosition - transform.localPosition).normalized;
        //    //kig på det her 

        //    //sensor.AddObservation(listOfTables[i].transform.localPosition);
        //    //sensor.AddObservation(listOfTablesWithPackge[i].transform.localPosition);

        //    //sensor.AddObservation(dirToPackage.x);
        //    //sensor.AddObservation(dirToPackage.z);
        //    float[] packageObservation = new float[]
        //    {
        //        dirToPackage.x,
        //        dirToPackage.z
        //    };

        //    m_BufferSensor.AppendObservation(packageObservation);
        //}
        ////m_BufferSensor.AppendObservation(dirToPackage.x);
        //for (int i = 0; i < ports.Count; i++)
        //{

        //    var dirToPort = (ports[i].transform.localPosition - transform.localPosition).normalized;

        //    sensor.AddObservation(dirToPort.x);
        //    sensor.AddObservation(dirToPort.z);

        //}

    }
    
        
        
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //steps++;
        //m_enviroment.GetComponent<EnvironmentController>().addGlobalRewards(-1f/MaxStep);
        AddReward(-1f / MaxStep);
        MoveAgent(actionBuffers.DiscreteActions);

    }
    //public override void heuristic(in actionbuffers actionsout)
    //{
    //    var discreteactionsout = actionsout.discreteactions;

    //    if (input.getkey(keycode.w))
    //    {
    //        //debug.log(rayoutputs);
    //        discreteactionsout[0] = 1;
    //    }
    //    else if (input.getkey(keycode.s))
    //    {
    //        discreteactionsout[0] = 2;
    //    }
    //    if (input.getkey(keycode.d))
    //    {
    //        discreteactionsout[1] = 2;
    //    }
    //    else if (input.getkey(keycode.a))
    //    {
    //        discreteactionsout[1] = 1;
    //    }

    //    /*
    //    else if (input.getkey(keycode.q))
    //    {
    //        discreteactionsout[0] = 3;
    //    }
    //    else if (input.getkey(keycode.e))
    //    {
    //        discreteactionsout[0] = 4;
    //    }
    //    */
    //}

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
            tableTargetPrefab.tag = "wall";
            //m_enviroment.GetComponent<EnvironmentController>().addGlobalRewards(0.5f);
            AddReward(0.5f);
            setActivatePackage.SetActive(true);
            whichPackage = other.GetComponent<TableCollisonCheck>().packageNumber;
            //changeMaterial = other.GetComponentsInChildren<MeshRenderer>().material;
            //super trashy m�de at g�re det p� f�ler jeg men det virker
            GameObject childGameObject1 = other.transform.GetChild(0).gameObject;
            changeMaterial = childGameObject1.GetComponent<MeshRenderer>().material;
          
            setActivatePackage.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(other.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material);
            gotPackage = true;
            settings.RemoveMat(tableTargetPrefab);
            //changeMaterial = childGameObject1.GetComponent<MeshRenderer>().material;
            //gotPackage = true;
            //setActivatePackage.GetComponent<Renderer>().material = changeMaterial;
        }


        if ((other.tag == "0"|| other.tag == "1" || other.tag == "2"|| other.tag == "3") && gotPackage == true)
        {
            if (other.GetComponent<TableCollisonCheck>().packageNumber == whichPackage)
            {
                //steps = 0;
                //m_enviroment.GetComponent<EnvironmentController>().addGlobalRewards(1f);
                AddReward(1f);
                //whichPackage = other.GetComponent<SpawnPackage>().randomMaterials.Length+1;
                whichPackage = -1;
                winIndicator.GetComponent<MeshRenderer>().material = winMaterial;
                /*
                //virker ikke fjener ikke fra liste
                listOfTablesWithPackge.Remove(other.gameObject);
                listOfTables.Remove(tableTargetPrefab);
                */
                //RemoveAt virker 
                RemoveFromList(listOfTables, other.gameObject);
                RemoveFromList(listOfTablesWithPackge, tableTargetPrefab);

                // other.GetComponent<TableCollisonCheck>().ItemDelivered(tableTargetPrefab);
                // other.GetComponent<TableCollisonCheck>().ItemDelivered(other.gameObject);
                //other.GetComponent<TableCollisonCheck>().ItemDelivered(other.gameObject, tableTargetPrefab);
                //m_SpawnTable.ItemDelivered(other.gameObject);
                m_SpawnTable.RemoveMat(tableTargetPrefab);
                gotPackage = false;
                setActivatePackage.SetActive(false);
                if (listOfTablesWithPackge.Count == 0)
                {
                    EndEpisode();
                    
                }
            }
        }

    }
    /*public void MoveAgent(ActionSegment<int> act)
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
    */
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
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
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * m_EnvironmentSettings.agentRunSpeed,
            ForceMode.VelocityChange);
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


    void ClearAndDestoryList(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i]);
        }
        list.Clear();

    }
}

