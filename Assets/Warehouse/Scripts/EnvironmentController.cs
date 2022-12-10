using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;
using UnityEditor.Experimental.GraphView;

public class EnvironmentController : MonoBehaviour
{
    [NonSerialized]
    public SimpleMultiAgentGroup m_AgentGroup;

    private SpawnTable settings;
    public class PlayerInfo
    {
        public AgentMover Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }
    public List<Agent> AgentsList = new List<Agent>();
    public Agent Agent;
    public GameObject AgentSpawnPoint;
    public List<GameObject> AgentSpawnPoints = new List<GameObject>();
    private EnvironmentSettings m_EnvironmentSettings;
    private int m_ResetTimer;
    [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;


    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_AgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
        //Hurry Up Penalty
        if (settings.packages.Count == 0)
        {
            Debug.Log("STTTTOOOOÅP");
            m_AgentGroup.EndGroupEpisode();
            ResetScene();
        }
        m_AgentGroup.AddGroupReward(-1f / MaxEnvironmentSteps);

    }


    // Start is called before the first frame update
    void Start()
    {
        m_EnvironmentSettings = FindObjectOfType<EnvironmentSettings>();
        LoadSpawnPoints();
        int numberOfAgents = m_EnvironmentSettings.numberOfAgents > AgentSpawnPoints.Count ? AgentSpawnPoints.Count : m_EnvironmentSettings.numberOfAgents;
        for (int i = 0; i < numberOfAgents; i++)
        {
            int randomNumber = UnityEngine.Random.Range(0, AgentSpawnPoints.Count);
            GameObject agent = Instantiate(Agent.gameObject, AgentSpawnPoints[randomNumber].transform.position, AgentSpawnPoints[randomNumber].transform.rotation,gameObject.transform);
            AgentsList.Add(agent.GetComponent<Agent>());
            AgentSpawnPoints.Remove(AgentSpawnPoints[randomNumber]);


        }


        settings = transform.GetComponent<SpawnTable>();
        m_AgentGroup = new SimpleMultiAgentGroup();

        foreach (var item in AgentsList)
        {
            m_AgentGroup.RegisterAgent(item);
        }

        ResetScene();

    }

    private void LoadSpawnPoints()
    {
        AgentSpawnPoints.Clear();
        for (int i = 0; i < AgentSpawnPoint.transform.childCount; i++)
        {
            AgentSpawnPoints.Add(AgentSpawnPoint.transform.GetChild(i).gameObject);
        }
    }

    private void ResetScene()
    {
        m_ResetTimer = 0;
        settings.SpawnTables();
        LoadSpawnPoints();
        

        foreach (var item in AgentsList)
        {
            ResetPosition(item.gameObject);
        }
        


    }

    void ResetPosition(GameObject obj)
    {
        int randomNumber = UnityEngine.Random.Range(0, AgentSpawnPoints.Count);
        obj.transform.position = AgentSpawnPoints[randomNumber].transform.position+ new Vector3(0,0.5f,0);
        obj.transform.rotation = AgentSpawnPoints[randomNumber].transform.rotation;
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        AgentSpawnPoints.RemoveAt(randomNumber);
        obj.GetComponent<AgentMover>().setActivatePackage.SetActive(false);
        obj.GetComponent<AgentMover>().gotPackage = false;
        obj.GetComponent<AgentMover>().whichPackage = -1;


    }

}
