using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TH20;

namespace LessFrustratingTPH
{
    [HarmonyPatch(typeof(WorldState), "GetItemsForRoom")]
    internal static class WorldState_GetItemsForRoom_Patch
    {
        public static RoomDefinition.Type _activeRoomType;

        private static void Postfix(RoomDefinition.Type roomType, bool decorationOnly, List<IRoomItemDefinition> outItems)
        {
            if (!Main.IsModEnabled || !Main.ModSettings.ReorderRoomItems)
                return;

            try
            {
                _activeRoomType = roomType;
            }
            catch (Exception ex)
            {
                Main.Logger.Log($"[WorldState] Exception occured: {ex}.");
            }
        }
    }

    [HarmonyPatch(typeof(RibbonMenuItemsState), "GetItemOrder")]
    internal static class RibbonMenuItemsState_GetItemOrder_Patch
    {
        private static RibbonMenuItemsState _instance;
        private static Level _level;
        private static bool _setupOnce = false;
        private static List<string> _userCustomItemsOrder = null;
        private static List<string> _modCustomItemsOrder = null;

        public static bool _bInputControlTextFilterActive;
        public static bool _bInputControlTextFilterCallbacksDisabled;
        public static bool _enabled;
        public static RibbonMenuItemsState.Settings _settings;
        public static string _textFilterText;
        public static string _textFilterTextLive;

        private static void Postfix(ref int __result, RibbonMenuItemsState __instance, Level level, IRoomItemDefinition definition, RequiredItem[] requiredItems, RoomDefinition.Type ____activeRoomType,
            bool ____bInputControlTextFilterActive, bool ____bInputControlTextFilterCallbacksDisabled, bool ____enabled, RibbonMenuItemsState.Settings ____settings, string ____textFilterText, string ____textFilterTextLive)
        {
            if (!Main.IsModEnabled || !Main.ModSettings.ReorderRoomItems)
                return;

            try
            {
                _instance = __instance;
                _level = level;

                _bInputControlTextFilterActive = ____bInputControlTextFilterActive;
                _bInputControlTextFilterCallbacksDisabled = ____bInputControlTextFilterCallbacksDisabled;
                _enabled = ____enabled;
                _settings = ____settings;
                _textFilterText = ____textFilterText;
                _textFilterTextLive = ____textFilterTextLive;
                //LogSettings();

                //Main.Logger.Log($"[RibbonMenuItemsState] _activeRoomType: {(int)WorldState_GetItemsForRoom_Patch._activeRoomType}, {_activeRoomType.ToString()}, {_activeRoomType}. BuildingLogicStateNull? {_level.BuildingLogic.CurrentState == BuildingLogic.State.Null}.");
                var customItemsOrderDict = TryGetCustomItemsOrderDict();
                __result = GetItemOrder(level, definition, requiredItems, customItemsOrderDict);
            }
            catch (Exception ex)
            {
                Main.Logger.Log($"[RibbonMenuItemsState] Exception occured: {ex}.");
            }
        }

        public static void LogSettings()
        {
            //if (_instance != null)
                Main.Logger.Log($"[RibbonMenuItemsState] _bInputControlTextFilterActive: {_bInputControlTextFilterActive}, _bInputControlTextFilterCallbacksDisabled: {_bInputControlTextFilterCallbacksDisabled}, _textFilterText: {_textFilterText}, _textFilterTextLive: {_textFilterTextLive}. [Settings]: " +
                                    $"_inputControlTextFilter.interactable: {_settings?._inputControlTextFilter?.interactable}, _inputControlTextFilter.isFocused: {_settings?._inputControlTextFilter?.isFocused}" +
                                    $", _inputControlTextFilter.IsInteractable(): {_settings?._inputControlTextFilter?.IsInteractable()}, _inputControlTextFilter.isRichTextEditingAllowed: {_settings?._inputControlTextFilter?.isRichTextEditingAllowed}, _inputControlTextFilter.onFocusSelectAll: {_settings?._inputControlTextFilter?.onFocusSelectAll}" +
                                    $", _inputControlTextFilter.wasCanceled: {_settings?._inputControlTextFilter?.wasCanceled}, _inputControlTextFilter.selectionAnchorPosition: {_settings?._inputControlTextFilter?.selectionAnchorPosition}, _inputControlTextFilter.selectionFocusPosition: {_settings?._inputControlTextFilter?.selectionFocusPosition}, _inputControlTextFilter.selectionStringAnchorPosition: {_settings?._inputControlTextFilter?.selectionStringAnchorPosition}, _inputControlTextFilter.selectionStringFocusPosition: {_settings?._inputControlTextFilter?.selectionStringFocusPosition}.");
            //else
            //    Main.Logger.Log("Still sleeping.");
        }

