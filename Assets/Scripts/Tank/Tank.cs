using Tank.Movement;
using UnityEngine;

namespace Tank
{
    public class Tank : MonoBehaviour
    {
        [SerializeField] private TankWheel[] leftWheels;
        [SerializeField] private TankWheel[] rightWheels;

        private float rightTrackWheelTorque;
        private float leftTrackWheelTorque;

        private void Awake()
        {
            Time.fixedDeltaTime = 0.005F;
        }

        private void Update()
        {
            var verticalAxis = Input.GetAxis("Vertical");
            var horizontalAxis = Input.GetAxis("Horizontal");

            rightTrackWheelTorque = verticalAxis;
            leftTrackWheelTorque = verticalAxis;

            if (horizontalAxis != 0)
            {
                rightTrackWheelTorque = -horizontalAxis;
                leftTrackWheelTorque = horizontalAxis;
            }
        }

        private void FixedUpdate()
        {
            foreach (var wheel in rightWheels)
                wheel.SetTorque(rightTrackWheelTorque);

            foreach (var wheel in leftWheels)
                wheel.SetTorque(leftTrackWheelTorque);
        }
    }
}
