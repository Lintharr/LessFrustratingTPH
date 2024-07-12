using System;
using System.Collections.Generic;
using Harmony12;
using TH20;

namespace LessFrustratingTPH
{
	[HarmonyPatch(typeof(JobApplicant), "AssignRandomQualifications")]
	internal class JobApplicant_AssignRandomQualifications_Patch
	{
		private class sortQualificationSlotsAscending : IComparer<QualificationSlot>
		{
			int IComparer<QualificationSlot>.Compare(QualificationSlot a, QualificationSlot b)
			{
				if (b.Definition == null)
				{
					return 1;
				}
				if (a.Definition == null)
				{
					return -1;
				}
				return a.Definition.NameLocalised.Term.CompareTo(b.Definition.NameLocalised.Term);
			}
		}

		private static List<QualificationSlot> getRequiredQualificationSlots(QualificationDefinition qualification)
		{
			List<QualificationSlot> list = new List<QualificationSlot>();
			for (int i = 0; i < qualification.RequiredQualifications.Length; i++)
			{
				list.Add(new QualificationSlot((QualificationDefinition)qualification.RequiredQualifications[i].Instance, complete: true));
				list.AddRange(getRequiredQualificationSlots((QualificationDefinition)qualification.RequiredQualifications[i].Instance));
			}
			return list;
		}

		private static void Postfix(JobApplicant __instance, WeightedList<QualificationDefinition> qualifications, Metagame metagame, Level level, int chanceOfEmptyTrainingSlot)
		{
			if (!Main.IsModEnabled && Main.ModSettings.ApplicantsQualifications.SpecializedQualifications)
				return;

			int num = __instance.MaxQualifications - 1;
			if (RandomUtils.GlobalRandomInstance.Next(0, 100) > chanceOfEmptyTrainingSlot)
				num++;

			__instance.Qualifications.Clear();
			if (num > 1)
			{
				WeightedList<QualificationDefinition> weightedList = new WeightedList<QualificationDefinition>();
				WeightedList<QualificationDefinition> weightedList2 = new WeightedList<QualificationDefinition>();
				WeightedList<QualificationDefinition> weightedList3 = new WeightedList<QualificationDefinition>();
				WeightedList<QualificationDefinition> weightedList4 = new WeightedList<QualificationDefinition>();
				foreach (KeyValuePair<QualificationDefinition, int> item in qualifications.List)
				{
					List<QualificationSlot> requiredQualificationSlots = getRequiredQualificationSlots(item.Key);
					if (item.Key.ValidFor(__instance.Definition._type, __instance.MaxQualifications, requiredQualificationSlots, metagame, level))
					{
						if (requiredQualificationSlots.Count == num - 1)
						{
							int num2 = item.Value;
							foreach (QualificationSlot item2 in requiredQualificationSlots)
							{
								num2 += qualifications.List[item2.Definition];
							}
							weightedList4.Add(item.Key, num2);
						}
						else if (item.Key.NameLocalised.Term.Contains("Radiology"))
							weightedList.Add(item.Key, item.Value);
						else if (item.Key.NameLocalised.Term.Contains("Genetics"))
							weightedList2.Add(item.Key, item.Value);
						else if (item.Key.NameLocalised.Term.Contains("Ghost"))
							weightedList3.Add(item.Key, item.Value);
						else if (item.Key.NameLocalised.Term.Contains("Speed"))
							weightedList3.Add(item.Key, item.Value);
					}
				}
				QualificationDefinition qualificationDefinition = weightedList4.Choose(null, RandomUtils.GlobalRandomInstance);
				if (qualificationDefinition != null)
				{
					List<QualificationSlot> requiredQualificationSlots2 = getRequiredQualificationSlots(qualificationDefinition);
					__instance.Qualifications.AddRange(requiredQualificationSlots2);
					if (qualificationDefinition.NameLocalised.Term.Contains("Diagnosis"))
					{
						//Main.Logger.Log("Is Diagnosis");
						weightedList.Add(qualificationDefinition, qualifications.List[qualificationDefinition]);
						__instance.Qualifications.Add(new QualificationSlot(weightedList.Choose(qualificationDefinition, RandomUtils.GlobalRandomInstance), complete: true));
					}
					else if (qualificationDefinition.NameLocalised.Term.Contains("Treatment"))
					{
						weightedList2.Add(qualificationDefinition, qualifications.List[qualificationDefinition]);
						__instance.Qualifications.Add(new QualificationSlot(weightedList2.Choose(qualificationDefinition, RandomUtils.GlobalRandomInstance), complete: true));
					}
					else if (qualificationDefinition.NameLocalised.Term.Contains("Mechanic") || qualificationDefinition.NameLocalised.Term.Contains("Maintenance"))
					{
						weightedList3.Add(qualificationDefinition, qualifications.List[qualificationDefinition]);
						__instance.Qualifications.Add(new QualificationSlot(weightedList3.Choose(qualificationDefinition, RandomUtils.GlobalRandomInstance), complete: true));
					}
					else
					{
						__instance.Qualifications.Add(new QualificationSlot(qualificationDefinition, complete: true));
					}
					__instance.Qualifications.Sort(new sortQualificationSlotsAscending());
				}
			}
			else if (num == 1)
			{
				WeightedList<QualificationDefinition> weightedList5 = new WeightedList<QualificationDefinition>();
				var qualificationsList = qualifications.List;
				if (Main.ModSettings.ApplicantsQualifications.DontRollUselessQualifications)
					qualificationsList.RemoveAll(x => x.Key.NameLocalised.Term.ContainsOneOf("Injection", "Pharmacy", "Happiness", "Training"));
				foreach (KeyValuePair<QualificationDefinition, int> item3 in qualificationsList)
				{
					if (item3.Key.ValidFor(__instance.Definition._type, __instance.MaxQualifications, __instance.Qualifications, metagame, level))
                        weightedList5.Add(item3.Key, item3.Value);
				}
				QualificationDefinition qualificationDefinition2 = weightedList5.Choose(null, RandomUtils.GlobalRandomInstance);
				if (qualificationDefinition2 != null)
				{
					__instance.Qualifications.Add(new QualificationSlot(qualificationDefinition2, complete: true));
				}
			}
		}
	}
}