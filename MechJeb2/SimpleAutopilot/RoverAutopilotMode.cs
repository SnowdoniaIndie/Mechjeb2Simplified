using System.Collections.Generic;
using UnityEngine;

namespace MuMech.SimpleAutopilot
{
    /// <summary>
    /// Rover autopilot: on engage, snapshots and holds the rover's CURRENT heading and ground speed
    /// using MechJeb's rover controller. Its button appears only on parts carrying
    /// <see cref="ModuleRoverAutopilot"/> (the RoveMate). Shares the "autopilot" exclusivity group,
    /// so engaging it disengages the aircraft hold and vice versa.
    /// </summary>
    public class RoverAutopilotMode : SimpleAutopilotMode
    {
        public override string Name => "Rover Autopilot";
        public override SimpleAutopilotGlyph Glyph => SimpleAutopilotGlyph.Rover;
        public override string ExclusivityGroup => "autopilot";
        public override string IconPath => "MechJeb2/SimpleAutopilot/Icons/rover";

        public override bool ShouldShow(Vessel v) => HasCore(v) && HasModule<ModuleRoverAutopilot>(v);

        public override void OnEngage(MechJebCore core)
        {
            MechJebModuleRoverController rover = core.Rover;
            if (rover == null)
                return;

            rover.Enabled = true;                                         // REQUIRED: MechJebCore only calls Drive() on enabled modules
            rover.heading.Val = core.VesselState.Heading;                 // snapshot current heading
            rover.speed.Val   = core.VesselState.SpeedSurfaceHorizontal;  // snapshot current ground speed
            rover.ControlHeading = true;
            rover.ControlSpeed   = true;
        }

        public override void OnDisengage(MechJebCore core)
        {
            MechJebModuleRoverController rover = core.Rover;
            if (rover == null)
                return;

            rover.ControlHeading = false;
            rover.ControlSpeed   = false;
            rover.Enabled = false;
        }

        public override IEnumerable<string> StatusLines(MechJebCore core)
        {
            yield return "ROVER AUTOPILOT";

            MechJebModuleRoverController rover = core.Rover;
            if (rover != null)
            {
                yield return "Heading: " + rover.heading.Val.ToString("000") + "°";
                yield return "Speed: " + rover.speed.Val.ToString("0") + " m/s";
            }
        }
    }
}
