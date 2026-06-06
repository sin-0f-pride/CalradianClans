using System;
using System.IO;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

using HarmonyLib;

using CalradianClans.Behaviors;
using CalradianClans.Models;

namespace CalradianClans
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            new Harmony("mod.bannerlord.calradianclans").PatchAll();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=0000}Calradian Clans loaded").ToString(), Color.FromUint(0x00E67E22)));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject is CampaignGameStarter starter)
            {
                starter.AddBehavior(new MarriageBehavior());
                starter.AddModel(new CalradianClansMarriageModel(GetModel<MarriageModel>(starter)));
            }
        }

        private T GetModel<T>(CampaignGameStarter starter)
        {
            foreach (GameModel model in starter.Models)
            {
                if (model is T gameModel)
                {
                    return gameModel;
                }
            }
            return default!;
        }

        public static void Log(string message)
        {
            string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord", "Logs");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            string path = Path.Combine(text, "TC.txt");
            using (StreamWriter streamWriter = new StreamWriter(path, true))
            {
                streamWriter.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message);
            }
        }
    }
}