        private static Dictionary<string, int> TryGetCustomItemsOrderDict()
        {
            if (!_setupOnce)
            {

                _setupOnce = true;
            }

            if (_userCustomItemsOrder != null)
                return _userCustomItemsOrder.ToDictionary(x => x, x => _userCustomItemsOrder.IndexOf(x));

            //level.Metagame.HasUnlocked(definition);
            //TODO: radiators vs ice sculp? vending machines?

            if (WorldState_GetItemsForRoom_Patch._activeRoomType == RoomDefinition.Type.Hospital || _level.BuildingLogic.CurrentState == BuildingLogic.State.Null)
                _modCustomItemsOrder = new List<string>()
                {
                    "Reception",
                    "culture bin",
                    "DLC6_Wormhole_Bin",
                    "Air con unit",
                    "Radiator Small",
                    "Radiator",
                    "Vending machine Luxury drinks",
                    "Vending machine Luxury snacks",
                    //"Vending machine Drinks",
                    //"Vending machine Snacks",
                    "Leaflet stand",
                    "H d Fountain 1",
                    "Sonic Character Standee",
                    "Sonic Palm Tree Standee",
                    "Sonic Sunflower Standee",
                    "Fire extinguisher",
                    "Hand sanitiser",
                    "Sweet dispenser",
                    "Magazine rack",
                    "Bookcase office",
                };
            else
                _modCustomItemsOrder = new List<string>()
                {
                    "Picture Award Star Gold",
                    "culture bin",
                    "DLC6_Wormhole_Bin",
                    "Air con unit",
                    "Radiator Small",
                    "Radiator",
                    "Fire extinguisher",
                    "Coffee maker",
                    "Sweet dispenser",
                    "Hand sanitiser",
                    //"Picture Poster Anatomy",
                };

            if (_modCustomItemsOrder != null)
                return _modCustomItemsOrder.ToDictionary(x => x, x => _modCustomItemsOrder.IndexOf(x));

            return null;
        }

