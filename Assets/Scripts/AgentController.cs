using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentController : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform[] keysPositions;
    private int _keys = 0;
    private Vector3 _agentStartingTransformPosition;
    private Quaternion _agentStartingTransformRotation;
    private Vector3 _targetStartingTransformPosition;

    public bool IsLookingAtTarget { get; private set; } = false;
    [SerializeField] private EnvironmentManager environmentManager;

    private void Start()
    {
        _agentStartingTransformPosition = transform.position;
        _agentStartingTransformRotation = transform.rotation;
        _targetStartingTransformPosition = target.transform.position;
    }

    private IEnumerator PrintObservations()
    {
        yield return new WaitForSeconds(Constants.PrintWaitTime);
        print(transform.position);
        print(transform.rotation);
        print(target.transform.position);
        foreach (float observation in GetObservations())
        {
            print(observation);
        }
    }

    protected void Move(float zMovement, float xMovement)
    {
        Vector3 directionToMove = new Vector3(xMovement, 0, zMovement);
        transform.Translate(directionToMove.normalized * (Constants.CharacterMoveSpeed * Time.deltaTime));
    }

    protected void Rotate(float xRotation)
    {
        float rotationDirection = xRotation * Constants.CharacterRotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationDirection);

        if (CheckIfLookingAtTarget() && !IsLookingAtTarget)
        {
            IsLookingAtTarget = true;
        }
    }

    private bool CheckIfLookingAtTarget()
    {
        Vector3 playerToTarget = target.transform.position - transform.position;
        float dotProduct = Vector3.Dot(playerToTarget.normalized, transform.forward.normalized);

        if (dotProduct > Constants.DotProductAmount)
        {
            return true;
        }

        IsLookingAtTarget = false;
        return false;
    }

    public override void OnEpisodeBegin()
    {
        float randomRange = Random.Range(0f, 1f);
        bool shouldRotateWall = randomRange >= 0.5f;

        Vector3 agentStartingPosition, targetStartingPosition;
        Vector3 startingPosition = environmentManager.startingTransform.position;
        if (shouldRotateWall)
        {
            (agentStartingPosition, targetStartingPosition) = (
                new Vector3(_agentStartingTransformPosition.x + 12, 0, startingPosition.z),
                new Vector3(_targetStartingTransformPosition.x - 12, 0, startingPosition.z));
        }
        else
        {
            (agentStartingPosition, targetStartingPosition) =
                (_agentStartingTransformPosition, _targetStartingTransformPosition);
        }
        
        (agentStartingPosition, targetStartingPosition) =
            (_agentStartingTransformPosition, _targetStartingTransformPosition);

        (agentStartingPosition, targetStartingPosition) = Random.Range(0f, 1f) >= 0.5f
            ? (targetStartingPosition, agentStartingPosition)
            : (agentStartingPosition, targetStartingPosition);

        transform.position = new Vector3(
            agentStartingPosition.x + Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition),
            agentStartingPosition.y,
            agentStartingPosition.z + Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition));
        ;
        transform.rotation = Quaternion.Euler(_agentStartingTransformRotation.x,
            Random.Range(Constants.RandomRangeMinRotation, Constants.RandomRangeMaxRotation),
            _agentStartingTransformRotation.z);
        target.transform.position = new Vector3(targetStartingPosition.x +
                                                Random.Range(Constants.RandomRangeMinPosition,
                                                    Constants.RandomRangeMaxPosition),
            targetStartingPosition.y,
            targetStartingPosition.z +
            Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition));
        environmentManager.OnEpisodeBegin(shouldRotateWall, agentStartingPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float zMovement = actions.ContinuousActions[0];
        float xMovement = actions.ContinuousActions[1];
        float xRotation = actions.ContinuousActions[2];

        Move(zMovement, xMovement);
        Rotate(xRotation);

        AddReward(Constants.MaxStepDividend / MaxStep);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(_keys);
        sensor.AddObservation(0);
        sensor.AddObservation(0);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        (continuousActions[0], continuousActions[1]) = (Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<ITriggerableObject>()?.Trigger(this);
        if (other.GetComponent<ITriggerableObject>() is Key)
        {
            _keys++;
        }
    }
}