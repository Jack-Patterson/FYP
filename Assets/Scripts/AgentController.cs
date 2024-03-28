using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentController : Agent
{
    [SerializeField] private EnvironmentManager environmentManager;
    private Vector3 _agentStartingTransformPosition;
    private Quaternion _agentStartingTransformRotation;
    public bool IsLookingAtTarget { get; private set; } = false;

    private void Start()
    {
        Transform agentTransform = transform;
        _agentStartingTransformPosition = agentTransform.position;
        _agentStartingTransformRotation = agentTransform.rotation;
    }

    private void Update()
    {
        if (MaxStep - StepCount < 10)
        {
            SetReward(-1f);
            environmentManager.AddTexture(false);
            EndEpisode();
        }
    }

    private IEnumerator PrintObservations()
    {
        yield return new WaitForSeconds(Constants.PrintWaitTime);
        print(transform.position);
        print(transform.rotation);
        foreach (float observation in GetObservations())
        {
            print(observation);
        }
    }

    private void Move(float zMovement, float xMovement)
    {
        Vector3 directionToMove = new Vector3(xMovement, 0, zMovement);
        transform.Translate(directionToMove.normalized * (Constants.CharacterMoveSpeed * Time.deltaTime));
    }

    private void Rotate(float xRotation)
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
        Vector3 playerToTarget = environmentManager.Target.transform.position - transform.position;
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
        transform.position = new Vector3(_agentStartingTransformPosition.x + Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition),
            _agentStartingTransformPosition.y, _agentStartingTransformPosition.z + Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition));
        transform.rotation = Quaternion.Euler(_agentStartingTransformRotation.x, 
            Random.Range(Constants.RandomRangeMinRotation, Constants.RandomRangeMaxRotation), _agentStartingTransformRotation.z);
        
        environmentManager.OnEpisodeBegin();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float zMovement = actions.ContinuousActions[0];
        float xMovement = actions.ContinuousActions[1];
        float xRotation = actions.ContinuousActions[2];

        Move(zMovement, xMovement);
        Rotate(xRotation);

        AddReward(Constants.MaxStepDividend/MaxStep);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(false);
        sensor.AddObservation(0);
        sensor.AddObservation(0);
        sensor.AddObservation(Vector3.Distance(transform.position, environmentManager.Target.position));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        (continuousActions[0], continuousActions[1]) = (Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<ITriggerableObject>()?.Trigger(this);
    }
}