using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

using HarmonyLib;

namespace CalradianClans.Patches
{
    [HarmonyPatch(typeof(MarriageOfferCampaignBehavior), "CanOfferMarriageForClan")]
    internal class CanOfferMarriageForClanPatch
    {
        public static void Postfix(MarriageOfferCampaignBehavior __instance, Clan consideringClan, ref bool __result)
        {
            if (__result && (consideringClan.Tier > Clan.PlayerClan.Tier + 1 || (Clan.PlayerClan.MapFaction.IsKingdomFaction && Clan.PlayerClan.MapFaction != consideringClan.MapFaction)))
            {
                __result = false;
            }
        }
    }
}