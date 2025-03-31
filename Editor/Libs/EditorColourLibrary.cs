using UnityEngine;
using UnityEditor;

namespace Hierarchy.Libraries {
    /// <summary>
    /// Contains all the colours used in the editor.
    /// Found @ https://www.foundations.unity.com/fundamentals/color-palette
    /// </summary>
    public static class EditorColourLibrary {
        private static class DarkThemeColours {
            // Backgrounds
            public static readonly Color Background1 = HexToRgba("#202020");       // Window background
            public static readonly Color Background2 = HexToRgba("#282828");       // Panel background
            
            // Borders
            public static readonly Color Border1 = HexToRgba("#3C3C3C");           // Standard border
            public static readonly Color Border2 = HexToRgba("#606060");           // Focus border
            public static readonly Color Border3 = HexToRgba("#808080");           // Highlight border
            
            // Text
            public static readonly Color Text1 = HexToRgba("#D7D7D7");             // Primary text
            public static readonly Color Text2 = HexToRgba("#FFFFFFB2");           // Secondary text (70% opacity)
            public static readonly Color Text3 = HexToRgba("#FFFFFF80");           // Disabled text (50% opacity)
            
            // Buttons
            public static readonly Color ButtonNormal = HexToRgba("#585858");      // Normal button
            public static readonly Color ButtonHover = HexToRgba("#676767");       // Hovered button
            public static readonly Color ButtonPressed = HexToRgba("#46607C");     // Pressed button
            public static readonly Color ButtonDisabled = HexToRgba("#404040");    // Disabled button
            
            // List Items
            public static readonly Color ListItemNormal = HexToRgba("#303030");    // Normal list item
            public static readonly Color ListItemHover = HexToRgba("#383838");     // Hovered list item
            public static readonly Color ListItemSelected = HexToRgba("#2D3B55");  // Selected list item
            public static readonly Color ListItemAlt = HexToRgba("#262626");       // Alternating list item
            
            // Toolbar
            public static readonly Color ToolbarBackground = HexToRgba("#292929"); // Toolbar background
            public static readonly Color ToolbarBorder = HexToRgba("#3C3C3C");     // Toolbar 
            public static readonly Color ToolbarText = HexToRgba("#C4C4C4");        // Toolbar text
            
            // Status Colors
            public static readonly Color Warning = HexToRgba("#F5A623");           // Warning
            public static readonly Color Error = HexToRgba("#FF3333");             // Error
            public static readonly Color Success = HexToRgba("#6CD97E");           // Success
        }
        
        private static class LightThemeColours {
            // Backgrounds
            public static readonly Color Background1 = HexToRgba("#F0F0F0");       // Window background
            public static readonly Color Background2 = HexToRgba("#FCFCFC");       // Panel background
            
            // Borders
            public static readonly Color Border1 = HexToRgba("#C8C8C8");           // Standard border
            public static readonly Color Border2 = HexToRgba("#A0A0A0");           // Focus border
            public static readonly Color Border3 = HexToRgba("#707070");           // Highlight border
            
            // Text
            public static readonly Color Text1 = HexToRgba("#000000");             // Primary text
            public static readonly Color Text2 = HexToRgba("#000000B2");           // Secondary text (70% opacity)
            public static readonly Color Text3 = HexToRgba("#00000080");           // Disabled text (50% opacity)
            
            // Buttons
            public static readonly Color ButtonNormal = HexToRgba("#E4E4E4");      // Normal button
            public static readonly Color ButtonHover = HexToRgba("#ECECEC");       // Hovered button
            public static readonly Color ButtonPressed = HexToRgba("#96C3FB");     // Pressed button
            public static readonly Color ButtonDisabled = HexToRgba("#F0F0F0");    // Disabled button
            
            // List Items
            public static readonly Color ListItemNormal = HexToRgba("#FCFCFC");    // Normal list item
            public static readonly Color ListItemHover = HexToRgba("#F0F0F0");     // Hovered list item
            public static readonly Color ListItemSelected = HexToRgba("#D8E5F9");  // Selected list item
            public static readonly Color ListItemAlt = HexToRgba("#F5F5F5");       // Alternating list item
            
            // Toolbar
            public static readonly Color ToolbarBackground = HexToRgba("#E5E5E5"); // Toolbar background
            public static readonly Color ToolbarBorder = HexToRgba("#C8C8C8");     // Toolbar border
            public static readonly Color ToolbarText = HexToRgba("#090909");        // Toolbar text
            
            // Status Colors
            public static readonly Color Warning = HexToRgba("#E6A700");           // Warning
            public static readonly Color Error = HexToRgba("#D83C3E");             // Error
            public static readonly Color Success = HexToRgba("#4DB55A");           // Success
        }
        
