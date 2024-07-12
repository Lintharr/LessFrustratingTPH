//using System;
//using Harmony12;
//using TH20;

//namespace LessFrustratingTPH
//{
//    [HarmonyPatch(typeof(CharacterEvents), "OnStaffPromoted")]
//    internal static class CharacterEvents_OnStaffPromoted_Patch
//    {
//        private static CharacterEvents _instance;
//        private static Level _level;

//        private static void Postfix(CharacterEvents __instance, Level ____level)
//        {
//            try
//            {
//                if (!Main.enabled || !Main.settings.SetEveryNewIllnessPrice)
//                    return;

//                _instance = __instance;
//                _level = ____level;
//                //Execute();
//            }
//            catch (Exception ex)
//            {
//                Main.Logger.Log("Exception occured: " + ex);
//            }
//        }

//        public static void Execute()
//        {
//            try
//            {
//                if (_level != null && _instance != null)
//                {
//                    var priceModifiers = _level.FinanceManager.PriceModifiers;
//                    var discoveredIllnesses = _level.GameplayStatsTracker.DiscoveredIllnesses;
//                    foreach (var illness in discoveredIllnesses)
//                    {
//                        priceModifiers.SetModifier(illness, 100);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Main.Logger.Log("Exception occured: " + ex);
//            }
//        }
//    }
//}