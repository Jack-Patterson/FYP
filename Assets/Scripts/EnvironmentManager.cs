using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnvironmentManager : MonoBehaviour
{
    internal AgentController AgentController;
    [SerializeField] private Renderer ground;
    [SerializeField] private Texture victoryTexture;
    [SerializeField] private Texture failureTexture;
    [SerializeField] private AgentController agent;
    [SerializeField] private GoalScript target;
    
    [SerializeField] private Transform[] wallGaps;
    [SerializeField] private Vector2 wallGapRange = new Vector2(-22,22);
    private List<Vector3> wallGapStartingPosition;

    private void Start()
    {
        foreach (Transform wallGap in wallGaps)
        {
            wallGapStartingPosition.Add(wallGap.position);
        }
    }

    internal void OnEpisodeBegin()
    {
        for (int i = 0; i < wallGaps.Length; i++)
        {
            Transform wallGap = wallGaps[i];
            Vector3 position = wallGap.position;
            
            position = new Vector3( wallGapStartingPosition[i].x + Random.Range(wallGapRange.y, wallGapRange.x), position.y, position.z);
            wallGap.position = position;
        }
    }
    
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