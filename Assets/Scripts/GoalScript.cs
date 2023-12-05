using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GoalScript : MonoBehaviour, ITriggerableObject
{
    [SerializeField] private EnvironmentManager environmentManager;
    
    public void Trigger(AgentController agent)
    {
        agent.AddReward(agent.IsLookingAtTarget ? Constants.TargetFacingCollisionReward : Constants.TargetNotFacingCollisionReward);
        environmentManager.AddTexture(true);
        agent.EndEpisode();
    }
}
