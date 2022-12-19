namespace Fertilizer
{
    /// <summary>
    /// Defines a mod, or part of a mod. This class will be instantiated
    /// as soon as the game has been opened.
    /// </summary>
    public abstract class Mod
    {
        /// <summary>
        /// Called when the mod should be enabled. Currently, this is called
        /// immediately after the class has been created.
        /// </summary>
        public virtual void OnEnable()
        {
        }
    }
}