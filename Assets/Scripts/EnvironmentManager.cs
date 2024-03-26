using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnvironmentManager : MonoBehaviour
{
    internal AgentController AgentController;
    [SerializeField] private List<Renderer> groundAreasRenderers;
    [SerializeField] private Texture victoryTexture;
    [SerializeField] private Texture failureTexture;
    [SerializeField] private AgentController agent;
    [SerializeField] private GoalScript target;
    internal Transform Target => target.transform;

    [SerializeField] private Vector2 wallGapRange = new(-22, 22);
    [SerializeField] private List<Transform> wallGaps = new();
    [SerializeField] private List<Transform> goalPotentialPositions = new();
    private readonly List<Vector3> _wallGapStartingPosition = new();

    private void Start()
    {
        foreach (Transform wallGap in wallGaps)
        {
            _wallGapStartingPosition.Add(wallGap.position);
        }
    }

    internal void OnEpisodeBegin()
    {
        for (int i = 0; i < wallGaps.Count; i++)
        {
            Transform wallGap = wallGaps[i];
            Vector3 position = wallGap.position;

            if (wallGap.transform.rotation.y != 0)
            {
                position = new Vector3(position.x, position.y,
                    _wallGapStartingPosition[i].z + Random.Range(wallGapRange.y, wallGapRange.x));
            }
            else
            {
                position = new Vector3(_wallGapStartingPosition[i].x + Random.Range(wallGapRange.y, wallGapRange.x),
                    position.y, position.z);
            }

            wallGap.position = position;
        }

        Vector3 goalPosition = goalPotentialPositions[0].position;
            // goalPotentialPositions[Random.Range(0, goalPotentialPositions.Count)].position;
        target.transform.position = new Vector3(goalPosition.x +
                                                Random.Range(Constants.RandomRangeMinPosition,
                                                    Constants.RandomRangeMaxPosition),
            goalPosition.y,
            goalPosition.z + Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition));
    }

    internal void AddTexture(bool succeeded)
    {
        foreach (Renderer groundAreasRenderer in groundAreasRenderers)
        {
            groundAreasRenderer.material.mainTexture = succeeded ? victoryTexture : failureTexture;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(agent.transform.position,
            agent.transform.position + agent.transform.forward * Constants.DrawGizmosDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(agent.transform.position, target.transform.position);
    }
}