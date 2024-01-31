using Unity.MLAgents.Actuators;
using UnityEngine;

public class AgentControllerExtendedMovement : AgentController
{
    private new void Move(float zMovement, float xMovement)
    {
        Vector3 directionToMove = new Vector3(xMovement, 0, zMovement);
        transform.Translate(directionToMove.normalized * (Constants.CharacterMoveSpeed * Time.deltaTime));
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float zMovement = actions.ContinuousActions[0];
        float xRotation = actions.ContinuousActions[1];
        float xMovement = actions.ContinuousActions[2];

        Move(zMovement, xMovement);
        Rotate(xRotation);

        AddReward(Constants.MaxStepDividend/MaxStep);
    }
}