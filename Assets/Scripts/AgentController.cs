using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentController : Agent
{
    [SerializeField] private Transform target;
    private Vector3 _agentStartingTransformPosition;
    private Quaternion _agentStartingTransformRotation;
    private Vector3 _targetStartingTransformPosition;
    
    private RayPerceptionSensorComponent3D _rayPerceptionSensorUpper;
    private RayPerceptionSensorComponent3D _rayPerceptionSensorMiddle;
    private RayPerceptionSensorComponent3D _rayPerceptionSensorLower;

    public bool IsLookingAtTarget { get; private set; } = false;
    [SerializeField] private EnvironmentManager environmentManager;

    private readonly float[] _emptyValues = new float[70];

    private void Start()
    {
        _agentStartingTransformPosition = transform.position;
        _agentStartingTransformRotation = transform.rotation;
        _targetStartingTransformPosition = target.transform.position;

        RayPerceptionSensorComponent3D[] sensorComponents = GetComponents<RayPerceptionSensorComponent3D>();
        foreach (RayPerceptionSensorComponent3D raySensor in sensorComponents)
        {
            if (raySensor.SensorName.Equals("RayPerceptionSensorUpper")) _rayPerceptionSensorUpper = raySensor;
            else if (raySensor.SensorName.Equals("RayPerceptionSensorMiddle")) _rayPerceptionSensorMiddle = raySensor;
            else if (raySensor.SensorName.Equals("RayPerceptionSensorLower")) _rayPerceptionSensorLower = raySensor;
            else Debug.LogError("RayPerceptionSensorComponent3D of unknown name found. Name: " + raySensor.SensorName);
        }
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
        transform.position = new Vector3(_agentStartingTransformPosition.x + Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition),
            _agentStartingTransformPosition.y, _agentStartingTransformPosition.z + Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition));;
        transform.rotation = Quaternion.Euler(_agentStartingTransformRotation.x, 
            Random.Range(Constants.RandomRangeMinRotation, Constants.RandomRangeMaxRotation), _agentStartingTransformRotation.z);
        target.transform.position = new Vector3(_targetStartingTransformPosition.x + 
                                                Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition), 
            _targetStartingTransformPosition.y,
            _targetStartingTransformPosition.z + Random.Range(Constants.RandomRangeMinPosition, Constants.RandomRangeMaxPosition));
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
    }
}