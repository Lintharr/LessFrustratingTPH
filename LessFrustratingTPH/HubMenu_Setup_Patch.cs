using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TH20;

namespace LessFrustratingTPH
{
    [HarmonyPatch(typeof(HubMenu), "Setup")]
    internal static class HubMenu_Setup_Patch
    {
        private static HubMenu _instance;
        private static Level _level;

        private static void Postfix(HubMenu __instance, Level level)
        {
            try
            {
                if (!Main.IsModEnabled)
                    return;

                _instance = __instance;
                _level = level;
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerElapsedEvent);

                _level.HospitalPolicy.AutoSendForTreatment = true; //this works
                _level.HospitalPolicy.Config.AutoSendForTreatment = true; //this does nothing (looks like it is read for saving/loading, but then it doesn't anything further)

                if (Main.ModSettings.AutoPromoteStaff)
                {
                    _level.HospitalPolicy.StaffPromotion = true;
                    _level.HospitalPolicy.Config.StaffPromotion = true;
                }


                //List<IRoomItemDefinition> _items = level.WorldState.AvailableRoomItems; //new List<IRoomItemDefinition>();
                ////List<IRoomItemDefinition> _items = new List<IRoomItemDefinition>(); //level.WorldState.AvailableRoomItems;
                ////_level.WorldState.GetItemsForRoom(RoomDefinition.Type.Hospital, false, _items); //level.WorldState.AvailableRoomItems
                //foreach (var item in _items)
                //{
                //    var DebugTag = item?.DebugTag;
                //    var DataViewMode = item?.DataViewMode;
                //    //var Attributes = item?.Attributes?.Select(x => x?._type).ListThis("", true);
                //    var InteractionAttributeModifiers = item?.InteractionAttributeModifiers?.Select(x => $"{x._interactionName}").ListThis("InteractionAttributeModifiers", true, ";");
                //    var Interactions = item?.Interactions?.Select(x => $"{x?.Name}").ListThis("Interactions", true, ";");
                //    var RoomModifiers = item?.RoomModifiers?.Select(x => $"{x} ({x.Description()})").ListThis("RoomModifiers", true, ";");
                //    var ToString = item?.ToString();
                //    var ToLocalisedString = item?.ToLocalisedString();
                //    var GetSanitizedName = item?.GetSanitizedName();
                //    Main.Logger.Log($"{DebugTag},{DataViewMode},{ToString},{ToLocalisedString},{GetSanitizedName},{InteractionAttributeModifiers},{Interactions},{RoomModifiers}");
                //    //Main.Logger.Log($"DebugTag: '{DebugTag}', DataViewMode: '{DataViewMode}', ToString: '{ToString}', ToLocalisedString: '{ToLocalisedString}', GetSanitizedName: '{GetSanitizedName}', {InteractionAttributeModifiers}, {Interactions}, {RoomModifiers}.");
                //}

                //Main.Logger.Log("HubMenu fired!");
            }
            catch (Exception ex)
            {
                Main.Logger.Log("[HubMenu] Exception occured: " + ex);
            }
        }


        public static void CtrlF()
        {
            var ribbonMenu = _level.HUD.FindMenu<RibbonMenu>();
            //Main.Logger.Log($"[HubMenu] Firing! Mode: {ribbonMenu.CurrentMode}.");
            switch (ribbonMenu.CurrentMode)
            {
                case RibbonMenu.Mode.Items: ribbonMenu.ItemsStateSettings._inputControlTextFilter.Select(); break;
                case RibbonMenu.Mode.Rooms: ribbonMenu.RoomsStateSettings._inputControlTextFilter.Select(); break;
            }
        }

        public static void TryCloseMenu()
        {
            var ribbonMenu = _level.HUD.FindMenu<RibbonMenu>();
            if (ribbonMenu != null)
                ribbonMenu.TryCloseMenu();
        }

        public static void ToggleHireList()
        {
            try
            {
                if (_level != null)
                {
                    _level.CharacterEvents.OnStaffCancelPickup.InvokeSafe(param: false);
                    _level.HospitalHUDManager.ToggleHireList();
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Log("[HubMenu] Exception occured: " + ex);
            }
        }

        public static void ToggleItemsList()
        {
            try
            {
                if (_level != null)
                {

                    //MethodInfo method = HubMenuButtons_ClickItemsButton_Patch._instance?.GetType().GetMethod("ClickItemsButton", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    //if (method != null)
                    //    method.Invoke(HubMenuButtons_ClickItemsButton_Patch._instance, new object[] { /*StaffCharacter.Definition._type*/ });
                    _level.HospitalHUDManager.ToggleItemsList(RoomDefinition.Type.Hospital, null, true);
                    //RibbonMenuItemsState_GetItemOrder_Patch.LogSettings();
                    var roomsListRibbonMenu = _level.HUD.FindMenu<RibbonMenu>();
                    //_timer.Start();
                    roomsListRibbonMenu.RoomsStateSettings._inputControlTextFilter.Select();
                    //RibbonMenuItemsState_GetItemOrder_Patch.LogSettings();
                }

                //if (_level.BuildingLogic.CurrentState == BuildingLogic.State.Null)
                //    _level.HospitalHUDManager.ToggleItemsList(RoomDefinition.Type.Hospital, null, playSFX: true);
                //else
                //    _level.HospitalHUDManager.ToggleItemsList(_level.BuildingLogic.CurrentFloorPlan.Definition._type, _level.BuildingLogic.CurrentFloorPlan, playSFX: true);
            }
            catch (Exception ex)
            {
                Main.Logger.Log("[HubMenu] Exception occured: " + ex);
            }
        }

        private static System.Timers.Timer _timer = new System.Timers.Timer(2000);
        private static void OnTimerElapsedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            var roomsListRibbonMenu = _level.HUD.FindMenu<RibbonMenu>();
            roomsListRibbonMenu.RoomsStateSettings._inputControlTextFilter.Select();
        }
        public static void ToggleRoomsList()
        {
            try
            {
                if (_level != null)
                {
                    //MethodInfo method = HubMenuButtons_ClickItemsButton_Patch._instance?.GetType().GetMethod("ClickRoomsButton", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    //if (method != null)
                    //    method.Invoke(HubMenuButtons_ClickItemsButton_Patch._instance, new object[] { /*StaffCharacter.Definition._type*/ });
                    //RibbonMenuItemsState_GetItemOrder_Patch.LogSettings();
                    _level.HospitalHUDManager.ToggleRoomsList();
                    //RibbonMenuItemsState_GetItemOrder_Patch.LogSettings();
                    if (Main.ModSettings.RoomsMenuOpensToTemplatesByDefault)
                    {
                        var roomsListRibbonMenu = _level.HUD.FindMenu<RibbonMenu>();
                        roomsListRibbonMenu.ToggleRoomTemplatesMenu();
                        //RibbonMenuItemsState_GetItemOrder_Patch.LogSettings();

                        //_timer.Start();
                        //roomsListRibbonMenu.RoomsStateSettings._inputControlTextFilter.Select();
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Log("[HubMenu] Exception occured: " + ex);
            }
        }
    }
}