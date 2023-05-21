using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts;
using MoreMountains.Tools;
using UnityEngine;

public class SwarmMovementController : MonoBehaviour, IShipActionController
{
    public float movementTargetDistance;
    public float movementTargetRotation;
    public Transform swarmMovementTarget;
    public List<SteeringMovement> dronesMovementSystems;

    public SteeringBehaviorPreset leaderPreset;
    public SteeringBehaviorPreset regularDronePreset;

    public Vector2[] swarmFormationOffsets;
    
    public ShipActionControllerType ControllerType => ShipActionControllerType.MovementController;
    private ShipOrder currentOrder;
    void Start()
    {
        currentOrder = new ShipOrder();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentOrder != null)
        {
            if (currentOrder.movementOrderType == MovementOrderType.Direct)
            {
                /*var moveDir = ((Vector2)transform.up).MMRotate(movementTargetRotation * currentOrder.movement.x);
                moveDir = moveDir.normalized * movementTargetDistance;
                swarmMovementTarget.position = (Vector2)transform.position + moveDir;*/
                
                
                var centerOfMass = Vector2.zero;
                List<Rigidbody2D> droneBodies = new List<Rigidbody2D>();
                foreach (var drone in dronesMovementSystems)
                {
                    if (drone != null)
                    {
                        droneBodies.Add(drone.Body);

                        centerOfMass += drone.Body.position;
                    }
                }

                centerOfMass /= droneBodies.Count;

                var leader = droneBodies[0];

                for (int i = 0; i < dronesMovementSystems.Count; i++)
                {
                    var drone = dronesMovementSystems[i];
                    
                    if (drone.Body == leader)
                    {
                        drone.steeringBehaviorPreset = leaderPreset;
                    }
                    else
                    {
                        drone.steeringBehaviorPreset = regularDronePreset;

                        drone.SteeringData.offsetPursuit = swarmFormationOffsets[i - 1];
                    }

                    drone.SteeringData.seekTarget = currentOrder.aim;
                    drone.SteeringData.fleeTarget = currentOrder.aim;
                    drone.SteeringData.arriveTarget = currentOrder.aim;

                    drone.SteeringData.neighbors = droneBodies;
                    drone.SteeringData.leader = leader;

                    drone.ProcessMovement();
                }
            }
        }
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(20, 30, 200, 200), 
            $"max speed = {dronesMovementSystems[0].MovementSystem.MaxSpeed},\n speed = {dronesMovementSystems[0].GetComponent<Rigidbody2D>().velocity.magnitude}\n" +
            $"calculated acceleration = {dronesMovementSystems[0].MovementSystem.CalculatedAcceleration}\n" +
            $"angular velocity = {dronesMovementSystems[0].GetComponent<Rigidbody2D>().angularVelocity}\n" +
            $"Temperature = {dronesMovementSystems[0].GetComponent<Shell>().Temperature}");
    }
}
