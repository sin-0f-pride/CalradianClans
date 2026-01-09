using System;
using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace CalradianClans.Models
{
    public class CalradianClansMarriageModel : MarriageModel
    {
        private readonly MarriageModel? _model;

        public CalradianClansMarriageModel(MarriageModel? model)
        {
            _model = model;
        }
        public override bool IsCoupleSuitableForMarriage(Hero firstHero, Hero secondHero)
        {
            if (firstHero.IsFemale && firstHero.Age > 40f && secondHero.Age < 40f)
            {
                return false;
            }
            if (secondHero.IsFemale && secondHero.Age > 40f && firstHero.Age < 40f)
            {
                return false;
            }
            if (IsClanSuitableForMarriage(firstHero.Clan) && IsClanSuitableForMarriage(secondHero.Clan))
            {
                if (firstHero.Clan != null && firstHero.Clan.Leader == firstHero && secondHero.Clan != null && secondHero.Clan.Leader == secondHero)
                {
                    return false;
                }
                if (firstHero.IsFemale != secondHero.IsFemale && !AreHeroesRelated(firstHero, secondHero, 3))
                {
                    Hero courtedHeroInOtherClan = Romance.GetCourtedHeroInOtherClan(firstHero, secondHero);
                    if (courtedHeroInOtherClan != null && courtedHeroInOtherClan != secondHero)
                    {
                        return false;
                    }
                    Hero courtedHeroInOtherClan2 = Romance.GetCourtedHeroInOtherClan(secondHero, firstHero);
                    return (courtedHeroInOtherClan2 == null || courtedHeroInOtherClan2 == firstHero) && firstHero.CanMarry() && secondHero.CanMarry();
                }
            }
            return false;
        }
        public override bool IsClanSuitableForMarriage(Clan clan)
        {
            return clan != null && !clan.IsBanditFaction && !clan.IsRebelClan;
        }
        public override float NpcCoupleMarriageChance(Hero firstHero, Hero secondHero)
        {
            float result = 0f;
            if (IsCoupleSuitableForMarriage(firstHero, secondHero))
            {
                Hero hero = !firstHero.IsFemale ? firstHero : secondHero;
                Hero hero2 = firstHero.IsFemale ? firstHero : secondHero;
                MBList<Hero> children = hero.Children;
                MBList<Hero> children2 = hero2.Children;
                result += (hero.Age - 25f) / 25f;
                result += (hero2.Age - 22f) / 20f;
                result -= (children != null ? children.Count : 0) / 10;
                result -= (children2 != null ? children2.Count : 0) / 5;
                result /= hero.Age > 40f && hero2.Age > 30f ? 3f : 12f;
                result -= Math.Abs(hero.Clan.Tier - hero2.Clan.Tier) / 4;
            }
            return result;
        }
        public override Clan GetClanAfterMarriage(Hero firstHero, Hero secondHero)
        {
            if (firstHero.IsHumanPlayerCharacter)
            {
                return firstHero.Clan;
            }
            if (secondHero.IsHumanPlayerCharacter)
            {
                return secondHero.Clan;
            }
            if (firstHero.Clan.Leader == firstHero)
            {
                return firstHero.Clan;
            }
            if (secondHero.Clan.Leader == secondHero)
            {
                return secondHero.Clan;
            }
            if (firstHero.IsFemale && ((firstHero.Mother != null && firstHero.Mother == firstHero.Clan.Leader) || (firstHero.Father != null && firstHero.Father == firstHero.Clan.Leader)))
            {
                foreach (Hero hero in firstHero.Clan.Leader.Children)
                {
                    if (!hero.IsFemale)
                    {
                        return secondHero.Clan;
                    }
                    if (hero.Age > firstHero.Age)
                    {
                        return secondHero.Clan;
                    }
                }
                return firstHero.Clan;
            }
            if (secondHero.IsFemale && ((secondHero.Mother != null && secondHero.Mother == secondHero.Clan.Leader) || (secondHero.Father != null && secondHero.Father == secondHero.Clan.Leader)))
            {
                foreach (Hero hero2 in secondHero.Clan.Leader.Children)
                {
                    if (!hero2.IsFemale)
                    {
                        return firstHero.Clan;
                    }
                    if (hero2.Age > secondHero.Age)
                    {
                        return firstHero.Clan;
                    }
                }
                return secondHero.Clan;
            }
            if (!firstHero.IsFemale)
            {
                return firstHero.Clan;
            }
            return secondHero.Clan;
        }
        public override bool ShouldNpcMarriageBetweenClansBeAllowed(Clan consideringClan, Clan targetClan)
        {
            if (consideringClan.Kingdom != null && targetClan.Kingdom != null)
            {
                if (consideringClan.Kingdom != targetClan.Kingdom && (consideringClan.Kingdom.RulingClan != consideringClan || targetClan.Kingdom.RulingClan != targetClan))
                {
                    return false;
                }
                if (consideringClan.Kingdom.RulingClan != consideringClan || targetClan.Kingdom.RulingClan != targetClan)
                {
                    if (Campaign.Current.Models.MapDistanceModel.GetDistance(consideringClan.HomeSettlement, targetClan.HomeSettlement, false, false, MobileParty.NavigationType.All) > 250f)
                    {
                        return false;
                    }
                }
            }
            else if (consideringClan.Culture != targetClan.Culture)
            {
                return false;
            }
            return targetClan != consideringClan && !consideringClan.IsAtWarWith(targetClan) && consideringClan.GetRelationWithClan(targetClan) >= -25;
        }
        private bool AreHeroesRelatedAux1(Hero firstHero, Hero secondHero, int ancestorDepth)
        {
            return firstHero == secondHero || (ancestorDepth > 0 && ((secondHero.Mother != null && AreHeroesRelatedAux1(firstHero, secondHero.Mother, ancestorDepth - 1)) || (secondHero.Father != null && AreHeroesRelatedAux1(firstHero, secondHero.Father, ancestorDepth - 1))));
        }
        private bool AreHeroesRelatedAux2(Hero firstHero, Hero secondHero, int ancestorDepth, int secondAncestorDepth)
        {
            return AreHeroesRelatedAux1(firstHero, secondHero, secondAncestorDepth) || (ancestorDepth > 0 && ((firstHero.Mother != null && AreHeroesRelatedAux2(firstHero.Mother, secondHero, ancestorDepth - 1, secondAncestorDepth)) || (firstHero.Father != null && AreHeroesRelatedAux2(firstHero.Father, secondHero, ancestorDepth - 1, secondAncestorDepth))));
        }
        private bool AreHeroesRelated(Hero firstHero, Hero secondHero, int ancestorDepth)
        {
            return AreHeroesRelatedAux2(firstHero, secondHero, ancestorDepth, ancestorDepth);
        }
        public override int MinimumMarriageAgeMale => _model.MinimumMarriageAgeMale;
        public override int MinimumMarriageAgeFemale => _model.MinimumMarriageAgeFemale;
        public override List<Hero> GetAdultChildrenSuitableForMarriage(Hero hero) => _model.GetAdultChildrenSuitableForMarriage(hero);
        public override int GetEffectiveRelationIncrease(Hero firstHero, Hero secondHero) => _model.GetEffectiveRelationIncrease(firstHero, secondHero);
        public override bool IsSuitableForMarriage(Hero maidenOrSuitor) => _model.IsSuitableForMarriage(maidenOrSuitor);
    }
}