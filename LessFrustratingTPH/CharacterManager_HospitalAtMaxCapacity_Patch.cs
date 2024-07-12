using Harmony12;
using TH20;

namespace LessFrustratingTPH
{
    [HarmonyPatch(typeof(CharacterManager), "HospitalAtMaxCapacity")]
    internal static class CharacterManager_HospitalAtMaxCapacity_Patch
    {
        private static bool Prefix(ref CharacterManager __instance, ref bool __result)
        {
            if (!Main.IsModEnabled)
                return true;

            __result = (__instance.Patients.Count >= Main.ModSettings.MaxPatients);
            return false;
        }
    }
}