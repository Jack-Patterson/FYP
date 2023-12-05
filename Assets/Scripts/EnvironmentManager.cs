using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    internal AgentController AgentController;
    [SerializeField] private Renderer ground;
    [SerializeField] private Texture victoryTexture;
    [SerializeField] private Texture failureTexture;
    [SerializeField] private AgentController agent;
    [SerializeField] private GoalScript target;
    
    internal void AddTexture(bool succeeded)
    {
        ground.material.mainTexture = succeeded ? victoryTexture : failureTexture;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(agent.transform.position, agent.transform.position + agent.transform.forward * Constants.DrawGizmosDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(agent.transform.position, target.transform.position);
    }
}