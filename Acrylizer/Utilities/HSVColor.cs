using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Acrylizer.Utilities
{
    public struct HSVColor
    {
        public float H { get; set; }
        public float S { get; set; }
        public float V { get; set; }

        public static HSVColor FromHSV(float h, float s, float v)
        {
            return new HSVColor() { H = h, S = s, V = v };
        }

        public override string ToString()
        {
            return string.Format("H:{0}, S:{1}, V:{2}", H, S, V);
        }

        public Color ToRGB()
        {
            int Hi = ((int)(H / 60.0)) % 6;
            float f = H / 60.0f - (int)(H / 60.0);
            float p = V * (1 - S);
            float q = V * (1 - f * S);
            float t = V * (1 - (1 - f) * S);

            switch (Hi)
            {
                case 0:
                    return FromRGB(V, t, p);
                case 1:
                    return FromRGB(q, V, p);
                case 2:
                    return FromRGB(p, V, t);
                case 3:
                    return FromRGB(p, q, V);
                case 4:
                    return FromRGB(t, p, V);
                case 5:
                    return FromRGB(V, p, q);
            }

            // ここには来ない
            throw new InvalidOperationException();
        }

        private Color FromRGB(float fr, float fg, float fb)
        {
            fr *= 255;
            fg *= 255;
            fb *= 255;
            byte r = (byte)((fr < 0) ? 0 : (fr > 255) ? 255 : fr);
            byte g = (byte)((fg < 0) ? 0 : (fg > 255) ? 255 : fg);
            byte b = (byte)((fb < 0) ? 0 : (fb > 255) ? 255 : fb);
            return Color.FromRgb(r, g, b);
        }
    }

    public static class ColorExtension
    {
        public static HSVColor ToHSV(this Color c)
        {
            float r = c.R / 255.0f;
            float g = c.G / 255.0f;
            float b = c.B / 255.0f;

            var list = new float[] { r, g, b };
            var max = list.Max();
            var min = list.Min();

            float h, s, v;
            if (max == min)
                h = 0;
            else if (max == r)
                h = (60 * (g - b) / (max - min) + 360) % 360;
            else if (max == g)
                h = 60 * (b - r) / (max - min) + 120;
            else
                h = 60 * (r - g) / (max - min) + 240;

            if (max == 0)
                s = 0;
            else
                s = (max - min) / max;

            v = max;

            return new HSVColor() { H = h, S = s, V = v };
        }
    }
}
