using System;
using UnityEngine;

namespace VulpesTool
{
    [Flags]
    public enum ColorsGUI
    {
        None = 0,

        // Основные цвета
        Red = 1 << 0,
        Green = 1 << 1,
        Blue = 1 << 2,

        // Комбинации для вторичных цветов
        Yellow = Red | Green,
        Magenta = Red | Blue,
        Cyan = Green | Blue,

        // Яркость
        Light = 1 << 3,
        Dark = 1 << 4,

        // Прозрачность
        Transparent = 1 << 5,

        // Готовые комбинации
        ErrorRed = Red | Dark,
        WarningYellow = Yellow | Light,
        SuccessGreen = Green | Light,
        InfoBlue = Blue | Light,

        // Особые
        Gray = Red | Green | Blue,
        White = Gray | Light,
        Black = Gray | Dark,

        // Пастельные тона
        PastelRed = Red | Light | Transparent,
        PastelGreen = Green | Light | Transparent,
        PastelBlue = Blue | Light | Transparent,
        PastelYellow = Yellow | Light | Transparent
    }

    public static partial class Extension
    {
        public static Color GetColorFromFlags(this ColorsGUI color)
        {
            float r, g, b, a = 1F;

            if (color.HasFlag(ColorsGUI.Red)) r = 1f; else r = 0.5f;
            if (color.HasFlag(ColorsGUI.Green)) g = 1f; else g = 0.5f;
            if (color.HasFlag(ColorsGUI.Blue)) b = 1f; else b = 0.5f;

            if (color.HasFlag(ColorsGUI.Light))
            {
                r = Mathf.Min(1f, r * 1.2f);
                g = Mathf.Min(1f, g * 1.2f);
                b = Mathf.Min(1f, b * 1.2f);
            }

            if (color.HasFlag(ColorsGUI.Dark))
            {
                r *= 0.7f;
                g *= 0.7f;
                b *= 0.7f;
            }

            if (color.HasFlag(ColorsGUI.Transparent))
            {
                a = 0.7f;
            }

            return new Color(r, g, b, a);
        }
    }
}

