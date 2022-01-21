using UnityEngine;

namespace TestGame
{
	[ExecuteInEditMode, DisallowMultipleComponent]
	public class Follower : MonoBehaviour
	{
		[SerializeField] private Transform followingTarget = null;
		[SerializeField] private bool onlyInEditMode = false;
		[SerializeField] private Vector3 offset;

		Transform target;

		public Transform FollowTarget
		{
			get { return followingTarget; }
			set { followingTarget = value; }
		}

		private void Awake()
		{
			target = transform;
		}

		private void Update()
		{
			if (!(onlyInEditMode && Application.isPlaying))
				Move();
		}

		public void Move()
		{
			if (FollowTarget)
				target.position = FollowTarget.position + offset;
			else
				Debug.Log("Target for following is not assigned!", this);
		}
	}
}