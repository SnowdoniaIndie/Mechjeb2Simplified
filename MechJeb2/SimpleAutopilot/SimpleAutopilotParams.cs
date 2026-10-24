namespace MuMech.SimpleAutopilot
{
    /// <summary>
    /// Adds a "SimpleAutopilot" section to the game's Difficulty Settings (new-game setup and the
    /// in-game pause → Difficulty Options) with one per-save toggle: "Enable MechJeb menus".
    /// Defaults to OFF. When off, MechJeb's own toolbar button and menu windows are hidden
    /// (SimpleAutopilot's own autopilot buttons are unaffected). Turn it on to get the full MechJeb
    /// UI back — handy if we ever add more autopilots that need MechJeb's windows.
    /// KSP auto-discovers CustomParameterNode subclasses, so no ModuleManager patch is needed.
    /// </summary>
    public class SimpleAutopilotParams : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomParameterUI("Enable MechJeb menus",
            toolTip = "Show MechJeb's own toolbar button and menu windows. SimpleAutopilot's own buttons always show.")]
        public bool enableMechJebMenus = false;

        public override string Title => "MechJeb";
        public override string Section => "SimpleAutopilot";
        public override string DisplaySection => "SimpleAutopilot";
        public override int SectionOrder => 1;
        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;
        public override bool HasPresets => false;

        /// <summary>Safe read of the toggle; returns false (menus hidden) when no game is loaded.</summary>
        public static bool MechJebMenusEnabled
        {
            get
            {
                if (HighLogic.CurrentGame == null)
                    return false;

                try
                {
                    return HighLogic.CurrentGame.Parameters.CustomParams<SimpleAutopilotParams>().enableMechJebMenus;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
