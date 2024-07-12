using FullInspector;
using Harmony12;
using System;
using System.Linq;
using System.Reflection;
using TH20;
using UnityEngine;
using UnityModManagerNet;

namespace LessFrustratingTPH
{
    internal static class Main
    {
        public static bool IsModEnabled;
        public static Settings ModSettings;
        public static UnityModManager.ModEntry.ModLogger Logger;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            ModSettings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            Logger = modEntry.Logger;

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnUpdate = OnUpdate;

            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            try
            {
                IsModEnabled = value;
                UpdateGameAlgorithmsConfig();
            }
            catch (Exception ex)
            {
                IsModEnabled = !IsModEnabled;
                Logger.Log("Updating GameAlgorithmsConfig failed");
                Logger.LogException(ex);
                return false;
            }
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            string text = "", text2 = "";
            int result = 0;

            GUILayout.Space(5);
            DrawMaxPatients();
            DrawItemMaintenanceThreshold();
            DrawPatientsQueueEmptyRooms();
            DrawQueueWarningLengthMeansSomething();

            ModSettings.Draw(modEntry);
            GUILayout.Space(5);
            GUILayout.BeginVertical(new GUIStyle("box"));
            GUILayout.Space(2);
            DrawOpenHireMenuKeybinding();
            DrawOpenItemsMenuKeybinding();
            DrawOpenRoomsMenuKeybinding();
            DrawSelectSearchToggle();
            GUILayout.Label(" <b>Esc</b> will now allow to close more menus. You can also now press <b>Ctrl</b>+<b>F</b> to activate search in Rooms/Items menu.", GUILayout.ExpandWidth(false));
            GUILayout.Space(4);
            GUILayout.EndVertical();

            GUILayout.Space(5);
            DrawDefamationMarketingCampaign();
            DrawReorderTrainingCourses();
            DrawReorderRoomItems();
            DrawPricesOnEveryNewIllness();
            DrawAutoPromoteStaff();
            GUILayout.Space(10);
            GUILayout.Label(" <b>Fast-Track Treatment Decision</b> in Overview Policy tab will be set to active every time a level is loaded. Sorry, not sorry, this is <i>non-negotiable</i>.", GUILayout.ExpandWidth(false));
            GUILayout.Space(5);