        // Default Border Colors
        public static Color DefaultBorder => IsProSkin ? DarkThemeColours.Border1 : LightThemeColours.Border1;
        public static Color ToolbarBorder => IsProSkin ? DarkThemeColours.ToolbarBorder : LightThemeColours.ToolbarBorder;
        public static Color WindowBorder => IsProSkin ? DarkThemeColours.Border1 : LightThemeColours.Border1;
        public static Color TitleBorder => IsProSkin ? DarkThemeColours.Border2 : LightThemeColours.Border2;
        public static Color HighlightBorder => IsProSkin ? DarkThemeColours.Border3 : LightThemeColours.Border3;
        
        // Background Colors
        public static Color DefaultBackground => IsProSkin ? DarkThemeColours.Background1 : LightThemeColours.Background1;
        public static Color ToolbarBackground => IsProSkin ? DarkThemeColours.ToolbarBackground : LightThemeColours.ToolbarBackground;
        public static Color WindowBackground => IsProSkin ? DarkThemeColours.Background2 : LightThemeColours.Background2;
        public static Color AlternatedRowsBackground => IsProSkin ? DarkThemeColours.ListItemAlt : LightThemeColours.ListItemAlt;
        
        
        // Button Colors
        public static Color ToolbarButtonBackground => IsProSkin ? DarkThemeColours.ButtonNormal : LightThemeColours.ButtonNormal;
        public static Color ToolbarButtonHoverBackground => IsProSkin ? DarkThemeColours.ButtonHover : LightThemeColours.ButtonHover;
        public static Color ToolbarButtonPressedBackground => IsProSkin ? DarkThemeColours.ButtonPressed : LightThemeColours.ButtonPressed;
        public static Color ToolbarButtonDisabledBackground => IsProSkin ? DarkThemeColours.ButtonDisabled : LightThemeColours.ButtonDisabled;
        
        // Text Colors
        public static Color PrimaryText => IsProSkin ? DarkThemeColours.Text1 : LightThemeColours.Text1;
        public static Color SecondaryText => IsProSkin ? DarkThemeColours.Text2 : LightThemeColours.Text2;
        public static Color DisabledText => IsProSkin ? DarkThemeColours.Text3 : LightThemeColours.Text3;
        public static Color ToolbarText => IsProSkin ? DarkThemeColours.ToolbarText : LightThemeColours.ToolbarText;
        
        // List Item Colors
        public static Color ListItemBackground => IsProSkin ? DarkThemeColours.ListItemNormal : LightThemeColours.ListItemNormal;
        public static Color ListItemHoverBackground => IsProSkin ? DarkThemeColours.ListItemHover : LightThemeColours.ListItemHover;
        public static Color ListItemSelectedBackground => IsProSkin ? DarkThemeColours.ListItemSelected : LightThemeColours.ListItemSelected;
        
        // Status Colors
        public static Color WarningColor => IsProSkin ? DarkThemeColours.Warning : LightThemeColours.Warning;
        public static Color ErrorColor => IsProSkin ? DarkThemeColours.Error : LightThemeColours.Error;
        public static Color SuccessColor => IsProSkin ? DarkThemeColours.Success : LightThemeColours.Success;
        
      
        /// <summary>
        /// Determines if editor is using the dark/pro skin
        /// </summary>
        private static bool IsProSkin => EditorGUIUtility.isProSkin;
        
        /// <summary>
        /// Converts a hex color string to a Unity Color
        /// </summary>
        /// <param name="hex">Hex color string (formats: #RRGGBB, #RRGGBBAA, RRGGBB, RRGGBBAA)</param>
        /// <returns>Unity Color</returns>
        public static Color HexToRgba(string hex) {
            if (ColorUtility.TryParseHtmlString(hex, out var color)) {
                return color;
            }
            Debug.LogWarning($"Failed to parse hex color: {hex}. Returning white.");
            return Color.white;
        }
        
        /// <summary>
        /// Converts a Unity Color to a hex string
        /// </summary>
        /// <param name="color">Unity Color</param>
        /// <param name="includeAlpha">Whether to include the alpha channel</param>
        /// <returns>Hex color string with # prefix</returns>
        public static string RgbaToHex(Color color, bool includeAlpha = false) {
            Color32 color32 = color;
            return includeAlpha
                ? $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}"
                : $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}";
        }
        
        /// <summary>
        /// Gets a color with adjusted brightness
        /// </summary>
        /// <param name="color">Base color</param>
        /// <param name="factor">Brightness factor (0-2, where 1 is original brightness)</param>
        /// <returns>Adjusted color</returns>
        public static Color AdjustBrightness(Color color, float factor) {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            v = Mathf.Clamp(v * factor, 0f, 1f);
            return Color.HSVToRGB(h, s, v);
        }
        
        /// <summary>
        /// Gets a color with adjusted alpha
        /// </summary>
        /// <param name="color">Base color</param>
        /// <param name="alpha">New alpha value (0-1)</param>
        /// <returns>Color with adjusted alpha</returns>
        public static Color WithAlpha(Color color, float alpha) {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}