        private static int GetItemOrder(Level level, IRoomItemDefinition definition, RequiredItem[] requiredItems, Dictionary<string, int> customItemsOrderDict)
        {
            int itemIndex = level.WorldState.AvailableRoomItems.IndexOf(definition);
            //var ToLocalisedString = definition?.ToLocalisedString();
            //var RoomModifiers = definition?.RoomModifiers?.Select(x => $"{x} ({x.Description()})").ListThis("RoomModifiers", true, ";");
            //Main.Logger.Log($"ToLocalisedString: '{ToLocalisedString}', {RoomModifiers}, AnyTraining: {definition.RoomModifiers?.Any(x => x is RoomModifierTrainingRate)}, count: {definition.RoomModifiers?.Count(x => x is RoomModifierTrainingRate)}.");

            if (!level.Metagame.HasUnlocked(definition))
            //{
            //    Main.Logger.Log($"Made it to still locked item with: '{definition.ToLocalisedString()}'.");
                return 10000 + itemIndex;
            //}
            if (definition.ItemType == RoomItemDefinition.Type.Door)
            //{
            //    Main.Logger.Log($"Made it to mothafuckin doors with: '{definition.ToLocalisedString()}'.");
                return 0; //GetIndexingOrdinalNumber("door"); //0;
            //}

            if (requiredItems != null)
            {
                foreach (var requiredItem in requiredItems)
                {
                    if (requiredItem.Contains(definition))
                    //{
                    //    Main.Logger.Log($"Made it to requiredItems with: '{definition.ToLocalisedString()}'.");
                        return 1000; //GetIndexingOrdinalNumber("requiredItems"); //1;
                    //}
                }
            }

            if (WorldState_GetItemsForRoom_Patch._activeRoomType == RoomDefinition.Type.Toilets && definition.DebugTag.IsIn("Toilet cubicle gold", "Toilet sink unit", "Toilet hand dryer", "Hand sanitiser"))
                return 1400;

            if (WorldState_GetItemsForRoom_Patch._activeRoomType == RoomDefinition.Type.Training && definition.RoomModifiers?.Count(x => x is RoomModifierTrainingRate) > 0) //didn't seem to work?
            {
                if (definition.DebugTag == "Bookcase Training 2 ") //yes, they case a trailing space in their debug name
                    return 1490;
                else if (definition.DebugTag == "Bookcase Training 1")
                    return 1495;
                else
                    //Main.Logger.Log($"Made it to Type.Training with: '{definition.ToLocalisedString()}'.");
                    return 1500; //GetIndexingOrdinalNumber("training"); //2;
            }

            if (customItemsOrderDict != null && (_userCustomItemsOrder == null ? !string.IsNullOrWhiteSpace(definition?.DebugTag) : false) 
                && customItemsOrderDict.TryGetValueCatchException/*LogNotFound*/(_userCustomItemsOrder != null ? definition.ToLocalisedString() : definition.DebugTag, out int customIndex) 
                && definition.ToLocalisedString() != "Air Con Unit" && definition.ToLocalisedString() != "Jasmine Statue")
            //{
            //    Main.Logger.Log($"Made it here with: '{(_userCustomItemsOrder != null ? definition.ToLocalisedString() : definition.DebugTag)}'. _userCustomItemsOrder == null? {_userCustomItemsOrder == null}.");
                return 2000 + customIndex; //GetIndexingOrdinalNumber("customItems") + customIndex; //2 + customIndex;
            //}

            if (definition.RoomModifiers?.Count(x => x is RoomModifierDiagnosis || x is RoomModifierTreatment || x is RoomModifierResearchRate) > 1) //didn't seem to work?
            //{
            //    Main.Logger.Log($"Made it to Count>1 with: '{definition.ToLocalisedString()}'.");
                return 4000; //GetIndexingOrdinalNumber("roomModifiers1"); //4;
            //}

            if (definition.RoomModifiers?.Count(x => x is RoomModifierDiagnosis || x is RoomModifierTreatment || x is RoomModifierResearchRate) > 0) //didn't seem to work?
            //{
            //    Main.Logger.Log($"Made it to Count>0 with: '{definition.ToLocalisedString()}'. {definition?.RoomModifiers?.Select(x => x).ListThis("Modifiers", true)}");
                return 5000; //GetIndexingOrdinalNumber("roomModifiers0"); //5;
            //}

            if (definition.ItemType == RoomItemDefinition.Type.Window)
            //{
            //    Main.Logger.Log($"Made it to a mothafuckin window with: '{definition.ToLocalisedString()}'.");
                return 6000; //GetIndexingOrdinalNumber("window"); //(customItemsOrderDict != null ? 6 + customItemsOrderDict.Count : 6);
            //}

            if ((definition.DebugTag?.ToUpperInvariant().Contains("BENCH") ?? false) || (definition.Interactions?.Any(x => x.Name == "Sit") ?? false) || (definition.Interactions?.Any(x => x.Name == "WaterPlant") ?? false))
            //{
            //    Main.Logger.Log($"Made it to trash with: '{definition.ToLocalisedString()}'.");
                return 8000; //GetIndexingOrdinalNumber("trash"); //8;
            //}

            //Main.Logger.Log($"Exiting without hitting any IF with: '{definition.ToLocalisedString()}'.");
            return itemIndex + 7000; //GetIndexingOrdinalNumber("rest"); //(customItemsOrderDict != null ? 7 + customItemsOrderDict.Count : 7);
        }

        private static int _indexingOrdinalNumber = 0;
        private static string _lastReceivedCategory = "";
        private static int GetIndexingOrdinalNumber(string category)
        {
            if (category == "door")
            {
                _indexingOrdinalNumber = 0;
                _lastReceivedCategory = category;
                return _indexingOrdinalNumber;
            }
            else if (category != _lastReceivedCategory)
            {
                _indexingOrdinalNumber = (_indexingOrdinalNumber + 1) * 100;
            }

            //if (category == "customItems")
            //    return _indexingOrdinalNumber;

            _lastReceivedCategory = category;
            return _indexingOrdinalNumber /*(customItemsOrderDict != null ? _indexingOrdinalNumber + customItemsOrderDict.Count : _indexingOrdinalNumber)*/;
        }
    }

