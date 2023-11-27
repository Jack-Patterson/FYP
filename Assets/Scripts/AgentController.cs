using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentController : Agent
{
    private const float CharacterMoveSpeed = 7f;
    private const float CharacterRotateSpeed = 80f;

    [SerializeField] private Transform target;
    private Vector3 _agentStartingTransformPosition;
    private Quaternion _agentStartingTransformRotation;
    private Vector3 _targetStartingTransformPosition;

    private bool _isLookingAtTarget = false;

    private void Start()
    {
        _agentStartingTransformPosition = transform.position;
        _agentStartingTransformRotation = transform.rotation;
        _targetStartingTransformPosition = target.transform.position;
        print("start");
        // StartCoroutine(PrintObservations());
    }

    private IEnumerator PrintObservations()
    {
        yield return new WaitForSeconds(10f);
        print(transform.position);
        print(transform.rotation);
        print(target.transform.position);
        foreach (float observation in GetObservations())
        {
            print(observation);
        }
    }

    private void Move(float zMovement)
    {
        Vector3 directionToMove = Vector3.forward * zMovement;
        transform.Translate(directionToMove * (CharacterMoveSpeed * Time.deltaTime));
    }

    private void Rotate(float xRotation)
    {
        float rotationDirection = xRotation * CharacterRotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationDirection);

        if (CheckIfLookingAtTarget() && !_isLookingAtTarget)
        {
            AddReward(0.1f);
            _isLookingAtTarget = true;
        }
    }

    private bool CheckIfLookingAtTarget()
    {
        Vector3 playerToTarget = target.transform.position - transform.position;
        float dotProduct = Vector3.Dot(playerToTarget.normalized, transform.forward.normalized);

        if (dotProduct > 0.8f)
        {
            return true;
        }
        _isLookingAtTarget = false;
        return false;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(_agentStartingTransformPosition.x + Random.Range(-3f, 3f),
            _agentStartingTransformPosition.y, _agentStartingTransformPosition.z + Random.Range(-3f, 3f));;
        transform.rotation = Quaternion.Euler(_agentStartingTransformRotation.x, Random.Range(-120f, 120f), _agentStartingTransformRotation.z);
        target.transform.position = new Vector3(_targetStartingTransformPosition.x + Random.Range(-3f, 3f), _targetStartingTransformPosition.y,
            _targetStartingTransformPosition.z + Random.Range(-3f, 3f));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float zMovement = actions.ContinuousActions[0];
        float xRotation = actions.ContinuousActions[1];

        Move(zMovement);
        Rotate(xRotation);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(target.position);
        sensor.AddObservation(_isLookingAtTarget);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxis("Vertical");
        continuousActions[1] = Input.GetAxis("Horizontal");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GoalScript>())
        {
            AddReward(1);
            EnvironmentManager.Instance.AddTexture(true);
            EndEpisode();
        }
        else if (other.GetComponent<WallScript>())
        {
            AddReward(-1);
            EnvironmentManager.Instance.AddTexture(false);
            EndEpisode();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        
        Gizmos.color = Color.red;
        Vector3 transformToTarget = transform.position - target.transform.position;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}