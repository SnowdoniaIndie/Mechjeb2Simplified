using UnityEngine;

namespace MuMech.SimpleAutopilot
{
    /// <summary>Which placeholder glyph a mode's toolbar icon draws.</summary>
    public enum SimpleAutopilotGlyph { Plane, Rover }

    /// <summary>
    /// Procedurally-generated PLACEHOLDER toolbar icons (no external PNG assets). Draws the requested
    /// glyph in the requested colour — grey for "off", the mode's colour for "on". Swap for real art
    /// later by loading textures instead.
    /// </summary>
    internal static class SimpleAutopilotIcons
    {
        private const int Size = 38;

        public static Texture2D CreateIcon(Color color, SimpleAutopilotGlyph glyph)
        {
            var tex = new Texture2D(Size, Size, TextureFormat.ARGB32, false);

            var pixels = new Color32[Size * Size];
            var clear = new Color32(0, 0, 0, 0);
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = clear;

            Color32 c = color;

            if (glyph == SimpleAutopilotGlyph.Rover)
                DrawRover(pixels, c);
            else
                DrawPlane(pixels, c);

            tex.SetPixels32(pixels);
            tex.Apply(false);
            return tex;
        }

        // Top-view aircraft: fuselage (vertical), main wing (horizontal), tailplane (short, near tail).
        private static void DrawPlane(Color32[] p, Color32 c)
        {
            VLine(p, 18, 6, 32, c);
            VLine(p, 19, 6, 32, c);
            HLine(p, 6, 32, 18, c);
            HLine(p, 6, 32, 19, c);
            HLine(p, 13, 24, 9, c);
            HLine(p, 13, 24, 10, c);
        }

        // Side-view car: two wheels below a wide chassis with a narrower cabin on top.
        private static void DrawRover(Color32[] p, Color32 c)
        {
            FillRect(p, 10, 8, 15, 13, c);  // rear wheel
            FillRect(p, 23, 8, 28, 13, c);  // front wheel
            FillRect(p, 7, 13, 31, 20, c);  // chassis
            FillRect(p, 13, 20, 25, 27, c); // cabin
        }

        private static void FillRect(Color32[] p, int x0, int y0, int x1, int y1, Color32 c)
        {
            for (int y = y0; y <= y1; y++)
                for (int x = x0; x <= x1; x++)
                    Set(p, x, y, c);
        }

        private static void HLine(Color32[] p, int x0, int x1, int y, Color32 c)
        {
            for (int x = x0; x <= x1; x++)
                Set(p, x, y, c);
        }

        private static void VLine(Color32[] p, int x, int y0, int y1, Color32 c)
        {
            for (int y = y0; y <= y1; y++)
                Set(p, x, y, c);
        }

        private static void Set(Color32[] p, int x, int y, Color32 c)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size)
                return;
            p[y * Size + x] = c;
        }
    }
}
