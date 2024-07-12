using System;
using System.Collections.Generic;
using System.Linq;
using Harmony12;
using TH20;

namespace LessFrustratingTPH
{
    [HarmonyPatch(typeof(TrainingMenu), "CalculateAvailableCourses")]
    internal static class TrainingMenu_CalculateAvailableCourses_Patch
    {
        private static TrainingMenu _instance;
        private static Level _level;
        private static List<QualificationDefinition> _availableCourses;

        private static void Postfix(TrainingMenu __instance, Level ____level, ref List<QualificationDefinition> ____availableCourses)
        {
            try
            {
                if (!Main.IsModEnabled || !Main.ModSettings.ReorderTrainingCourses)
                    return;

                _instance = __instance;
                _level = ____level;
                _availableCourses = ____availableCourses;

                //Main.Logger.Log($"{_level.JobApplicantManager.Qualifications.List.Keys.Select(x => x.NameLocalised.ToAnalyticsTermString()).ListThis("All qualifications", true)}");

                if (!string.IsNullOrWhiteSpace(Main.ModSettings.TrainingCoursesOrder))
                {
                    string[] orderedCourseAnalyticalTerms = Main.ModSettings.TrainingCoursesOrder.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    _sortingOrder = orderedCourseAnalyticalTerms.ToDictionary(x => x, y => orderedCourseAnalyticalTerms.IndexOf(y));
                    //Main.Logger.Log($"[TrainingMenu] {_sortingOrder.Select(x => $"['{x.Key}', {x.Value}]").ListThis("New order of training menu registered", true, " | ")}.");
                }
                //TODO: setting for removing useless qualifications

                Execute();
            }
            catch (Exception ex)
            {
                Main.Logger.Log("Exception occured: " + ex);
            }
        }


        private static Dictionary<string, int> _sortingOrder = new Dictionary<string, int>()
        {
            { "Doctor_GeneralPractice_5_Name", 1 },
            { "Doctor_GeneralPractice_4_Name", 2 },
            { "Doctor_GeneralPractice_3_Name", 3 },
            { "Doctor_GeneralPractice_2_Name", 4 },
            { "Doctor_GeneralPractice_1_Name", 5 },
            { "Doctor_Diagnosis_5_Name", 6 },
            { "Doctor_Diagnosis_4_Name", 7 },
            { "Doctor_Diagnosis_3_Name", 8 },
            { "Doctor_Diagnosis_2_Name", 9 },
            { "Doctor_Diagnosis_1_Name", 10 },
            { "Doctor_Treatment_5_Name", 11 },
            { "Doctor_Treatment_4_Name", 12 },
            { "Doctor_Treatment_3_Name", 13 },
            { "Doctor_Treatment_2_Name", 14 },
            { "Doctor_Treatment_1_Name", 15 },
            { "Nurse_WardManagement_5_Name", 16 },
            { "Nurse_WardManagement_4_Name", 17 },
            { "Nurse_WardManagement_3_Name", 18 },
            { "Nurse_WardManagement_2_Name", 19 },
            { "Nurse_WardManagement_1_Name", 20 },
            { "Doctor_Genetics_1_Name", 21 },
            { "Doctor_Radiology_1_Name", 22 },
            { "Doctor_Psychiatry_5_Name", 23 },
            { "Doctor_Psychiatry_4_Name", 24 },
            { "Doctor_Psychiatry_3_Name", 25 },
            { "Doctor_Psychiatry_2_Name", 26 },
            { "Doctor_Psychiatry_1_Name", 27 },
            { "Doctor_Surgery_5_Name", 28 },
            { "Doctor_Surgery_4_Name", 29 },
            { "Doctor_Surgery_3_Name", 30 },
            { "Doctor_Surgery_2_Name", 31 },
            { "Doctor_Surgery_1_Name", 32 },
            { "Janitor_Mechanics_5_Name", 33 },
            { "Janitor_Mechanics_4_Name", 34 },
            { "Janitor_Mechanics_3_Name", 35 },
            { "Janitor_Mechanics_2_Name", 36 },
            { "Janitor_Mechanics_1_Name", 37 },
            { "Janitor_Maintenance_5_Name", 38 },
            { "Janitor_Maintenance_4_Name", 39 },
            { "Janitor_Maintenance_3_Name", 40 },
            { "Janitor_Maintenance_2_Name", 41 },
            { "Janitor_Maintenance_1_Name", 42 },
            { "Janitor_GhostCapture_1_Name", 43 },
            { "General_Speed_1_Name", 44 },
            { "Janitor_VehicleMechanics_5_Name", 45 },
            { "Janitor_VehicleMechanics_4_Name", 46 },
            { "Janitor_VehicleMechanics_3_Name", 47 },
            { "Janitor_VehicleMechanics_2_Name", 48 },
            { "Janitor_VehicleMechanics_1_Name", 49 },
            { "Assistant_Service_5_Name", 50 },
            { "Assistant_Service_4_Name", 51 },
            { "Assistant_Service_3_Name", 52 },
            { "Assistant_Service_2_Name", 53 },
            { "Assistant_Service_1_Name", 54 },
            { "Assistant_Marketing_5_Name", 55 },
            { "Assistant_Marketing_4_Name", 56 },
            { "Assistant_Marketing_3_Name", 57 },
            { "Assistant_Marketing_2_Name", 58 },
            { "Assistant_Marketing_1_Name", 59 },
            { "Assistant_TimeTunnel_5_Name", 60 },
            { "Assistant_TimeTunnel_4_Name", 61 },
            { "Assistant_TimeTunnel_3_Name", 62 },
            { "Assistant_TimeTunnel_2_Name", 63 },
            { "Assistant_TimeTunnel_1_Name", 64 },
            { "Doctor_Research_5_Name", 65 },
            { "Doctor_Research_4_Name", 66 },
            { "Doctor_Research_3_Name", 67 },
            { "Doctor_Research_2_Name", 68 },
            { "Doctor_Research_1_Name", 69 },
            { "General_Training_1_Name", 70 },
            { "General_PatientHappiness_1_Name", 71 },
            { "General_Happiness_1_Name", 72 },
            { "General_Energy_1_Name", 73 },
            { "Nurse_Pharmacy_1_Name", 74 },
            { "Nurse_Injections_1_Name", 75 },
            { "Doctor_Driving_5_Name", 76 },
            { "Doctor_Driving_4_Name", 77 },
            { "Doctor_Driving_3_Name", 78 },
            { "Doctor_Driving_2_Name", 79 },
            { "Doctor_Driving_1_Name", 80 },
            { "Doctor_Flying_5_Name", 81 },
            { "Doctor_Flying_4_Name", 82 },
            { "Doctor_Flying_3_Name", 83 },
            { "Doctor_Flying_2_Name", 84 },
            { "Doctor_Flying_1_Name", 85 },
        };

