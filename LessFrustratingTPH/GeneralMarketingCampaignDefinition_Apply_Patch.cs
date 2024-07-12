using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using TH20;
using UnityEngine;
using UnityEngine.UI;

namespace LessFrustratingTPH
{
    [HarmonyPatch(typeof(GeneralMarketingCampaignDefinition), "Apply")]
    internal static class GeneralMarketingCampaignDefinition_Apply_Patch
    {
        //public static GeneralMarketingCampaignDefinition SmallReputationCampaign = null;

        private static bool Prefix(GeneralMarketingCampaignDefinition __instance, ref float multiplier, MarketingManager marketingManager)
        {
            if (!Main.IsModEnabled || !Main.ModSettings.EnableDefamationMarketingCampaign)
                return true;

            //Main.Logger.Log($"[Marketing] {__instance.ExtractCampaignInfo()} multiplier: '{multiplier}', '{marketingManager.GetCampaigns(MarketingCampaignType.General).Select(x => x.ExtractCampaignInfo()).ListThis("General campaigns", true)}'.");

            //if (multiplier != 0) Main.Logger.Log($"[Marketing] {__instance.NameLocalised.ToAnalyticsTermString()} multiplier: '{multiplier}'");

            //the lazy way:
            if (__instance.NameLocalised.ToAnalyticsTermString() == "General_S_Name")
            {
                multiplier = multiplier * -3;
                //__instance.NameLocalised.Term = "Defamation_Campaign"; //TODO: TH20.MarketingCampaignListItem public TMP_Text Name; or MarketingCampaignMenu private TMP_Text _campaignNameText; private TMP_Text _campaignDescriptionText; private void OnSelectCampaignCategory(MarketingCampaignType type, Button button
                //__instance.NameLocalised.Term = "Defamatory Campaign"; //TODO: TH20.MarketingCampaignListItem public TMP_Text Name; or MarketingCampaignMenu private TMP_Text _campaignNameText; private TMP_Text _campaignDescriptionText; private void OnSelectCampaignCategory(MarketingCampaignType type, Button button
                //__instance.LaunchCost = 50000; // TH20.MarketingManager public List<MarketingCampaignDefinition> GetCampaigns(MarketingCampaignType type)
                //__instance.MonthlySpend = 10000;
            }

            //if (multiplier != 0) Main.Logger.Log($"[Marketing] {__instance.NameLocalised.ToAnalyticsTermString()} multiplier: '{multiplier}'");

            return true;
        }

        private static string ExtractCampaignInfo(this MarketingCampaignDefinition campaign)
            => $"Name: '{campaign.NameLocalised}', AnalyticsTerm: '{campaign.NameLocalised.ToAnalyticsTermString()}', Term: '{campaign.NameLocalised.Term}', LaunchCost: '{campaign.LaunchCost}'.";
    }

    [HarmonyPatch(typeof(MarketingCampaignMenu), "OnSelectCampaignCategory")]
    internal static class MarketingCampaignMenu_OnSelectCampaignCategory_Patch
    {
        private static bool IsDefamationCampaignActive = false;
        private static List<GameObject> _campaignItems;
        private static Level _level;

        private static int _originalLaunchCost = 0;
        private static int _originalMonthlySpend = 0;

