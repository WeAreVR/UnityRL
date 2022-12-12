using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public List<GameObject> agents;

    public void addGlobalRewards(float reward)
    {
        for (int i = 0; i < agents.Count; i++){
            agents[i].GetComponent<AgentMover>().AddReward(reward);
        }
    }
    //public void Update()
    //{
    //    for (int i = 0; i < agents.Count; i++)
    //    {
    //        if (agents[i].GetComponent<AgentMover>().gotPackage == true
    //            && (agents[i].GetComponent<AgentMover>().GetComponent<Collider>().gameObject.tag == "0"
    //            || agents[i].GetComponent<AgentMover>().GetComponent<Collider>().gameObject.tag == "1"
    //            || agents[i].GetComponent<AgentMover>().GetComponent<Collider>().gameObject.tag == "2"
    //            || agents[i].GetComponent<AgentMover>().GetComponent<Collider>().gameObject.tag == "3"))
    //        {
    //           if(agents[i])

    //        }
    //    }
    //}

}
