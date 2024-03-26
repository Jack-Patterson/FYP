using UnityEngine;

public class WallAreaScript : ITriggerableObject
{
    [SerializeField] private EnvironmentManager environmentManager;

    public void Trigger(AgentController agent)
    {
        agent.AddReward(0.5f);
        environmentManager.AddTexture(true);
        agent.EndEpisode();
    }
}