namespace MuMech.SimpleAutopilot
{
    /// <summary>
    /// Flight computer for the aircraft autopilot (a MechJebCore subclass). Added to the MK2 Drone
    /// Core via SimpleAutopilot.cfg; its presence gates <see cref="AircraftHoldMode"/>.
    /// </summary>
    public class ModuleFlightAutopilot : SimpleAutopilotCore
    {
        public override string GetModuleDisplayName() => "Flight Autopilot";

        public override string GetInfo() => "Altitude\nHeading\nSpeed";
    }
}