    [HarmonyPatch(typeof(HubMenuButtons), "ClickRoomsButton")]
    internal static class HubMenuButtons_ClickRoomsButton_Patch
    {
        private static HubMenuButtons _instance;
        private static Level _level;
        private static RibbonMenu _ribbonMenu;

        private static void Postfix(HubMenuButtons __instance, Level ____level)
        {
            if (!Main.IsModEnabled/* || !Main.ModSettings.ReorderRoomItems*/)
                return;

            try
            {
                _instance = __instance;
                _level = ____level;
                _ribbonMenu = _level.HUD.FindMenu<RibbonMenu>();
                //Main.Logger.Log("HubMenuButtons fired!");
                if (Main.ModSettings.RoomsMenuOpensToTemplatesByDefault)
                    _ribbonMenu.ToggleRoomTemplatesMenu();
                if (Main.ModSettings.SelectSearchAfterOpeningRoomsOrItems)
                    CtrlF();
            }
            catch (Exception ex)
            {
                Main.Logger.Log($"[HubMenuButtons] Exception occured: {ex}.");
            }
        }

        public static void CtrlF()
        {
            //Main.Logger.Log($"[HubMenuButtons] Firing! Mode: {_ribbonMenu.CurrentMode}.");
            switch (_ribbonMenu.CurrentMode)
            {
                case RibbonMenu.Mode.Items: _level.HUD.FindMenu<RibbonMenu>().ItemsStateSettings._inputControlTextFilter.Select(); break;
                case RibbonMenu.Mode.Rooms: _level.HUD.FindMenu<RibbonMenu>().RoomsStateSettings._inputControlTextFilter.Select(); break;
            }
        }
    }

    [HarmonyPatch(typeof(HubMenuButtons), "ClickItemsButton")]
    internal static class HubMenuButtons_ClickItemsButton_Patch
    {
        public static HubMenuButtons _instance;
        private static Level _level;
        private static RibbonMenu _ribbonMenu;

        private static void Postfix(HubMenuButtons __instance, Level ____level)
        {
            if (!Main.IsModEnabled/* || !Main.ModSettings.ReorderRoomItems*/)
                return;

            try
            {
                _instance = __instance;
                _level = ____level;
                //Main.Logger.Log("HubMenuButtons fired!");
                _ribbonMenu = _level.HUD.FindMenu<RibbonMenu>();
                CtrlF();
            }
            catch (Exception ex)
            {
                Main.Logger.Log($"[HubMenuButtons] Exception occured: {ex}.");
            }
        }

        public static void CtrlF()
        {
            //Main.Logger.Log($"[HubMenuButtons] Firing! Mode: {_ribbonMenu.CurrentMode}.");
            switch (_ribbonMenu.CurrentMode)
            {
                case RibbonMenu.Mode.Items: _level.HUD.FindMenu<RibbonMenu>().ItemsStateSettings._inputControlTextFilter.Select(); break;
                case RibbonMenu.Mode.Rooms: _level.HUD.FindMenu<RibbonMenu>().RoomsStateSettings._inputControlTextFilter.Select(); break;
            }
        }
    }


    //[HarmonyPatch(typeof(System.Collections.Generic.List<IRoomItemDefinition>), "Sort")]
    //internal static class Sort
    //{
    //    private static bool Prefix(System.Collections.Generic.List<IRoomItemDefinition> __instance, Comparison<IRoomItemDefinition> comparison)
    //    {
    //        if (!Main.IsModEnabled/* || !Main.ModSettings.ReorderRoomItems*/)
    //            return true;

    //        try
    //        {
    //            var genericTypeArgs = __instance.GetType().DeclaringType?.GenericTypeArguments;
    //            var compMatchTypeof = comparison.GetType() == typeof(Func<int, IRoomItemDefinition, IRoomItemDefinition>);
    //            var compMatchEquals = comparison.GetType().Equals(typeof(Func<int, IRoomItemDefinition, IRoomItemDefinition>));
    //            Main.Logger.Log($"[Sort] Sorting! {genericTypeArgs?.ListThis("genericTypeArgs", true)}, typeof: {compMatchTypeof}, equals: {compMatchEquals}.");
    //        }
    //        catch (Exception ex)
    //        {
    //            Main.Logger.Log($"[RibbonMenu] Exception occured: {ex}.");
    //        }

