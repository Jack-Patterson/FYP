using UnityEngine;

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
