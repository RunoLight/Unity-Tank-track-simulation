using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TestGame
{
    public class CameraBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform cameraT;
        [SerializeField] private Transform focusT;

        [SerializeField] private string scrollAxisName;

        [SerializeField] private float minDistance;
        [SerializeField] private float maxDistance;
        [SerializeField] private float scrollSpeed;
        [SerializeField] private float scrollDamping;

        [SerializeField] private float cameraRotationSpeed;
        [SerializeField] private float deathRotationRange;
        [SerializeField] private float angleDeceleration;
        [SerializeField] private float maxRotationSpeed;

        [SerializeField] KeyCode antiClockwiseRotationKey;
        [SerializeField] KeyCode clockwiseRotationKey;

        private float currentDistance;
        private float deltaAngle;

        public void Start()
        {
            currentDistance = minDistance + ((maxDistance - minDistance) / 2);
            cameraT.position = currentDistance * -cameraT.forward + focusT.position;
        }

        private void Update()
        {
            bool rotating = Rotate();

            deltaAngle = Mathf.Clamp(deltaAngle, -maxRotationSpeed, maxRotationSpeed);
            cameraT.RotateAround(focusT.position, Vector3.up, deltaAngle * Time.deltaTime);

            if (!rotating)
                Decelerate();

            SetPosition();
        }

        private bool Rotate()
        {
            if (Input.GetKey(antiClockwiseRotationKey))
            {
                deltaAngle -= cameraRotationSpeed * Time.deltaTime;
                return true;
            }
            if (Input.GetKey(clockwiseRotationKey))
            {
                deltaAngle += cameraRotationSpeed * Time.deltaTime;
                return true;
            }
            return false;
        }
        private void Decelerate()
        {
            deltaAngle = Mathf.Lerp(deltaAngle, 0, angleDeceleration);

            if (Mathf.Abs(deltaAngle) <= deathRotationRange)
                deltaAngle = 0;
        }

        private void SetPosition()
        {
            currentDistance -= Input.GetAxis(scrollAxisName) * scrollSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
            Vector3 desiredPos = currentDistance * -cameraT.forward + focusT.position;
            cameraT.position = Vector3.Lerp(cameraT.position, desiredPos, scrollDamping);
        }
    }
}
