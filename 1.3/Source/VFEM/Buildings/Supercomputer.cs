using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;
using VFEM;

namespace VFEMech
{
    public class Supercomputer : Building
    {
        private CompPowerTrader compPower;
        private int lastTick;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            compPower = this.GetComp<CompPowerTrader>();
        }

        public override void TickLong()
        {
            base.TickLong();
            if (compPower.PowerOn && this.Faction == Faction.OfPlayer)
            {
                AdvancePlayerResearch(Find.TickManager.TicksGame - lastTick);
            }
            // Consider updating research state once when compPower is turned off
            lastTick = Find.TickManager.TicksGame;
        }

        private void AdvancePlayerResearch(int ticks)
        {
            var proj = Find.ResearchManager.currentProj;
            if (proj == null)
                return;
            Verse.Log.Message($"Advancing research for {ticks} ticks -> {((float)ticks / GenDate.TicksPerHour)} points");
            FieldInfo fieldInfo = AccessTools.Field(typeof(ResearchManager), "progress");
            Dictionary<ResearchProjectDef, float> dictionary = fieldInfo.GetValue(Find.ResearchManager) as Dictionary<ResearchProjectDef, float>;
            if (dictionary.ContainsKey(proj))
            {
                dictionary[proj] += MechShipsMod.settings.VFEM_SuperComputerResearchPointYield * ((float)ticks / GenDate.TicksPerHour);
            }
            if (proj.IsFinished)
            {
                Find.ResearchManager.FinishProject(proj, doCompletionDialog: true);
            }
        }
    }
}
