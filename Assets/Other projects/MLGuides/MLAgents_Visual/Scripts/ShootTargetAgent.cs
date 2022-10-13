using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using CodeMonkey.Utils;

public class ShootTargetAgent : Agent {

    [SerializeField] private ShootTargetEnvironment shootTargetEnvironment;
    [SerializeField] private Transform mapTransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform shootTransform;


    private void Start() {
        shootTargetEnvironment.OnBirdRespawned += ShootTargetEnvironment_OnBirdRespawned;
    }

    private void ShootTargetEnvironment_OnBirdRespawned(object sender, System.EventArgs e) {
        EndEpisode();
    }

    public override void OnEpisodeBegin() {
        //targetTransform.localPosition = new Vector3(Random.Range(0, 50), Random.Range(0, 50));
    }

    public override void OnActionReceived(ActionBuffers actions) {
        int positionX = actions.DiscreteActions[0];
        int positionY = actions.DiscreteActions[1];

        Vector2 shootPositionLocal = new Vector2(positionX, positionY);

        shootTransform.localPosition = shootPositionLocal;

        Vector2 shootPosition = mapTransform.TransformPoint(shootPositionLocal);

        if (shootTargetEnvironment.ShootAt(shootPosition)) {
            // Hit target!
            AddReward(1f);
            EndEpisode();
        } else {
            AddReward(-0.01f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        Vector3 worldPosition = UtilsClass.GetMouseWorldPosition();
        Vector3 localPosition = mapTransform.InverseTransformPoint(worldPosition);

        ActionSegment<int> discreteAction = actionsOut.DiscreteActions;
        discreteAction[0] = Mathf.Clamp(Mathf.RoundToInt(localPosition.x), 0, 49);
        discreteAction[1] = Mathf.Clamp(Mathf.RoundToInt(localPosition.y), 0, 49);
    }

}
