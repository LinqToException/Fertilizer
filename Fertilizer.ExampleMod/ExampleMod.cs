namespace Fertilizer.ExampleMod
{
    public class ExampleMod : Mod
    {
        public override void OnEnable()
        {
            var harmony = new HarmonyLib.Harmony("fertilizer.example");
            harmony.PatchAll(GetType().Assembly);
        }
    }
}