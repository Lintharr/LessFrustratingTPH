using System;
using Harmony12;
using TH20;
using UnityEngine;

namespace LessFrustratingTPH
{
	[HarmonyPatch(typeof(GameAlgorithms), "CalculateRoomScore")]
	internal static class GameAlgorithms_CalculateRoomScore_Patch
	{
		private static bool Prefix(Character character, Room room, Room roomGoingTo, Vector3 position, ref float __result)
		{
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			if (!Main.IsModEnabled)
			{
				return true;
			}
			float num = 0f;
			float num2 = (float)room.QueueLength * GameAlgorithms.Config.RoomScoreQueueLength;
			if (room == roomGoingTo)
			{
				num2 = (float)room.PositionInQueue(character) * GameAlgorithms.Config.RoomScorePositionScore;
			}
			else
			{
				Level level = character.Level;
				if (Main.ModSettings.QueueWarningLengthMeansSomething && room.QueueLength >= level.HospitalPolicy.QueueWarningLength)
				{
					num = 30f;
				}
			}
			Vector3 val = (room.FloorPlan.Door != null) ? room.FloorPlan.Door.WorldPosition : room.Center;
			float num3 = Vector3.Distance(position, val) * GameAlgorithms.Config.RoomScoreDistanceFactor;
			float num4 = room.IsFullyStaffed() ? 0f : GameAlgorithms.Config.RoomScoreNotFullyStaffed;
			float num5 = room.IsFunctional() ? 0f : GameAlgorithms.Config.RoomScoreRoomNotFunctional;
			__result = num2 + num3 + num4 + num5 + num;
			return false;
		}
	}
}