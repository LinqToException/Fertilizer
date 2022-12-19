using HarmonyLib;
using MonoProtoFramework.NewUI;
using TopDownEditor;
using TopDownEditor.NewUI;

namespace Fertilizer.ExampleMod
{
    [HarmonyPatch(typeof(TopDownEditor.NewUI.Screens.StallQuantityCost), MethodType.Constructor, typeof(UIElementBase), typeof(InteractableObject), typeof(int), typeof(int), typeof(bool))]
    internal static class StallQuantityCostPatch
    {
        private static void Postfix(UINumberInput ___cost, int item, int starRating)
        {
            ___cost.Set(LootManager.FullItemLookup[item].GetMoneyToSell(starRating));
        }
    }
}