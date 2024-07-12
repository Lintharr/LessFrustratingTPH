using System;
using System.Collections.Generic;
using System.Linq;
using Harmony12;
using TH20;

namespace LessFrustratingTPH
{
	[HarmonyPatch(typeof(CharacterTraitsManager), "GenerateRandomTraits")]
	internal class CharacterTraitsManager_GenerateRandomTraits_Patch
	{
		private static List<string> badCharacterTraits = new List<string>
		{
			"Hangry",
			"Nasty",
			"PayHigh",
			"ShortTemper",
			"ToiletRage",
			"Unmotivated",
			"Happiness_Lower",
			"Learn_Slow",
			"WeakBladder",
			"Unhygienic",
			"Litterer",
			"Narcolepsy",
			"Argumentative",
			"Boring",
			"Dirty",
			"Lazy",
			"Evil",
			"NauseaInducing"
		};

		private static bool Prefix(CharacterTraitsManager __instance, CharacterTraitsManager.Config ____config, ref CharacterTraits __result, WeightedList<CharacterTraitDefinition> ____traits, StaffDefinition.Type staffType, Character.Sex sex)
		{
			//Main.Logger.Log($"[LINTHAR - CharTraitsManager] In {nameof(Prefix)}. Main.enabled - {Main.enabled}. Main.settings.ReputationBasedTraits - {Main.settings.ReputationBasedTraits}.");
			if (!Main.IsModEnabled)
				return true;

			//Main.Logger.Log($"[LINTHAR - CharTraitsManager] Got through the exiting if.");
			List<CharacterTraitDefinition> finalCharacterTraitsList = new List<CharacterTraitDefinition>();
			WeightedList<CharacterTraitDefinition> filteredTraitsWList = new WeightedList<CharacterTraitDefinition>();

			int additionalTraitsAmount = Main.ModSettings.ApplicantsTraits.AdditionalTrait ? 1 : 0;
			int drawnCharacterTraitsAmount = RandomUtils.GlobalRandomInstance.Next(____config.GameplayTraitsMin + additionalTraitsAmount, ____config.GameplayTraitsMax + 1 + additionalTraitsAmount);
			int drawnCharacterFlavourTraitsAmount = RandomUtils.GlobalRandomInstance.Next(____config.FlavourTraitsMin, ____config.FlavourTraitsMax + 1);

			//Main.Logger.Log($"[LINTHAR - CharTraitsManager] About to hit first switch.");
            switch (Main.ModSettings.ApplicantsTraits.PositiveTraitsMode)
            {
				case Settings.TraitsMode.RandomPositiveTraits: //Random Positive Traits
				case Settings.TraitsMode.MaxPositiveTraits: //Max Positive Traits
					foreach (KeyValuePair<CharacterTraitDefinition, int> trait in ____traits.List)
					{
						bool isBadTrait = badCharacterTraits.Any(trait.Key.ShortNameLocalisedMale.Term.Contains);
						if (!isBadTrait)
							filteredTraitsWList.Add(trait.Key, trait.Value);
					}
					break;
				case Settings.TraitsMode.ReputationBasedTraits: //Reputation Based Traits
					foreach (KeyValuePair<CharacterTraitDefinition, int> trait in ____traits.List)
					{
						bool isBadTrait = badCharacterTraits.Any(trait.Key.ShortNameLocalisedMale.Term.Contains);
						if (JobApplicantPool_AddApplicant_Patch.reputation > 0.8f && !isBadTrait)
							filteredTraitsWList.Add(trait.Key, trait.Value);
						else if (JobApplicantPool_AddApplicant_Patch.reputation < 0.2f && isBadTrait)
							filteredTraitsWList.Add(trait.Key, trait.Value);
					}
					break;
				case Settings.TraitsMode.Off:
                default: 
					filteredTraitsWList = ____traits; 
					break;
            }

			//Main.Logger.Log($"[LINTHAR - CharTraitsManager] Hitting second switch.");
			switch (Main.ModSettings.ApplicantsTraits.PositiveTraitsMode)
			{
				case Settings.TraitsMode.MaxPositiveTraits: //Max Positive Traits
					foreach (KeyValuePair<CharacterTraitDefinition, int> positiveTrait in filteredTraitsWList.List)
					{
						CharacterTraitDefinition key = positiveTrait.Key;
						if (key != null && key.CanAdd(finalCharacterTraitsList) && key.IsValidFor(staffType))
							finalCharacterTraitsList.Add(key);
					}
					break;
				default:
				case Settings.TraitsMode.Off:
				case Settings.TraitsMode.RandomPositiveTraits: //Random Positive Traits
				case Settings.TraitsMode.ReputationBasedTraits: //Reputation Based Traits
					for (int i = 0; i < drawnCharacterTraitsAmount; i++)
					{
						CharacterTraitDefinition characterTraitDefinition = filteredTraitsWList.Choose(null, RandomUtils.GlobalRandomInstance);
						if (characterTraitDefinition != null && characterTraitDefinition.CanAdd(finalCharacterTraitsList) && characterTraitDefinition.IsValidFor(staffType))
							finalCharacterTraitsList.Add(characterTraitDefinition);
					}
					break;
			}

			//Main.Logger.Log($"[LINTHAR - CharTraitsManager] Hitting result.");
			__result = new CharacterTraits(finalCharacterTraitsList, ((CharacterFlavourTraits)____config.FlavourTraits.Instance).GenerateFlavour(drawnCharacterFlavourTraitsAmount, sex));
			//Main.Logger.Log($"[LINTHAR - CharTraitsManager] Returning.");
			return false;
		}
	}
}