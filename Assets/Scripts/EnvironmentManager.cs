using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    internal AgentController AgentController;
    [SerializeField] private Renderer ground;
    [SerializeField] private Texture victoryTexture;
    [SerializeField] private Texture failureTexture;
    [SerializeField] private AgentController agent;
    [SerializeField] private GoalScript target;
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Transform[] bounds;

    [SerializeField] private Transform[] wallGaps;
    [SerializeField] internal Transform startingTransform;
    [SerializeField] private Vector2 wallGapRange = new Vector2(-22, 22);
    private List<Vector3> wallGapStartingPosition;
    private GameObject _key = null;

    internal void OnEpisodeBegin(bool rotateWall)
    {
        for (int i = 0; i < wallGaps.Length; i++)
        {
            Transform wallGap = wallGaps[i];
            Vector3 position = wallGap.position;
            Quaternion rotation = wallGap.rotation;

            if (rotateWall)
            {
                position = new Vector3(startingTransform.position.x,
                    startingTransform.position.y,
                    startingTransform.position.z + Random.Range(wallGapRange.y, wallGapRange.x));
                rotation.eulerAngles = new Vector3(0, 90, 0);
            }
            else
            {
                position = new Vector3(startingTransform.position.x + Random.Range(wallGapRange.y, wallGapRange.x),
                    startingTransform.position.y, startingTransform.position.z);
                rotation.eulerAngles = Vector3.zero;
            }

            wallGap.position = position;
            wallGap.rotation = rotation;
        }


        if (_key != null)
        {
            Destroy(_key.gameObject);
        }
        // Vector3 keyPosition = new Vector3(Random.Range(bounds[0].position.x, bounds[1].position.x), 0,
        //     Random.Range(bounds[0].position.z, bounds[1].position.z));
        // _key = Instantiate(keyPrefab, keyPosition, Quaternion.identity);
    }

    internal void AddTexture(bool succeeded)
    {
        ground.material.mainTexture = succeeded ? victoryTexture : failureTexture;
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