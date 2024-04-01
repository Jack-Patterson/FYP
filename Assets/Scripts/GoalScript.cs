using UnityEngine;

public class GoalScript : MonoBehaviour, ITriggerableObject
{
    [SerializeField] private EnvironmentManager environmentManager;
    
    public void Trigger(AgentController agent)
    {
        agent.AddReward(Constants.TargetFacingCollisionReward);
        environmentManager.AddTexture(true);
        agent.EndEpisode();
    }
}
