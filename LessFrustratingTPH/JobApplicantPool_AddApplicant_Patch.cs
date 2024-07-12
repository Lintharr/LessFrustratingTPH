using System;
using System.Collections.Generic;
using System.Linq;
using Harmony12;
using TH20;

namespace LessFrustratingTPH
{
	[HarmonyPatch(typeof(JobApplicantPool), "AddApplicant")]
	internal class JobApplicantPool_AddApplicant_Patch
	{
		public static float reputation = 1f;
		public static WeightedList<int> OriginalRankWeights = new WeightedList<int>();

		public static int PrestigeTrackerLvl = 0;

        private static bool Prefix(JobApplicantPool __instance, float[] recruitmentFeePercentage, WeightedList<QualificationDefinition> qualifications, CharacterTraitsManager traitsManager, Metagame metagame, Level level, WeightedList<int> ____rankWeights, StaffDefinition ____staffDefinition, CharacterNameGenerator ____nameGenerator, JobApplicantPool.Config ____config, JobApplicantManager ____jobApplicantManager)
		{
			if (!Main.IsModEnabled)
				return true;

			reputation = level.ReputationTracker.OverallReputation;
			OriginalRankWeights = ____rankWeights;
			PrestigeTrackerLvl = level.PrestigeTracker.Level;

			WeightedList<int> weightedList = new WeightedList<int>();
            switch (Main.ModSettings.ApplicantsQualifications.ApplicantsQualificationsMode)
            {
                case Settings.ApplicantQualificationsMode.NewHiresWithOnlyNoneOrOneQual:
                {
					____rankWeights.List.Where(x => x.Key == 0/* || x.Key == 1*/).Do(x => weightedList.Add(x.Key, x.Value));
					break;
                }
				case Settings.ApplicantQualificationsMode.HospitalLevelBasedQualifications:
                {
					int lowestRankLimiter = level.PrestigeTracker.Level / 16;
					int highestRankLimiter = level.PrestigeTracker.Level / 6;
					foreach (KeyValuePair<int, int> item in ____rankWeights.List)
					{
						if (item.Key >= lowestRankLimiter && item.Key <= highestRankLimiter)
							weightedList.Add(item.Key, item.Value);
					}
					break;
                }
				case Settings.ApplicantQualificationsMode.Off:
                default:
                {
					weightedList = ____rankWeights;
                    break;
                }
			}
			//Main.Logger.Log($"[AddApplicant] {string.Join(" | ", weightedList.List.Select(x => $"[Key: {x.Key}, Value: {x.Value}]"))} (type: {____staffDefinition._type}).");

			int applicantsRank = weightedList.Choose(0, RandomUtils.GlobalRandomInstance);
			JobApplicant jobApplicant = new JobApplicant(____staffDefinition, ____nameGenerator, recruitmentFeePercentage[applicantsRank], ____config.ChanceOfEmptyTrainingSlot, applicantsRank, qualifications, traitsManager, metagame, level);
			__instance.Applicants.Add(jobApplicant);
			____jobApplicantManager.OnJobApplicantAdded.InvokeSafe(__instance, jobApplicant);
			return false;
		}
	}
}
