using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;

namespace LessFrustratingTPH
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw(" <b>Applicants' Traits</b>", Collapsible = true), Space(5)] public ApplicantsTraitsHolder ApplicantsTraits = new ApplicantsTraitsHolder();

        [Draw(" <b>Applicants' Qualifications</b>", Collapsible = true), Space(5)] public ApplicantsQualificationsHolder ApplicantsQualifications = new ApplicantsQualificationsHolder();

        [DrawFields(DrawFieldMask.OnlyDrawAttr)]
        public class ApplicantsTraitsHolder
        {
            [Draw("Applicants' traits special mode", Vertical = true, Tooltip = "<color=#ffa500ff><size=\"16\"><b>(off / only random positive traits / all positive traits \n/ traits quality based on reputation)</b></size></color>")]
            public TraitsMode PositiveTraitsMode = TraitsMode.Off;

            [Draw("Applicants have 1 more trait than usually", Vertical = true), Space(5)]
            public bool AdditionalTrait = false;
        }

        [DrawFields(DrawFieldMask.Public)]
        public class ApplicantsQualificationsHolder
        {
            [Draw("Applicants' qualifications special mode", Vertical = true, Height = 20, Tooltip = "<color=#ffa500ff><size=\"16\"><b>(off / new hires will have only zero or one qualification \n/ new hires will have qualifications based on reputation)</b></size></color>")]
            public ApplicantQualificationsMode ApplicantsQualificationsMode = ApplicantQualificationsMode.Off;

            [Draw("Applicants are more likely to be specialized in one field rather than all over the place", Vertical = true), Space(5)]
            public bool SpecializedQualifications = true;

            [Draw("Do not roll useless qualifications (also affects training)", Vertical = true, Tooltip = "<color=#ffa500ff><size=\"16\"><b>(New hires won't roll Bedside Manners, Happiness, \nTraining Masterclass, Pharmacy and Injection Administration)</b></size></color>")]
            public bool DontRollUselessQualifications = false;
        }

        public int MaxPatients = 150;

        public float ItemMaintenanceThreshold = 60f;

        public bool PatientsQueueEmptyRooms = true;

        public bool QueueWarningLengthMeansSomething = true;

        public KeyCode ToggleHireMenuKeybind = KeyCode.F2;
        public KeyCode ToggleItemsMenuKeybind = KeyCode.F3;
        public KeyCode ToggleRoomsMenuKeybind = KeyCode.F4;
        public bool RoomsMenuOpensToTemplatesByDefault = false;
        public bool SelectSearchAfterOpeningRoomsOrItems = true;

        public bool EnableDefamationMarketingCampaign = false;

        public bool ReorderTrainingCourses = true;
        public bool ReorderRoomItems = true;

        public bool SetPriceOnEveryNewIllness = true;
        public int PriceOnEveryNewIllness = 100;

        public bool AutoPromoteStaff = false;

        public string TrainingCoursesOrder; //TODO
        public string RoomItemsOrder; //TODO

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }

        public void OnChange()
        {
        }

        public enum TraitsMode
        {
            Off,
            RandomPositiveTraits,
            MaxPositiveTraits,
            ReputationBasedTraits,
        }

        public enum ApplicantQualificationsMode
        {
            Off,
            NewHiresWithOnlyNoneOrOneQual,
            HospitalLevelBasedQualifications,
        }

        //public static readonly Dictionary<string, string> AllowedKeysToDisplayValueDict = new Dictionary<string, string>
        //{
        //    { "None", "None" },
        //    { "BackQuote", "~" },
        //    { "Tab", "Tab" },
        //    { "Space", "Space" },
        //    { "Return", "Enter" },

        //    { "Alpha0", "0" },
        //    { "Alpha1", "1" },
        //    { "Alpha2", "2" },
        //    { "Alpha3", "3" },
        //    { "Alpha4", "4" },
        //    { "Alpha5", "5" },
        //    { "Alpha6", "6" },
        //    { "Alpha7", "7" },
        //    { "Alpha8", "8" },
        //    { "Alpha9", "9" },
        //    { "Minus", "-" },
        //    { "Equals", "=" },
        //    { "Backspace", "Backspace" },

        //    { "F1", "F1" },
        //    { "F2", "F2" },
        //    { "F3", "F3" },
        //    { "F4", "F4" },
        //    { "F5", "F5" },
        //    { "F6", "F6" },
        //    { "F7", "F7" },
        //    { "F8", "F8" },
        //    { "F9", "F9" },
        //    { "F10", "F10" },
        //    { "F11", "F11" },
        //    { "F12", "F12" },

        //    { "A", "A" },
        //    { "B", "B" },
        //    { "C", "C" },
        //    { "D", "D" },
        //    { "E", "E" },
        //    { "F", "F" },
        //    { "G", "G" },
        //    { "H", "H" },
        //    { "I", "I" },
        //    { "J", "J" },
        //    { "K", "K" },
        //    { "L", "L" },
        //    { "M", "M" },
        //    { "N", "N" },
        //    { "O", "O" },
        //    { "P", "P" },
        //    { "Q", "Q" },
        //    { "R", "R" },
        //    { "S", "S" },
        //    { "T", "T" },
        //    { "U", "U" },
        //    { "V", "V" },
        //    { "W", "W" },
        //    { "X", "X" },
        //    { "Y", "Y" },
        //    { "Z", "Z" },

        //    { "LeftBracket", "[" },
        //    { "RightBracket", "]" },
        //    { "Semicolon", ";" },
        //    { "Quote", "'" },
        //    { "Backslash", "\\" },
        //    { "Comma", "," },
        //    { "Period", "." },
        //    { "Slash", "/" },

        //    { "Insert", "Insert" },
        //    { "Home", "Home" },
        //    { "Delete", "Delete" },
        //    { "End", "End" },
        //    { "PageUp", "Page Up" },
        //    { "PageDown", "Page Down" },
        //    { "UpArrow", "Up Arrow" },
        //    { "DownArrow", "Down Arrow" },
        //    { "RightArrow", "Right Arrow" },
        //    { "LeftArrow", "Left Arrow" },

        //    { "KeypadDivide", "Numpad /" },
        //    { "KeypadMultiply", "Numpad *" },
        //    { "KeypadMinus", "Numpad -" },
        //    { "KeypadPlus", "Numpad +" },
        //    { "KeypadEnter", "Numpad Enter" },
        //    { "KeypadPeriod", "Numpad Del" },
        //    { "Keypad0", "Numpad 0" },
        //    { "Keypad1", "Numpad 1" },
        //    { "Keypad2", "Numpad 2" },
        //    { "Keypad3", "Numpad 3" },
        //    { "Keypad4", "Numpad 4" },
        //    { "Keypad5", "Numpad 5" },
        //    { "Keypad6", "Numpad 6" },
        //    { "Keypad7", "Numpad 7" },
        //    { "Keypad8", "Numpad 8" },
        //    { "Keypad9", "Numpad 9" },

        //    { "RightShift", "Right Shift" },
        //    { "LeftShift", "Left Shift" },
        //    { "RightControl", "Right Ctrl" },
        //    { "LeftControl", "Left Ctrl" },
        //    { "RightAlt", "Right Alt" },
        //    { "LeftAlt", "Left Alt" },

        //    { "Pause", "Pause" },
        //    { "Escape", "Escape" },
        //    { "Numlock", "Num Lock" },
        //    { "CapsLock", "Caps Lock" },
        //    { "ScrollLock", "Scroll Lock" },
        //    { "Print", "Print Screen" },
        //};

        public static readonly Dictionary<string, string> AllowedKeysToKeyCodeDict = new Dictionary<string, string>
        {
            { "None", "None" },
            { "~", "BackQuote" },
            { "Tab", "Tab" },
            { "Space", "Space" },
            { "Enter", "Return" },

            { "0", "Alpha0" },
            { "1", "Alpha1" },
            { "2", "Alpha2" },
            { "3", "Alpha3" },
            { "4", "Alpha4" },
            { "5", "Alpha5" },
            { "6", "Alpha6" },
            { "7", "Alpha7" },
            { "8", "Alpha8" },
            { "9", "Alpha9" },
            { "-", "Minus" },
            { "=", "Equals" },
            { "Backspace", "Backspace" },

            { "F1", "F1" },
            { "F2", "F2" },
            { "F3", "F3" },
            { "F4", "F4" },
            { "F5", "F5" },
            { "F6", "F6" },
            { "F7", "F7" },
            { "F8", "F8" },
            { "F9", "F9" },
            { "F10", "F10" },
            { "F11", "F11" },
            { "F12", "F12" },

            { "A", "A" },
            { "B", "B" },
            { "C", "C" },
            { "D", "D" },
            { "E", "E" },
            { "F", "F" },
            { "G", "G" },
            { "H", "H" },
            { "I", "I" },
            { "J", "J" },
            { "K", "K" },
            { "L", "L" },
            { "M", "M" },
            { "N", "N" },
            { "O", "O" },
            { "P", "P" },
            { "Q", "Q" },
            { "R", "R" },
            { "S", "S" },
            { "T", "T" },
            { "U", "U" },
            { "V", "V" },
            { "W", "W" },
            { "X", "X" },
            { "Y", "Y" },
            { "Z", "Z" },

            { "[", "LeftBracket" },
            { "]", "RightBracket" },
            { ";", "Semicolon" },
            { "'", "Quote" },
            { "\\", "Backslash" },
            { ",", "Comma" },
            { ".", "Period" },
            { "/", "Slash" },

            { "Insert", "Insert" },
            { "Home", "Home" },
            { "Delete", "Delete" },
            { "End", "End" },
            { "Page Up", "PageUp" },
            { "Page Down", "PageDown" },
            { "Up Arrow", "UpArrow" },
            { "Down Arrow", "DownArrow" },
            { "Right Arrow", "RightArrow" },
            { "Left Arrow", "LeftArrow" },

            { "Numpad /", "KeypadDivide" },
            { "Numpad *", "KeypadMultiply" },
            { "Numpad -", "KeypadMinus" },
            { "Numpad +", "KeypadPlus" },
            { "Numpad Enter", "KeypadEnter" },
            { "Numpad Del", "KeypadPeriod" },
            { "Numpad 0", "Keypad0" },
            { "Numpad 1", "Keypad1" },
            { "Numpad 2", "Keypad2" },
            { "Numpad 3", "Keypad3" },
            { "Numpad 4", "Keypad4" },
            { "Numpad 5", "Keypad5" },
            { "Numpad 6", "Keypad6" },
            { "Numpad 7", "Keypad7" },
            { "Numpad 8", "Keypad8" },
            { "Numpad 9", "Keypad9" },

            { "Right Shift", "RightShift" },
            { "Left Shift", "LeftShift" },
            { "Right Ctrl", "RightControl" },
            { "Left Ctrl", "LeftControl" },
            { "Right Alt", "RightAlt" },
            { "Left Alt", "LeftAlt" },

            { "Pause", "Pause" },
            { "Escape", "Escape" },
            { "Num Lock", "Numlock" },
            { "Caps Lock", "CapsLock" },
            { "Scroll Lock", "ScrollLock" },
            { "Print Screen", "Print" },
        };
    }
}