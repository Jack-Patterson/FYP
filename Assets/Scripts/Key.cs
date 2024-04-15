using UnityEngine;

public class Key : MonoBehaviour, ITriggerableObject
{
    public void Trigger(AgentController agent)
    {
        agent.AddReward(0.1f);
        Destroy(gameObject);
    }
}