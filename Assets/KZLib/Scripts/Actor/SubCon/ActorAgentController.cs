using UnityEngine;
using UnityEngine.AI;

namespace KZLib.Actors
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class ActorAgentController : MonoBehaviour
	{
		[SerializeField]
		protected NavMeshAgent m_navMeshAgent = null;

		public void Initialize(float moveSpeed,bool updateRotation = false)
		{
			m_navMeshAgent.speed = moveSpeed;
			m_navMeshAgent.updateRotation = updateRotation;
		}

		public void SetDestination(Vector3 destination)
		{
			m_navMeshAgent.SetDestination(destination);
		}

		public void Stop()
		{
			m_navMeshAgent.ResetPath();
			m_navMeshAgent.velocity = Vector3.zero;
		}

		public void RotateToTarget(Vector3 targetPosition,float deltaTime)
		{
			var direction = targetPosition-transform.position;

			direction.y = 0.0f;

			if(direction.sqrMagnitude < Global.MinSqrMagnitude)
			{
				return;
			}

			var targetRotation = Quaternion.LookRotation(direction);

			transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,m_navMeshAgent.angularSpeed*deltaTime);
		}

		private void Reset()
		{
			if(!m_navMeshAgent)
			{
				m_navMeshAgent = GetComponent<NavMeshAgent>();
			}
		}
	}
}