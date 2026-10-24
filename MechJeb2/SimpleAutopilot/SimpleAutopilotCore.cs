namespace MuMech.SimpleAutopilot
{
    /// <summary>
    /// Base for SimpleAutopilot's per-type flight computers. Each concrete subclass IS a full
    /// MechJebCore (so it provides the autopilot engine), but appears on its part under its own name
    /// and info, and its presence gates the matching toolbar mode. The core is always running and
    /// MechJeb's inherited per-part enable/disable toggle is hidden. Subclasses just override the
    /// display name + GetInfo (pattern: name as the header, one controlled parameter per line).
    /// </summary>
    public abstract class SimpleAutopilotCore : MechJebCore
    {
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            running = true;
            Fields["running"].guiActive = false;
            Fields["running"].guiActiveEditor = false;
        }
    }
}
