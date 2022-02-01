using System;
using System.Reflection;

using AutoInority.Extentions;

using Harmony;

using UnityEngine;

namespace TodayOrdeal
{
    public class Harmony_Patch
    {
        public const string ModName = "Lobotomy.inority.RelocateAgents";

        public Harmony_Patch()
        {
            Invoke(() =>
            {
                HarmonyInstance mod = HarmonyInstance.Create(ModName);
                PatchUnitMouseEventManager(mod);
                PatchSefiraRecoverGaugeUI(mod);
            });
        }

        public static bool ClickSefiraReturn_Prefix(SefiraRecoverGaugeUI __instance)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                Invoke(() => SefiraManager.instance.sefiraList.ForEach(x => x.MoveToSefira(__instance.sefiraEnum)));
                return false;
            }
            return true;
        }

        public static void UnitMouseEventManager_Update_Postfix(UnitMouseEventManager __instance)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    Invoke(() => SefiraManager.instance.sefiraList.ForEach(x => x.MoveToNeighborPassage()));
                    return;
                }
                Invoke(() => SefiraManager.instance.sefiraList.ForEach(x => x.ReturnAgentsToSefira()));
            }
        }

        public void PatchSefiraRecoverGaugeUI(HarmonyInstance mod)
        {
            var prefix = typeof(Harmony_Patch).GetMethod(nameof(ClickSefiraReturn_Prefix));
            var origin = typeof(SefiraRecoverGaugeUI).GetMethod(nameof(SefiraRecoverGaugeUI.OnClickSefiraReturn));
            mod.Patch(origin, new HarmonyMethod(prefix), null);
        }

        public void PatchUnitMouseEventManager(HarmonyInstance mod)
        {
            var postfix = typeof(Harmony_Patch).GetMethod(nameof(UnitMouseEventManager_Update_Postfix));
            var origin = typeof(UnitMouseEventManager).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
            mod.Patch(origin, null, new HarmonyMethod(postfix));
        }

        private static void Invoke(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}