        private static bool Prefix(MarketingCampaignMenu __instance, MarketingCampaignType type, Button button, List<GameObject> ____campaignItems, Level ____level)
        {
            if (!Main.IsModEnabled/* || !Main.ModSettings.EnableDefamationMarketingCampaign*/)
                return true;

            try
            {
                _campaignItems = ____campaignItems;
                _level = ____level;
                if (/*!SetupOnce || */Main.ModSettings.EnableDefamationMarketingCampaign != IsDefamationCampaignActive)
                {
                    //I2.Loc.LanguageSourceData lsdSmallCampaign = I2.Loc.LocalizationManager.Sources.FirstOrDefault(x => x.mTerms.Any(y => y.Term.Contains("General_S_Name"))); //Marketing/General_S_Desc
                    //I2.Loc.LanguageSourceData languageSourceData = I2.Loc.LocalizationManager.Sources.FirstOrDefault(x => x.mTerms.Any(y => y.Term.Contains("Doctor_GeneralPractice_1_Name")));
                    //I2.Loc.TermData termData = new I2.Loc.TermData()
                    //{
                    //    Term = "Defamation_Campaign",
                    //    Description = "Allows you to decrease your reputation at a bit faster rate than Large Marketing Campaign increases the reputation",
                    //    Languages = new string[] { I2.Loc.LocalizationManager.CurrentLanguage },
                    //    TermType = I2.Loc.eTermType.Text,
                    //};
                    //I2.Loc.LanguageSourceData newSource = new I2.Loc.LanguageSourceData()
                    //{
                    //    mTerms = new System.Collections.Generic.List<I2.Loc.TermData>()
                    //    {
                    //        termData,
                    //    },
                    //    mLanguages = languageSourceData.mLanguages/*new System.Collections.Generic.List<I2.Loc.LanguageData>() { new I2.Loc.LanguageData() { } }*/,
                    //    OnMissingTranslation = I2.Loc.LanguageSourceData.MissingTranslationAction.ShowTerm,
                    //    mDictionary = new Dictionary<string, I2.Loc.TermData>() { { termData.Term, termData } },
                    //};
                    //I2.Loc.LocalizationManager.Sources.Add(newSource);
                    //for (int i = 0; i < newSource.mLanguages.Count(); i++)
                    //{
                    //    newSource.mLanguages[i].SetLoaded(loaded: true);
                    //}
                    //newSource.UpdateDictionary(force: true);

                    List<MarketingCampaignDefinition> campaigns = ____level.MarketingManager.GetCampaigns(MarketingCampaignType.General);
                    MarketingCampaignDefinition campaign = campaigns.FirstOrDefault(x => x.NameLocalised.ToAnalyticsTermString() == "General_S_Name");
                    if (_originalLaunchCost == 0)
                    {
                        _originalLaunchCost = campaign.LaunchCost;
                        _originalMonthlySpend = campaign.MonthlySpend;
                    }

                    campaign.LaunchCost = Main.ModSettings.EnableDefamationMarketingCampaign ? 50000 : _originalLaunchCost;
                    campaign.MonthlySpend = Main.ModSettings.EnableDefamationMarketingCampaign ? 10000 : _originalMonthlySpend;

                    //I2.Loc.LocalizationManager.UpdateSources();

                    ////var cat = lsdSmallCampaign.GetCategories();
                    //var exportCsv = lsdSmallCampaign.Export_CSV("Marketing/General_S_Name");
                    //var exportCsvI2 = lsdSmallCampaign.Export_I2CSV("Marketing/General_S_Name");
                    ////var  = lsdSmallCampaign.ClearAllData();
                    //var langData = lsdSmallCampaign.GetLanguageData("Marketing/General_S_Name");
                    ////var langs = lsdSmallCampaign.GetLanguages();
                    ////var langsC = lsdSmallCampaign.GetLanguagesCode();
                    ////var terms = lsdSmallCampaign.GetTermsList();
                    //var termDataLSD = lsdSmallCampaign.GetTermData("Marketing/General_S_Name");
                    ////var = lsdSmallCampaign.Import_CSV();
                    ////var = lsdSmallCampaign.Import_I2CSV();
                    //var mDict = lsdSmallCampaign.mDictionary;
                    //var mLangs = lsdSmallCampaign.mLanguages;
                    //var mTerms = lsdSmallCampaign.mTerms;
                    //var mAppName = lsdSmallCampaign.mTerm_AppName;
                    //Main.Logger.Log($"[LSD] {termDataLSD?.Languages?.ListThis("termDataLSD.Languages", true)}, langData.Name: '{langData?.Name}', termDataLSD.Description: '{termDataLSD?.Description}', termDataLSD.Term: '{termDataLSD?.Term}', termDataLSD.TermType: '{termDataLSD?.TermType}', exportCsv: '{exportCsv}', exportCsvI2: '{exportCsvI2}'.");
                    //Main.Logger.Log($"[LSD] Key: {mDict?.FirstOrDefault().Key}, Value: {mDict?.FirstOrDefault().Value.Description}, Name: {mLangs?.FirstOrDefault().Name}, Code: {mLangs?.FirstOrDefault().Code}, Term: {mTerms?.FirstOrDefault().Term}, Description: {mTerms?.FirstOrDefault().Description}.");

                    //Main.Logger.Log("Setup complete!");
                    IsDefamationCampaignActive = Main.ModSettings.EnableDefamationMarketingCampaign;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Log($"[MarketingCampaignMenu - Prefix] Exception occured: {ex}.");
            }

            return true;
        }

        //private static void Postfix(MarketingCampaignMenu __instance, MarketingCampaignType type, Button button, List<GameObject> ____campaignItems, Level ____level)
        //{
        //    if (!Main.IsModEnabled || !Main.ModSettings.EnableDefamationMarketingCampaign || type != MarketingCampaignType.General)
        //        return;

        //    _campaignItems = ____campaignItems;
        //    _level = ____level;

        //    try
        //    {
        //        //List<MarketingCampaignDefinition> campaigns = _level.MarketingManager.GetCampaigns(type);
        //        //MarketingCampaignDefinition campaign = campaigns.FirstOrDefault(x => x.NameLocalised.ToAnalyticsTermString() == "General_S_Name"); //Marketing/General_S_Desc
        //        //var campaignObject = _campaignItems.FirstOrDefault(gameObject => gameObject.GetComponent<MarketingCampaignListItem>()?.Name?.text == campaign.NameLocalised.Translation);
        //        //campaign.NameLocalised.Term = "Defamation_Campaign"; //TODO: TH20.MarketingCampaignListItem public TMP_Text Name; or MarketingCampaignMenu private TMP_Text _campaignNameText; private TMP_Text _campaignDescriptionText; private void OnSelectCampaignCategory(MarketingCampaignType type, Button button
        //        //campaign.DescriptionLocalised.Term = "Defamation_Campaign";
        //        //campaignObject.GetComponent<MarketingCampaignListItem>().Name.text = "PLZZ";
        //    }
        //    catch (Exception ex)
        //    {
        //        Main.Logger.Log($"[MarketingCampaignMenu - Postfix] Exception occured: {ex}.");
        //    }
        //}
    }

    [HarmonyPatch(typeof(I2.Loc.LocalizationManager), "GetTranslation")]
    internal static class LocalizationManager_GetTranslation_Patch
    {
        private static bool Prefix(ref string __result, string Term)
        {
            if (!Main.IsModEnabled || !Main.ModSettings.EnableDefamationMarketingCampaign || !Term.Contains("General_S_"))
                return true;

            if (Term.Contains("General_S_Name"))
                __result = "Defamation Campaign";
            else if (Term.Contains("General_S_Desc"))
                __result = "So you've come to a point where your reputation overwhelms you? Can't handle the spotlight? You need to calm down the hordes storming your hospital entrance? Well, you've come to the right place then! We can launch a campaign where we will tarnish the hospital's reputation into oblivion! Better don't ask us how we do it.";
            else
                return true;

            return false;
        }
    }
}