using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KSP.UI.Screens;
using UnityEngine;

namespace MuMech.SimpleAutopilot
{
    /// <summary>
    /// Owns every SimpleAutopilot mode: discovers them by reflection, gives each its own toolbar
    /// button (shown/hidden per the mode's ShouldShow), toggles engagement (honouring exclusivity
    /// groups), and draws each engaged mode's persistent HUD stacked above the navball. Adding a new
    /// autopilot type needs no changes here — just a new SimpleAutopilotMode subclass.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class SimpleAutopilotManager : MonoBehaviour
    {
        private class Entry
        {
            public SimpleAutopilotMode Mode;
            public ApplicationLauncherButton Button;
            public Texture2D IconOn;
            public Texture2D IconOff;
            public bool Engaged;
        }

        private static readonly Color OffColor = new Color(165f / 255f, 165f / 255f, 165f / 255f);

        private readonly List<Entry> _entries = new List<Entry>();
        private GUIStyle _statusStyle;

        private void Start()
        {
            DiscoverModes();

            GameEvents.onGUIApplicationLauncherReady.Add(RefreshAll);
            GameEvents.onVesselChange.Add(OnVesselChanged);
            GameEvents.onVesselWasModified.Add(OnVesselChanged);

            RefreshAll();
        }

        private void OnDestroy()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(RefreshAll);
            GameEvents.onVesselChange.Remove(OnVesselChanged);
            GameEvents.onVesselWasModified.Remove(OnVesselChanged);

            foreach (Entry e in _entries)
                RemoveButton(e);
        }

        private void DiscoverModes()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsAbstract || !t.IsSubclassOf(typeof(SimpleAutopilotMode)))
                    continue;

                var mode = (SimpleAutopilotMode)Activator.CreateInstance(t);
                _entries.Add(new Entry
                {
                    Mode = mode,
                    IconOff = LoadIcon(mode.IconPath, OffColor, mode.Glyph),
                    IconOn = LoadIcon(mode.IconPath, mode.IconColor, mode.Glyph)
                });
            }
        }

        // Loads the mode's WHITE glyph PNG from disk (readable) and tints it; falls back to the drawn
        // glyph in the tint colour if the mode gives no path or the file is missing.
        private static Texture2D LoadIcon(string gameDataPath, Color tint, SimpleAutopilotGlyph glyph)
        {
            if (!string.IsNullOrEmpty(gameDataPath))
            {
                string fullPath = KSPUtil.ApplicationRootPath + "GameData/" + gameDataPath + ".png";
                if (File.Exists(fullPath))
                {
                    var white = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                    if (white.LoadImage(File.ReadAllBytes(fullPath))) // LoadImage makes the texture readable
                        return Tint(white, tint);
                }
            }

            return SimpleAutopilotIcons.CreateIcon(tint, glyph);
        }

        // Multiplies every pixel by the tint (white -> tint colour, alpha preserved).
        private static Texture2D Tint(Texture2D source, Color tint)
        {
            Color[] pixels = source.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] *= tint;

            var result = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
            result.SetPixels(pixels);
            result.Apply(false);
            return result;
        }

        private void OnVesselChanged(Vessel v) => RefreshAll();

        private void RefreshAll()
        {
            Vessel v = FlightGlobals.ActiveVessel;
            foreach (Entry e in _entries)
            {
                if (e.Mode.ShouldShow(v))
                    AddButton(e);
                else
                    RemoveButton(e);
            }
        }

        private void AddButton(Entry e)
        {
            if (e.Button != null || ApplicationLauncher.Instance == null)
                return;

            e.Button = ApplicationLauncher.Instance.AddModApplication(
                () => OnClick(e, true), () => OnClick(e, false),
                null, null, null, null,
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                e.Engaged ? e.IconOn : e.IconOff);
        }

        private void RemoveButton(Entry e)
        {
            if (e.Button != null && ApplicationLauncher.Instance != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(e.Button);
                e.Button = null;
            }

            // Button vanished while engaged (core/part removed) -> stop and clear state.
            if (e.Engaged)
            {
                MechJebCore core = FlightGlobals.ActiveVessel?.GetMasterMechJeb();
                if (core != null)
                    e.Mode.OnDisengage(core);
                e.Engaged = false;
            }
        }

        private void OnClick(Entry e, bool on)
        {
            MechJebCore core = FlightGlobals.ActiveVessel?.GetMasterMechJeb();

            if (on)
            {
                if (core == null)
                {
                    Message(e.Mode.Name + ": no MechJeb core on this vessel");
                    e.Button?.SetFalse(false);
                    return;
                }

                // Exclusivity: disengage any engaged mode sharing this one's group.
                if (!string.IsNullOrEmpty(e.Mode.ExclusivityGroup))
                {
                    foreach (Entry other in _entries)
                    {
                        if (other != e && other.Engaged && other.Mode.ExclusivityGroup == e.Mode.ExclusivityGroup)
                            SetEngaged(other, false, core);
                    }
                }

                SetEngaged(e, true, core);
                Message(e.Mode.Name + " engaged");
            }
            else
            {
                SetEngaged(e, false, core);
                Message(e.Mode.Name + " disengaged");
            }
        }

        private static void SetEngaged(Entry e, bool engaged, MechJebCore core)
        {
            if (engaged)
            {
                e.Mode.OnEngage(core);
                e.Engaged = true;
                e.Button?.SetTexture(e.IconOn);
                e.Button?.SetTrue(false);
            }
            else
            {
                if (core != null)
                    e.Mode.OnDisengage(core);
                e.Engaged = false;
                e.Button?.SetTexture(e.IconOff);
                e.Button?.SetFalse(false);
            }
        }

        private void OnGUI()
        {
            MechJebCore core = FlightGlobals.ActiveVessel?.GetMasterMechJeb();
            if (core == null)
                return;

            EnsureStyle();

            const float w = 220f;
            const float lineHeight = 20f;
            float blockBottom = Screen.height - 305f; // baseline just above the navball

            foreach (Entry e in _entries)
            {
                if (!e.Engaged)
                    continue;

                var lines = new List<string>(e.Mode.StatusLines(core));
                if (lines.Count == 0)
                    continue;

                float h = lines.Count * lineHeight;
                _statusStyle.normal.textColor = e.Mode.IconColor;
                GUI.Label(new Rect(Screen.width / 2f - w / 2f, blockBottom - h, w, h),
                    string.Join("\n", lines.ToArray()), _statusStyle);

                blockBottom -= h + 8f; // stack the next engaged mode above this one
            }
        }

        private void EnsureStyle()
        {
            if (_statusStyle != null)
                return;

            _statusStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.LowerCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 14
            };
        }

        private static void Message(string text)
        {
            ScreenMessages.PostScreenMessage(text, 3f, ScreenMessageStyle.UPPER_CENTER);
        }
    }
}
