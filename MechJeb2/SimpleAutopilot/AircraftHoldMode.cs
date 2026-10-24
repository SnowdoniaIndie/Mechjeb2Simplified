using System.Collections.Generic;
using UnityEngine;

namespace MuMech.SimpleAutopilot
{
    /// <summary>
    /// Full aircraft hold: on engage, captures and holds the craft's CURRENT altitude, heading and
    /// speed (with a fixed V/S cap and max bank). This is the original SimpleAutopilot behaviour,
    /// now expressed as a mode. Shown wherever the shared flight computer (ModuleSimpleAutopilot) is.
    /// </summary>
    public class AircraftHoldMode : SimpleAutopilotMode
    {
        private const double VertSpeedCap  = 50; // V/S +-
        private const double MaxBankDegrees = 30; // Bank Angle +-

        public override string Name => "Aircraft Hold";
        public override string ExclusivityGroup => "autopilot";
        public override string IconPath => "MechJeb2/SimpleAutopilot/Icons/flight";

        public override bool ShouldShow(Vessel v) => HasCore(v) && HasModule<ModuleFlightAutopilot>(v);

        public override void OnEngage(MechJebCore core)
        {
            MechJebModuleAirplaneAutopilot ap = core.Airplane;
            if (ap == null)
                return;

            VesselState vs = core.VesselState;

            ap.Enabled = true;                    // module-level enable MUST come first
            ap.AltitudeTarget  = vs.AltitudeASL;  // hold CURRENT altitude
            ap.HeadingTarget   = vs.Heading;      // hold CURRENT heading
            ap.SpeedTarget     = vs.SpeedSurface; // hold CURRENT speed
            ap.VertSpeedTarget = VertSpeedCap;
            ap.BankAngle       = MaxBankDegrees;
            ap.EnableAltitudeHold();              // altitude + auto-cascades vertical-speed hold
            ap.EnableHeadingHold();               // heading + roll
            ap.EnableSpeedHold();                 // speed (throttle controller)
        }

        public override void OnDisengage(MechJebCore core)
        {
            MechJebModuleAirplaneAutopilot ap = core.Airplane;
            if (ap == null)
                return;

            ap.DisableAltitudeHold();
            ap.DisableHeadingHold();
            ap.DisableSpeedHold();
            ap.Enabled = false;
        }

        public override IEnumerable<string> StatusLines(MechJebCore core)
        {
            yield return "AUTOPILOT ENGAGED";

            MechJebModuleAirplaneAutopilot ap = core.Airplane;
            if (ap != null)
            {
                yield return "Altitude: " + ap.AltitudeTarget.ToString("0") + " m";
                yield return "Heading: " + ap.HeadingTarget.ToString("000") + "°";
                yield return "Speed: " + ap.SpeedTarget.ToString("0") + " m/s";
            }
        }
    }
}
