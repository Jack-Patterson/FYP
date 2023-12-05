using UnityEngine;

public class WallScript : MonoBehaviour, ITriggerableObject
{
    [SerializeField] private EnvironmentManager environmentManager;
    
    public void Trigger(AgentController agent)
    {
        agent.AddReward(Constants.WallCollisionReward);
        environmentManager.AddTexture(false);
        agent.EndEpisode();
    }
}