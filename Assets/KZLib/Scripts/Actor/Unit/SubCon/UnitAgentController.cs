namespace KZLib.Actors
{
	public class UnitAgentController : ActorAgentController
	{
		public float AttackRange => m_navMeshAgent.stoppingDistance;

		public void Initialize(float attackRange,float moveSpeed,bool updateRotation = false)
		{
			m_navMeshAgent.stoppingDistance = attackRange;

			Initialize(moveSpeed,updateRotation);
		}
	}
}