﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using VFE.Mechanoids.Needs;

namespace VFE.Mechanoids.HarmonyPatches
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
    public static class MechanoidsObeyOrders
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (pawn.drafter != null)
                __result = true;
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddDraftedOrders")]
    public static class AddDraftedOrders_Patch
    {
        public static bool Prefix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            if (pawn.RaceProps.IsMechanoid && pawn.needs.TryGetNeed<Need_Power>() is Need_Power need && need.CurLevel <= 0f)
            {
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(ForbidUtility), "CaresAboutForbidden")]
    public static class CaresAboutForbidden_Patch
    {
        public static void Postfix(bool __result, Pawn pawn, bool cellTarget)
        {
            if (!__result)
            {
            }
        }
    }

    [HarmonyPatch(typeof(WanderUtility), "GetColonyWanderRoot")]
    public static class GetColonyWanderRoot_Patch
    {
        public static void Postfix(IntVec3 __result, Pawn pawn)
        {
            if (pawn.RaceProps.IsMechanoid)
            {
                pawn.Map.debugDrawer.FlashCell(__result);
            }
        }
    }

    [HarmonyPatch(typeof(RCellFinder), "RandomWanderDestFor")]
    public static class RandomWanderDestFor_Patch
    {
        public static void Postfix(ref IntVec3 __result, Pawn pawn, IntVec3 root, float radius, Func<Pawn, IntVec3, IntVec3, bool> validator, Danger maxDanger)
        {
            if (pawn.RaceProps.IsMechanoid && pawn.Faction == Faction.OfPlayer && __result.IsForbidden(pawn) && pawn.playerSettings.AreaRestriction.ActiveCells.Count() > 0)
            {
                __result = pawn.playerSettings.AreaRestriction.ActiveCells.OrderBy(x => x.DistanceTo(pawn.Position)).Take(10).RandomElement();
            }
        }
    }
}
