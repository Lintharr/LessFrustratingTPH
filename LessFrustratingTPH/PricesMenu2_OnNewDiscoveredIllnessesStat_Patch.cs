using System;
using Harmony12;
using TH20;

namespace LessFrustratingTPH
{
    [HarmonyPatch(typeof(PricesMenu2), "OnNewDiscoveredIllnessesStat")]
    internal static class PricesMenu2_OnNewDiscoveredIllnessesStat_Patch
    {
        private static PricesMenu2 _instance;
        private static Level _level;

        private static void Postfix(PricesMenu2 __instance, Level ____level)
        {
            try
            {
                if (!Main.IsModEnabled || !Main.ModSettings.SetPriceOnEveryNewIllness)
                    return;

                _instance = __instance;
                _level = ____level;
                Execute();
            }
            catch (Exception ex)
            {
                Main.Logger.Log("Exception occured: " + ex);
            }
        }

        public static void Execute()
        {
            try
            {
                if (_level != null && _instance != null)
                {
                    var priceModifiers = _level.FinanceManager.PriceModifiers;
                    var discoveredIllnesses =_level.GameplayStatsTracker.DiscoveredIllnesses;
                    foreach (var illness in discoveredIllnesses)
                    {
                        priceModifiers.SetModifier(illness, Main.ModSettings.PriceOnEveryNewIllness);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Log("Exception occured: " + ex);
            }
        }
    }
}