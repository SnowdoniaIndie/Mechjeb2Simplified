namespace MuMech.SimpleAutopilot
{
    /// <summary>
    /// Flight computer for the rover autopilot (a MechJebCore subclass). Added to the RoveMate
    /// (roverBody/_v2) via SimpleAutopilot.cfg; its presence gates <see cref="RoverAutopilotMode"/>.
    /// </summary>
    public class ModuleRoverAutopilot : SimpleAutopilotCore
    {
        public override string GetModuleDisplayName() => "Rover Autopilot";

        public override string GetInfo() => "Heading\nSpeed";
    }
}
