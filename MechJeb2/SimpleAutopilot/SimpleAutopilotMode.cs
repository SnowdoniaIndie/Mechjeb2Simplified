using System.Collections.Generic;
using UnityEngine;

namespace MuMech.SimpleAutopilot
{
    /// <summary>
    /// Base class for a SimpleAutopilot "type". Define one of these per autopilot and the manager
    /// gives it a toolbar button, visibility gating, a persistent HUD, and exclusivity handling.
    /// Everything a new type needs is here — no plumbing to repeat.
    /// </summary>
    public abstract class SimpleAutopilotMode
    {
        /// <summary>Short name, used in screen messages.</summary>
        public abstract string Name { get; }

        /// <summary>Apply this mode's MechJeb variables (called on engage).</summary>
        public abstract void OnEngage(MechJebCore core);

        /// <summary>Clear this mode's MechJeb variables (called on disengage).</summary>
        public abstract void OnDisengage(MechJebCore core);

        /// <summary>The lines of persistent HUD text shown while engaged (first line is the heading).</summary>
        public abstract IEnumerable<string> StatusLines(MechJebCore core);

        /// <summary>Tint of the "on" icon and the HUD text. Shared green so all autopilots match.</summary>
        public virtual Color IconColor => new Color(60f / 255f, 220f / 255f, 60f / 255f);

        /// <summary>Which placeholder glyph the toolbar icon draws when no PNG is supplied.</summary>
        public virtual SimpleAutopilotGlyph Glyph => SimpleAutopilotGlyph.Plane;

        /// <summary>GameData-relative path (no extension) to a WHITE glyph PNG, tinted in code for the
        /// on (IconColor) and off (grey) states. Null or a missing file falls back to the drawn glyph.</summary>
        public virtual string IconPath => null;

        /// <summary>Modes sharing a non-null group are mutually exclusive: engaging one disengages the others.</summary>
        public virtual string ExclusivityGroup => null;

        /// <summary>Whether this mode's button should currently be shown. Default: the vessel has a MechJeb core.</summary>
        public virtual bool ShouldShow(Vessel v) => HasCore(v);

        // ---- helpers for subclasses ----

        protected static bool HasCore(Vessel v) => v != null && v.GetMasterMechJeb() != null;

        protected static bool HasModule<T>(Vessel v) where T : PartModule =>
            v != null && v.FindPartModulesImplementing<T>().Count > 0;
    }
}
