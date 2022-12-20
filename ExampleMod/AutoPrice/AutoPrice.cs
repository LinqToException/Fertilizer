namespace Fertilizer.ExampleMod
{
    public class AutoPrice : Mod
    {
        public override void OnEnable()
        {
            var harmony = new HarmonyLib.Harmony("fertilizer.example");
            harmony.PatchAll(GetType().Assembly);
        }
    }
}