    //        return false;
    //    }

    //    public delegate int Comparison(IRoomItemDefinition x, IRoomItemDefinition y);
    //}

    public static class Injection
    {
        public static void Install(Type targetClassType, string targetMethodName, Type injectionClassType, string injectionMethodName)
        {
            MethodInfo methodToReplace = targetClassType.GetMethod(targetMethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo methodToInject = injectionClassType.GetMethod(injectionMethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            DoTheMagicWithMethods(methodToReplace, methodToInject);
        }

        public static void Install(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            DoTheMagicWithMethods(methodToReplace, methodToInject);
        }

        private static void DoTheMagicWithMethods(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);
            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    int* inj = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                    int* tar = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
#if DEBUG
                    Console.WriteLine("\nVersion x86 Debug\n");

                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;

                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x86 Release\n");
                    *tar = *inj;
#endif
                }
                else
                {

                    long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
#if DEBUG
                    Console.WriteLine("\nVersion x64 Debug\n");
                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;


                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x64 Release\n");
                    *tar = *inj;
#endif
                }
            }
        }
    }

    //[HarmonyPatch()] // patch Sort in RibbonMenuItemsState.SetItemList
    public class Patch_Class
    {
        private static MethodInfo TargetMethod()
        {
            return typeof(System.Collections.Generic.List<IRoomItemDefinition>)
                .GetMethod("Sort", BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(typeof(IRoomItemDefinition));
        }

        private static void Sort(Comparison<IRoomItemDefinition> comparison)
        {
            Main.Logger.Log("Injection worked!");
        }

        public static void Patch()
        {
            var myMethod = typeof(Patch_Class).GetMethod("Sort", BindingFlags.NonPublic | BindingFlags.Static);
            Injection.Install(TargetMethod(), myMethod);
        }

        //static void PostFix() { } //etc
    }

    //[HarmonyPatch(typeof(RibbonMenuItemsState), "SetItemList")]
    //internal static class RibbonMenuItemsState_SetItemList_Patch
    //{
    //    private static RibbonMenuItemsState _instance;
    //    private static Level _level;
    //    private static RibbonMenu _ribbonMenu;
    //    private static IRibbonMenuView _ribbonMenuView;

    //    private static void Postfix(RibbonMenuItemsState __instance, Level ____level, RoomDefinition.Type roomType, bool decorationOnly, IRibbonMenuView ____ribbonMenuView)
    //    {
    //        if (!Main.IsModEnabled/* || !Main.ModSettings.ReorderRoomItems*/)
    //            return;

    //        try
    //        {
    //            _instance = __instance;
    //            _level = ____level;
    //            _ribbonMenu = _level.HUD.FindMenu<RibbonMenu>();
    //            _ribbonMenuView = ____ribbonMenuView;

    //            if (_textFilterTextLive.IsNullOrEmpty())
    //            {
    //                if (_itemFilter == null && _bundleFilter == null)
    //                {
    //                    SetTemporaryInputControlTextFilterText(string.Empty);
    //                }
    //                else
    //                {
    //                    if (_itemFilter != null)
    //                        SetTemporaryInputControlTextFilterText(_itemFilter.LocalisedName.Translation);
    //                    if (_bundleFilter != null)
    //                        SetTemporaryInputControlTextFilterText(_bundleFilter._bundleName);
    //                }
    //            }
    //            _ribbonMenuView.SetTableHeadersActive(active: false);
    //            if (_showGridForItems)
    //            {
    //                _ribbonMenuView.TransitionBody(ref _settings.BodyGridAnimatorTarget, _settings.BodyGameObjects);
    //                _ribbonMenuView.SwapToggleToTableIcon();
    //                _ribbonMenuView.EnableGrid();
    //            }
    //            else
    //            {
    //                _ribbonMenuView.TransitionBody(ref _settings.BodyTableAnimatorTarget, _settings.BodyGameObjects);
    //                _ribbonMenuView.EnableTable();
    //                _ribbonMenuView.SetTableColumnHeaders(_settings.TableHeader);
    //                _ribbonMenuView.SetTableColumnDefinitions(_settings.ColumnDefinitions);
    //                _ribbonMenuView.SetTableRowFilter((RectTransform rect) => FilterRow(rect.GetComponent<RibbonItemRow>()));
    //                _ribbonMenuView.SetTableRowHeight(_settings.RowHeight);
    //                _ribbonMenuView.SetTableDirtyLayout();
    //            }
    //            _settings.FilterButtonAnimator.CurrentState = ButtonAnimator.State.Selectable;
    //            _showFilters = false;
    //            _ribbonMenuView.DestroyAllListItems();
    //            _activeRoomType = roomType;
    //            _decorationOnly = decorationOnly;
    //            _items.Clear();
    //            _level.WorldState.GetItemsForRoom(roomType, decorationOnly, _items);
    //            _items.Sort(delegate (IRoomItemDefinition definition1, IRoomItemDefinition definition2)
    //            {
    //                int itemOrder = GetItemOrder(_level, definition1, RequiredItems);
    //                int itemOrder2 = GetItemOrder(_level, definition2, RequiredItems);
    //                return itemOrder.CompareTo(itemOrder2);
    //            });
    //            _rows.Clear();
    //            _requiredRows.Clear();
    //            foreach (IRoomItemDefinition item in _items)
    //            {
    //                GameObject gameObject = (!_showGridForItems) ? _ribbonMenuView.InstantiateAsRowInTable(_settings.RibbonItemRowPrefab) : _ribbonMenuView.InstantiateAsCellInGrid(_settings.RibbonItemCellPrefab);
    //                RibbonItemRow ribbonItemRow = gameObject.GetComponent<RibbonItemRow>();
    //                ribbonItemRow.Setup(item, _level.Metagame, _level.GameplayStatsTracker);
    //                RefreshRequiredItemsState(ribbonItemRow);
    //                ribbonItemRow.Button.onPrimaryDown.AddListener(delegate
    //                {
    //                    SelectItem(ribbonItemRow);
    //                });
    //                ribbonItemRow.ButtonExtContent?.onPrimaryDown.AddListener(delegate
    //                {
    //                    OnRowUGCButton(ribbonItemRow);
    //                });
    //                RefreshRowMode(ribbonItemRow, placedThisFrame: false);
    //                if (ribbonItemRow.CurrentMode == RibbonItemRow.Mode.Locked)
    //                {
    //                    ribbonItemRow.Affordable = _level.Metagame.CanAffordSilver(ribbonItemRow.RoomItemDefinition);
    //                }
    //                else
    //                {
    //                    ribbonItemRow.Affordable = (_level.FinanceManager.Balance >= ribbonItemRow.RoomItemDefinition.GetCost());
    //                }
    //                _rows.Add(ribbonItemRow);
    //                if (_activeFloorPlan != null && _activeFloorPlan.Definition.IsRequiredItem(ribbonItemRow.RoomItemDefinition))
    //                {
    //                    _requiredRows.Add(ribbonItemRow);
    //                }
    //            }
    //            if (_showGridForItems)
    //            {
    //                _ribbonMenuView.FilterGridCells((UnityEngine.RectTransform rect) => FilterRow(rect.GetComponent<RibbonItemRow>()));
    //                int numOfGridColumns = _ribbonMenuView.GetNumOfGridColumns();
    //                int num = 0;
    //                float num2 = _ribbonMenuView.GetGridCellWidth() - 0.5f * _ribbonMenuView.GetGridCellSpacingHorizontal();
    //                foreach (RibbonItemRow row in _rows)
    //                {
    //                    if (row.isActiveAndEnabled)
    //                    {
    //                        int num3 = num % numOfGridColumns;
    //                        float x = (float)(numOfGridColumns - num3 - 1) * num2;
    //                        row.SetTooltipOffset(new UnityEngine.Vector3(x, 0f, 0f));
    //                        num++;
    //                    }
    //                }
    //                _ribbonMenuView.RecalulateGridHeight();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Main.Logger.Log($"[RibbonMenuItemsState] Exception occured: {ex}.");
    //        }
    //    }
    //}

}