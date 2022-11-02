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
    private SpawnTable m_SpawnTable;

    [SerializeField] private GameObject setActivatePackage;


    public bool gotPackage;
    public int whichPackage;
    private int numberOfVectorsInTablesWithPair;

    public GameObject tableTargetPrefab;
    public Material changeMaterial;
    private List<GameObject> listOfTables = new List<GameObject>();
    private List<GameObject> listOfTablesWithPackge = new List<GameObject>();
    private GameObject settings;
    private bool isColliding = false;


     void Update()
    {
        isColliding = false;
    }

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        gotPackage = false;
        m_EnvironmentSettings = FindObjectOfType<EnvironmentSettings>();
        m_BehaviorParameters = FindObjectOfType<BehaviorParameters>();
        m_SpawnTable = FindObjectOfType<SpawnTable>();
        settings = GameObject.Find("EnvironmentSettings");
        //listOfTables = new List<GameObject>(settings.GetComponent<SpawnTable>().tables);
        //Get back with smart way to do this
        //vi får lidt fejl indtil videre fordi vi fjener fra listen så der er færre obsevationer end objecter, men det vil løse sig selv hvis vi adder borde når et bliver fjernet
        //Overstående er fixed men beholder kommentar fordi vi skal add borde
        //

        //3 for transform og 2 for de andre
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
        ClearAndDestoryList(listOfTablesWithPackge, listOfTables);
        m_SpawnTable.SpawnTables();
        //listOfTables = new List<GameObject>(settings.GetComponent<SpawnTable>().tables);
        listOfTables = settings.GetComponent<SpawnTable>().tables;
        Invoke("addToList", 0.2f);
        transform.localPosition = new Vector3(0, 0, 0);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //targetTransform.localPosition = new Vector3(Random.Range(-1f, 5f), 0, Random.Range(4.2f, +7.7f));
        //targetTransform.localPosition = new Vector3(0, 2, -2);
        gotPackage = false;
        whichPackage = -1;


    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //På nuværende tidspunkt ved den ikke hvilke border som har/skal havde de rigtige pakker
        //Vi skal adde whichPackage fra bordene også men ved ikke hvordan vi gør det på nurværnde tidspunkt, måske bare GetComponent<TableCollisonCheck>().packageNumber;
        //eller add dem i et dict til at starte med
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
            sensor.AddObservation(listOfTablesWithPackge[i].transform.position.x);
            sensor.AddObservation(listOfTablesWithPackge[i].transform.position.z);
            sensor.AddObservation(listOfTables[i].transform.position.x);
            sensor.AddObservation(listOfTables[i].transform.position.z);
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

        if (other.tag == "wall" || other.tag == "agent")
        {

            Debug.Log("bang");
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
            //super trashy m�de at g�re det p� f�ler jeg men det virker
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

        //testede det her og det virkede ikke s�rlig godt, men det ville nok v�re det rigtige at bruge se om vi kan g�re bedre
        //skal den kunne rotate up?
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

