using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tank.Movement
{
    public class TankWheel : MonoBehaviour
    {
        [SerializeField] private float couplingRadius;
        [SerializeField] private Transform connectedTrack;
        [SerializeField] private float velocityMultiplier = 5;

        private List<TrackPart> trackParts;
        private Collider wheelCollider;

        private void Awake()
        {
            trackParts = new List<TrackPart>(64);
            connectedTrack.GetComponentsInChildren(true, trackParts);
            wheelCollider = GetComponent<Collider>();
            wheelCollider.material = wheelCollider.material;
        }

        public void SetTorque(float torque)
        {
            var material = wheelCollider.material;

            if (torque == 0)
            {
                material.staticFriction = 10;
                material.dynamicFriction = 8;
                material.frictionCombine = PhysicMaterialCombine.Maximum;
            }
            else
            {
                material.staticFriction = 0;
                material.dynamicFriction = 0;
                material.frictionCombine = PhysicMaterialCombine.Minimum;
            }

            var nearParts = new List<TrackPart>(16);
            var sqrRadius = couplingRadius * couplingRadius;
            nearParts.AddRange(trackParts.Where(trackPart => Vector3.SqrMagnitude(trackPart.PhysicsPosition - transform.position) < sqrRadius));
            foreach (var trackPart in nearParts)
            {
                trackPart.Mark = true;
                trackPart.SetVelocity(torque * velocityMultiplier / nearParts.Count);
            }
        }

        private void OnDrawGizmosSelected()
        {
            var c = Color.red;
            c.a = 0.5F;
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position, couplingRadius);
        }

        private void OnValidate()
        {
            if (couplingRadius < 0.01F)
                couplingRadius = 0.01F;
        }
    }
}
