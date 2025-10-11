using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class UnitAgentCon : SerializedMonoBehaviour
{
	[SerializeField] private NavMeshAgent m_navMeshAgent = null;

	public float AttackRange => m_navMeshAgent.stoppingDistance;

	public void Initialize(float attackRange,float moveSpeed)
	{
		m_navMeshAgent.stoppingDistance = attackRange;
		m_navMeshAgent.speed = moveSpeed;
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
}