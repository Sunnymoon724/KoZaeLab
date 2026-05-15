using System;
using System.Collections.Generic;
using KZLib.Actors;
using KZLib.Utilities;

namespace KZLib
{
	public class TeamRelationManager : Singleton<TeamRelationManager>
	{
		private static readonly int s_count = Enum.GetValues(typeof(TeamType)).Length-1;

		private readonly TeamRelationType[,] m_relationArray = new TeamRelationType[s_count,s_count];

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				Array.Clear(m_relationArray,0,m_relationArray.Length);
			}

			base._Release(disposing);
		}

		public void SetRelation(TeamType teamA,TeamType teamB,TeamRelationType relationAtoB,TeamRelationType relationBtoA = TeamRelationType.None)
		{
			if(!_TryConvertIndex(teamA,out var indexA) || !_TryConvertIndex(teamB,out var indexB))
			{
				return;
			}

			if(teamA == teamB)
			{
				m_relationArray[indexA,indexB] = TeamRelationType.Ally;

				return;
			}

			if(relationAtoB == TeamRelationType.None)
			{
				LogChannel.Game.E($"relationAtoB cannot be None : [{relationAtoB}]");

				return;
			}

			if(relationBtoA == TeamRelationType.None)
			{
				relationBtoA = relationAtoB;
			}

			m_relationArray[indexA,indexB] = relationAtoB;
			m_relationArray[indexB,indexA] = relationBtoA;
		}

		public IEnumerable<IActor> GetActorsByRelation(TeamType team,TeamRelationType relation,IEnumerable<IActor> actorGroup,bool includeDead = false)
		{
			foreach(var actor in actorGroup)
			{
				if((includeDead || !actor.IsDead) && GetRelation(team,actor.MyTeam) == relation)
				{
					yield return actor;
				}
			}
		}

		public IEnumerable<TeamType> GetTeamsByRelation(TeamType team,TeamRelationType relation)
		{
			if(!_TryConvertIndex(team,out var index))
			{
				yield break;
			}

			for(var i=0;i<s_count;i++)
			{
				if(m_relationArray[index,i] == relation)
				{
					yield return (TeamType) (i+1);
				}
			}
		}

		public TeamRelationType GetRelation(TeamType teamA,TeamType teamB)
		{
			if(!_TryConvertIndex(teamA,out var indexA) || !_TryConvertIndex(teamB,out var indexB))
			{
				return TeamRelationType.None;
			}

			var type = m_relationArray[indexA,indexB];

			if(type == TeamRelationType.None)
			{
				LogChannel.Game.E($"Team relation not set between [{teamA}] and [{teamB}]");

				return TeamRelationType.None;
			}

			return type;
		}

		private bool _TryConvertIndex(TeamType team,out int index)
		{
			if(team == TeamType.None)
			{
				index = -1;

				LogChannel.Game.E($"Invalid team type: [{team}]");

				return false;
			}

			index = (int) team-1;

			return true;
		}
	}
}
