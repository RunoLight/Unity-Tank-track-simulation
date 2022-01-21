using UnityEngine;

namespace Tank.Movement
{
    [ExecuteAlways]
    public class TrackPart : MonoBehaviour
    {
        private HingeJoint joint;
        private Rigidbody partRb;

        [SerializeField] private float force;
        [SerializeField] private float anchorNodeIdent;

        public bool Mark { get; set; }

        public Vector3 PhysicsPosition => partRb.position;

        private void Awake()
        {
            joint = GetComponent<HingeJoint>();
            partRb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            joint.axis = transform.right;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = Vector3.back * anchorNodeIdent;
            joint.anchor = Vector3.forward * anchorNodeIdent;
        }

        public void Setup(TrackPart next)
        {
            joint.connectedBody = next.partRb;
        }

        public void SetVelocity(float velocityMagnitude)
        {
            var velocityDir = velocityMagnitude * transform.forward;
            partRb.AddForce(velocityDir, ForceMode.VelocityChange);
        }

        private void OnDrawGizmos()
        {
            if (!Mark) return;

            var c = Color.red;
            c.a = 0.25F;
            Gizmos.color = c;
            Gizmos.DrawCube(partRb.position, GetComponentInChildren<MeshRenderer>().transform.localScale);

            Mark = false;
        }
    }
}
