using System;
using System.Collections.Generic;
using KZLib.Actors;
using KZLib.Utilities;

namespace KZLib
{
	public class SideRelationManager : Singleton<SideRelationManager>
	{
		private static readonly int s_count = Enum.GetValues(typeof(SideType)).Length-1;

		private readonly SideRelationType[,] m_relationArray = new SideRelationType[s_count,s_count];

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				Array.Clear(m_relationArray,0,m_relationArray.Length);
			}

			base._Release(disposing);
		}

		public void SetRelation(SideType sideA,SideType sideB,SideRelationType relation1,SideRelationType relation2 = SideRelationType.None)
		{
			if(!_TryConvertIndex(sideA,sideB,out var indexA,out var indexB))
			{
				return;
			}

			if(sideA == sideB)
			{
				m_relationArray[indexA,indexB] = SideRelationType.Ally;

				return;
			}

			if(relation1 == SideRelationType.None)
			{
				LogChannel.Game.E($"relation1 cannot be None : [{relation1}]");

				return;
			}

			if(relation2 == SideRelationType.None)
			{
				relation2 = relation1;
			}

			m_relationArray[indexA,indexB] = relation1;
			m_relationArray[indexB,indexA] = relation2;
		}

		public IEnumerable<IActor> GetActorGroup(SideType mySide,SideRelationType relation,IEnumerable<IActor> actorGroup,bool includeDead = false)
		{
			foreach(var actor in actorGroup)
			{
				if((includeDead || !actor.IsDead) && GetRelation(mySide,actor.MySide) == relation)
				{
					yield return actor;
				}
			}
		}

		public SideRelationType GetRelation(SideType sideA,SideType sideB)
		{
			if(!_TryConvertIndex(sideA,sideB,out var indexA,out var indexB))
			{
				return SideRelationType.None;
			}

			var type = m_relationArray[indexA,indexB];

			if(type == SideRelationType.None)
			{
				type = m_relationArray[indexB,indexA];

				if(type == SideRelationType.None)
				{
					LogChannel.Game.E($"Side relation not set between [{sideA}] and [{sideB}]");

					return SideRelationType.None;
				}
			}

			return type;
		}

		private bool _TryConvertIndex(SideType sideA,SideType sideB,out int indexA,out int indexB)
		{
			if(sideA == SideType.None || sideB == SideType.None)
			{
				indexA = -1;
				indexB = -1;

				LogChannel.Game.E($"Invalid side types: [{sideA}] and [{sideB}]");

				return false;
			}

			indexA = (int) sideA-1;
			indexB = (int) sideB-1;

			return true;
		}
	}
}
