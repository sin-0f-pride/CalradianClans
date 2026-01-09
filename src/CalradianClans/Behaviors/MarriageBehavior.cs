


using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;



namespace CalradianClans.Behaviors
{
    internal sealed class MarriageBehavior : CampaignBehaviorBase
    {

        public override void RegisterEvents()
        {
            CampaignEvents.OnMarriageOfferedToPlayerEvent.AddNonSerializedListener(this, OnMarriageOfferedToPlayer);
            CampaignEvents.BeforeHeroesMarried.AddNonSerializedListener(this, OnBeforeHeroesMarried);
        }
        public void OnMarriageOfferedToPlayer(Hero suitor, Hero maiden)
        {
            int attraction = suitor.Clan == Clan.PlayerClan ? Campaign.Current.Models.RomanceModel.GetAttractionValuePercentage(maiden, suitor) : Campaign.Current.Models.RomanceModel.GetAttractionValuePercentage(suitor, maiden);
            Hero relevantMember = suitor.Clan == Clan.PlayerClan ? suitor : maiden;
            int relation = suitor.GetRelation(maiden);
            string text = "If I may be so bold, I think I love them already - unless you bare me ill will, please accept this proposal.";
            if (attraction + relation < 30)
            {
                text = "Please don't make me marry them - anybody else!";
            }
            else if (attraction + relation < 60)
            {
                text = "They would not be my choice, but if this is what you want, I am not willing it fight over it.";
            }
            else if (attraction + relation < 80)
            {
                text = "I think this is a strong match and will happily marry them.";
            }
            else if (attraction + relation < 94)
            {
                text = "Please accept this proposal. I will happily marry them.";
            }
            InformationManager.ShowInquiry(new InquiryData(new TextObject("{FIRST_NAME} wants you to know", null).SetTextVariable("FIRST_NAME", relevantMember.Name.ToString()).ToString(), text, true, true, "I'll consider it.", "This isn't about you!", delegate ()
            {
                ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, relevantMember, 2, true);
            }, delegate ()
            {
                ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, relevantMember, -2, true);
            }, "", 0f, null, null, null), true, false);
        }

        public void OnBeforeHeroesMarried(Hero hero, Hero hero2, bool notification)
        {
            if (AreHeroesRelated(hero, Hero.MainHero, 3))
            {
                int attractionValuePercentage = Campaign.Current.Models.RomanceModel.GetAttractionValuePercentage(hero, hero2);
                int relation = hero.GetRelation(hero2);
                int relationChange = InterestInMarriageMatch(attractionValuePercentage, relation);
                ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, hero, relationChange, true);
            }
            else if (AreHeroesRelated(hero2, Hero.MainHero, 3))
            {
                int attractionValuePercentage2 = Campaign.Current.Models.RomanceModel.GetAttractionValuePercentage(hero2, hero);
                int relation2 = hero2.GetRelation(hero);
                int relationChange2 = InterestInMarriageMatch(attractionValuePercentage2, relation2);
                ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, hero2, relationChange2, true);
            }
        }
        private int InterestInMarriageMatch(int attraction, int relation)
        {
            if (attraction + relation < 20)
            {
                return -5;
            }
            else if (attraction + relation < 40)
            {
                return -2;
            }
            else if (attraction + relation > 70)
            {
                return 3;
            }
            else if (attraction + relation > 90)
            {
                return 5;
            }
            return 0;
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

        public override void SyncData(IDataStore dataStore)
        {

        }
    }
}