            void DrawMaxPatients()
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(" Max number of patients allowed at the hospital:", GUILayout.ExpandWidth(false));
                text = ModSettings.MaxPatients.ToString();
                text2 = GUILayout.TextField(text, 3, GUILayout.Width(30f), GUILayout.Height(20f));
                if (text2 != text && int.TryParse(text2, out result))
                    ModSettings.MaxPatients = Mathf.Clamp(result, 1, 999);
                GUILayout.EndHorizontal();
            }
            void DrawItemMaintenanceThreshold()
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(" Janitors will start repairing machines and unclog toilets when maintenance level <=", GUILayout.ExpandWidth(false));
                text = ModSettings.ItemMaintenanceThreshold.ToString();
                text2 = GUILayout.TextField(text, 2, GUILayout.Width(30f), GUILayout.Height(20f));
                if (text2 != text && int.TryParse(text2, out result))
                    ModSettings.ItemMaintenanceThreshold = Mathf.Clamp(result, 20, 80);
                GUILayout.Label("% (min:20, max:80)");
                GUILayout.EndHorizontal();
            }
            void DrawPatientsQueueEmptyRooms()
            {
                GUILayout.BeginHorizontal();
                ModSettings.PatientsQueueEmptyRooms = GUILayout.Toggle(ModSettings.PatientsQueueEmptyRooms, " Patients will queue empty rooms even if unstaffed", GUILayout.ExpandWidth(false), GUILayout.Height(20f));
                GUILayout.EndHorizontal();
            }
            void DrawQueueWarningLengthMeansSomething()
            {
                GUILayout.BeginHorizontal();
                ModSettings.QueueWarningLengthMeansSomething = GUILayout.Toggle(ModSettings.QueueWarningLengthMeansSomething, " Patients de-prioritize queues that go above the queue warning length", GUILayout.ExpandWidth(false), GUILayout.Height(20f));
                GUILayout.EndHorizontal();
            }
            void DrawPricesOnEveryNewIllness()
            {
                GUILayout.BeginHorizontal();
                ModSettings.SetPriceOnEveryNewIllness = GUILayout.Toggle(ModSettings.SetPriceOnEveryNewIllness, " Whenever a new illness is discovered, set prices of all illnesses to", GUILayout.ExpandWidth(false), GUILayout.Height(20f));
                GUI.enabled = ModSettings.SetPriceOnEveryNewIllness;
                text = ModSettings.PriceOnEveryNewIllness.ToString();
                text2 = GUILayout.TextField(text, 3, GUILayout.Width(30f), GUILayout.Height(20f));
                if (text2 != text && int.TryParse(text2, out result))
                    ModSettings.PriceOnEveryNewIllness = Mathf.Clamp(result, -100, 100);
                GUI.enabled = true;
                GUILayout.Label("%");
                GUILayout.EndHorizontal();
            }
            void DrawDefamationMarketingCampaign()
            {
                GUILayout.BeginHorizontal();
                ModSettings.EnableDefamationMarketingCampaign = GUILayout.Toggle(ModSettings.EnableDefamationMarketingCampaign, " Replace Small Reputation Marketing Campaign with Defamatory Marketing Campaign", GUILayout.ExpandWidth(false), GUILayout.Height(20f));
                GUILayout.EndHorizontal();
            }
            void DrawReorderTrainingCourses()
            {
                GUILayout.BeginHorizontal();
                ModSettings.ReorderTrainingCourses = GUILayout.Toggle(ModSettings.ReorderTrainingCourses, " Reorder training courses", GUILayout.ExpandWidth(false), GUILayout.Height(20f));
                GUILayout.EndHorizontal();
            }
            void DrawReorderRoomItems()
            {
                GUILayout.BeginHorizontal();
                ModSettings.ReorderRoomItems = GUILayout.Toggle(ModSettings.ReorderRoomItems, " Reorder room items", GUILayout.ExpandWidth(false), GUILayout.Height(20f));
                GUILayout.EndHorizontal();
            }
            void DrawAutoPromoteStaff()
            {
                GUILayout.BeginHorizontal();
                ModSettings.AutoPromoteStaff = GUILayout.Toggle(ModSettings.AutoPromoteStaff, " Set Auto Promote Staff in Overview Policy tab to active (happens at the start of every level)", GUILayout.ExpandWidth(false), GUILayout.Height(20f));
                GUILayout.EndHorizontal();
            }

            void DrawOpenHireMenuKeybinding()
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(" Toggle Hire Menu Keybind:", GUILayout.Width(200));
                DrawAndHandleKeybindValue(ref ModSettings.ToggleHireMenuKeybind, "Hire Menu");
                GUILayout.EndHorizontal();
            }
            void DrawOpenItemsMenuKeybinding()
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(" Toggle Items Menu Keybind:", GUILayout.Width(200));
                DrawAndHandleKeybindValue(ref ModSettings.ToggleItemsMenuKeybind, "Items Menu");
                GUILayout.EndHorizontal();
            }
            void DrawOpenRoomsMenuKeybinding()
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(" Toggle Rooms Menu Keybind:", GUILayout.Width(200));
                DrawAndHandleKeybindValue(ref ModSettings.ToggleRoomsMenuKeybind, "Rooms Menu");
                ModSettings.RoomsMenuOpensToTemplatesByDefault = GUILayout.Toggle(ModSettings.RoomsMenuOpensToTemplatesByDefault, " Rooms menu opens to templates by default", GUILayout.ExpandWidth(false), GUILayout.Height(20f));
                GUILayout.EndHorizontal();
            }
            void DrawSelectSearchToggle()
            {
                GUILayout.BeginHorizontal();
                ModSettings.SelectSearchAfterOpeningRoomsOrItems = GUILayout.Toggle(ModSettings.SelectSearchAfterOpeningRoomsOrItems, " After opening Rooms or Items menu, select search textbox to allow immediate typing and searching", GUILayout.ExpandWidth(false), GUILayout.Height(20f));
                GUILayout.EndHorizontal();
            }
        }

        private static void DrawAndHandleKeybindValue(ref KeyCode keybindSetting, string keybindName)
        {
            var chosenKeybindIndex = Settings.AllowedKeysToKeyCodeDict.Values.ToList().IndexOf(keybindSetting.ToString());
            if (UnityModManager.UI.PopupToggleGroup(ref chosenKeybindIndex, Settings.AllowedKeysToKeyCodeDict.Keys.ToArray(), $"Choose the keybind\nfor {keybindName}", null, GUILayout.ExpandWidth(false)))
            {
                var chosenKeybindString = Settings.AllowedKeysToKeyCodeDict.Keys.ElementAt(chosenKeybindIndex);
                var chosenKeybindStringTranslated = Settings.AllowedKeysToKeyCodeDict[chosenKeybindString];
                keybindSetting = Enum.Parse<KeyCode>(chosenKeybindStringTranslated);
            }
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            ModSettings.Save(modEntry);
            UpdateGameAlgorithmsConfig();
        }

        public static void UpdateGameAlgorithmsConfig()
        {
            if (!IsModEnabled)
                return;

            GameAlgorithmsConfig gameAlgorithmsConfig = null;
            MainScript mainScript = UnityEngine.Object.FindObjectOfType<MainScript>();
            if (mainScript)
                gameAlgorithmsConfig = ((Traverse.Create(mainScript).Field("_appConfig").GetValue<SharedInstance<AppConfig>>()?.Instance)?.GameAlgorithmsConfig?.Instance);

            if (gameAlgorithmsConfig != null)
            {
                Logger.Log("Updating GameAlgorithmsConfig");
                Traverse traverse = Traverse.Create(gameAlgorithmsConfig);
                traverse.Field("ItemMaintenanceThreshold").SetValue(100f - ModSettings.ItemMaintenanceThreshold);
                if (ModSettings.PatientsQueueEmptyRooms)
                    traverse.Field("RoomScoreNotFullyStaffed").SetValue(5f);
            }
        }

        private static bool _fuckYouUnity = false;
        private static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if (Input.GetKeyDown(ModSettings.ToggleHireMenuKeybind))
                HubMenu_Setup_Patch.ToggleHireList();
            if (Input.GetKeyDown(ModSettings.ToggleItemsMenuKeybind))
                HubMenu_Setup_Patch.ToggleItemsList();
            if (Input.GetKeyDown(ModSettings.ToggleRoomsMenuKeybind))
                HubMenu_Setup_Patch.ToggleRoomsList();

            //if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
            //    RibbonMenu_RefreshHeaderText_Patch.CtrlF();
            if (Input.GetKeyDown(KeyCode.LeftControl) || _fuckYouUnity)
            {
                _fuckYouUnity = true;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    HubMenu_Setup_Patch.CtrlF();
                    _fuckYouUnity = false;
                }
            }


            if (Input.GetKeyDown(KeyCode.Escape))
                HubMenu_Setup_Patch.TryCloseMenu();
            //if (Input.GetKeyDown(KeyCode.F4))
            //    PricesMenu2_OnNewDiscoveredIllnessesStat_Patch.Execute();
            //if (Input.GetKeyDown(KeyCode.F3))
            //    TrainingMenu_CalculateAvailableCourses_Patch.Execute();
        }
    }
}