        private static int Sort(QualificationDefinition main, QualificationDefinition other)
        {
            if (main == null && other == null)
                return 0;
            if (main == null)
                return -1;
            if (other == null)
                return 1;

            if (_sortingOrder.TryGetValueThrowException(main.NameLocalised.ToAnalyticsTermString()) > _sortingOrder.TryGetValueThrowException(other.NameLocalised.ToAnalyticsTermString()))
                return 1;
            if (_sortingOrder.TryGetValueThrowException(main.NameLocalised.ToAnalyticsTermString()) < _sortingOrder.TryGetValueThrowException(other.NameLocalised.ToAnalyticsTermString()))
                return -1;
            if (_sortingOrder.TryGetValueThrowException(main.NameLocalised.ToAnalyticsTermString()) == _sortingOrder.TryGetValueThrowException(other.NameLocalised.ToAnalyticsTermString()))
                return 0;

            throw new ArgumentException();
        }

        public static void Execute()
        {
            try
            {
                var theNames = string.Join(", ", _availableCourses.Select(x => x.NameLocalised));
                var theTermNames = string.Join(", ", _availableCourses.Select(x => x.NameLocalised.Term));
                //_availableCourses[0].NameLocalised;
                //_availableCourses[0].StaffType;
                //Main.Logger.Log($"[LINTHAR - PricesMenuStatsTracker] Course names (Count: {_availableCourses.Count}): {theNames}.");
                //Main.Logger.Log($"[LINTHAR - PricesMenuStatsTracker] Course TERM names (Count: {_availableCourses.Count}): {theTermNames}.");

                _availableCourses.Sort(Sort);
                //Main.Logger.Log($"[LINTHAR - PricesMenuStatsTracker] Course names (Count: {_availableCourses.Count}): {theNames}.");
                //Main.Logger.Log($"[LINTHAR - PricesMenuStatsTracker] Course TERM names (Count: {_availableCourses.Count}): {theTermNames}.");
            }
            catch (Exception ex)
            {
                Main.Logger.Log("Exception occured: " + ex);
            }
        }
    }
}