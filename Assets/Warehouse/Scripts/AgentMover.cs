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
using System.Linq;

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

    public GameObject tableTargetPrefab;
    public Material changeMaterial;
    private List<GameObject> listOfTables = new List<GameObject>();
    private List<GameObject> listOfTablesWithPackge = new List<GameObject>();
    private List<GameObject> ports = new List<GameObject>();
    public List<int> timePerEpoch = new List<int>();
    private SpawnTable settings;
    private bool isColliding = false;
    public int agentSpeed = 1;
    public GameObject plane;
    public RayPerceptionOutput.RayOutput[] RayOutputs;
    public Material winMaterial;
    public Material LoseMaterial;
    public int steps;
    public Transform spawnPoint;
    public GameObject[] prefabs;
    private GameObject envPrefab;
    private int numberToRun = 0;
    private int numberToRun2 = 0;

    //public GameObject plane;

    void FixedUpdate()
    {
        isColliding = false;
        steps++;


    }

    public override void Initialize()
    {
        prefabs = Resources.LoadAll<GameObject>("Prefabs");

        //m_BufferSensor = GetComponent<BufferSensorComponent>();
        m_AgentRb = GetComponent<Rigidbody>();
        gotPackage = false;
        m_EnvironmentSettings = FindObjectOfType<EnvironmentSettings>();
        m_BehaviorParameters = gameObject.transform.GetComponent<BehaviorParameters>();
        
        //3 for transform og 2 for de andre
        //numberOfVectorsInTablesWithPair = (m_EnvironmentSettings.numberOfTables * 2) + 8;
        //m_BehaviorParameters.BrainParameters.VectorObservationSize =  5+8;
        m_BehaviorParameters.BrainParameters.VectorObservationSize = 5;
        //m_BufferSensor.MaxNumObservables = (settings.rows.Count*2);

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

        setActivatePackage.SetActive(true);

        var mat = FindObjectOfType<TableCollisonCheck>().transform.GetChild(0).GetComponent<Renderer>().material;
        setActivatePackage.GetComponent<Renderer>().material = mat;


    }

    public override void OnEpisodeBegin()
    {
        Destroy(envPrefab);
        if (steps != 0)
        {
            timePerEpoch.Add(steps);
        }

        steps = 0;
        //if (numberToRun2 % 10 == 0 && numberToRun2 != 0 && numberToRun !=prefabs.Length) numberToRun++;
        var randomEnv = Random.Range(0, prefabs.Length);
        envPrefab = Instantiate(prefabs[randomEnv], transform.root.position, Quaternion.Euler(0, 0, 0));
        spawnPoint = envPrefab.transform.Find("Spawnpoint");
        numberToRun2++;
        //Package on top of the agent
        setActivatePackage.SetActive(false);
        m_AgentRb.velocity = Vector3.zero;
        m_AgentRb.angularVelocity = Vector3.zero;
        //clear list so we we dont get double
        //ClearAndDestoryList(listOfTablesWithPackge);
        //ClearAndDestoryList(listOfTables);
        //Instantiate all the tables
        settings = FindObjectsOfType<SpawnTable>()[0];
        settings.SpawnTables();

        
        //listOfTables = new List<GameObject>(settings.GetComponent<SpawnTable>().tables
        listOfTables = settings.tables;
        listOfTablesWithPackge = settings.packages;
        ports = settings.ports;
        transform.localPosition = spawnPoint.transform.GetChild(Random.Range(0, spawnPoint.transform.childCount)).transform.localPosition;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        gotPackage = false;
        whichPackage = -1;
        
        //
        // Denne funktion skal kun køres når vi kun vil teste et bord
       // setStartPackage();
        //


    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
        sensor.AddObservation(gotPackage);
        sensor.AddObservation(whichPackage);
    }
    
        
        
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //steps++;
        AddReward(-1f / MaxStep);
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
        if (isColliding) return;
        isColliding = true;

        if (other.tag == "wall" || other.tag == "Agent")
        {
            //AddReward(-0.1f);
            //EndEpisode();
        }
        if (other.tag == "TablePackage" && gotPackage == false)
        {
            Debug.Log("Got package");
            tableTargetPrefab = other.gameObject;
            AddReward(0.5f);
            setActivatePackage.SetActive(true);
            whichPackage = other.GetComponent<TableCollisonCheck>().packageNumber;
            GameObject childGameObject1 = other.transform.GetChild(0).gameObject;
            changeMaterial = childGameObject1.GetComponent<MeshRenderer>().material;
            gotPackage = true;
            setActivatePackage.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(other.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material);
            settings.RemoveMat(tableTargetPrefab);
        }


        if ((other.tag == "0"|| other.tag == "1" || other.tag == "2"|| other.tag == "3") && gotPackage == true)
        {
            if (other.GetComponent<TableCollisonCheck>().packageNumber == whichPackage)
            {
                Debug.Log("Package delivered");

                AddReward(1f);
                whichPackage = -1;
                
                
                //RemoveAt virker 
                RemoveFromList(listOfTables, other.gameObject);
                RemoveFromList(listOfTablesWithPackge, tableTargetPrefab);

                gotPackage = false;
                setActivatePackage.SetActive(false);
                if (listOfTablesWithPackge.Count == 0)
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

