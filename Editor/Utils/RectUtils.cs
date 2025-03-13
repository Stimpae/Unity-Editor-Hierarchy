using UnityEngine;
using UnityEditor;
using System;

namespace Hierarchy.Utils {
    /// <summary>
    /// Static utility class for Rect transformations with a fluent API, similar to CSS/USS layout manipulation.
    /// </summary>
    public static class RectStyle
    {
        // Positioning
        public static Rect SetPosition(this Rect rect, float x, float y) { rect.x = x; rect.y = y; return rect; }
        public static Rect SetPosition(this Rect rect, Vector2 position) => rect.SetPosition(position.x, position.y);

        public static Rect SetX(this Rect rect, float x) => rect.SetPosition(x, rect.y);
        public static Rect SetY(this Rect rect, float y) => rect.SetPosition(rect.x, y);

        public static Rect SetAnchor(this Rect rect, float x, float y) => rect.SetPosition(x - rect.width / 2, y - rect.height / 2);
        public static Rect SetAnchor(this Rect rect, Vector2 anchor) => rect.SetAnchor(anchor.x, anchor.y);

        // Movement
        public static Rect Translate(this Rect rect, float dx, float dy) { rect.x += dx; rect.y += dy; return rect; }
        public static Rect Translate(this Rect rect, Vector2 delta) => rect.Translate(delta.x, delta.y);
        
        public static Rect MoveX(this Rect rect, float dx) => rect.Translate(dx, 0);
        public static Rect MoveY(this Rect rect, float dy) => rect.Translate(0, dy);

        // Sizing
        public static Rect SetSize(this Rect rect, float width, float height) { rect.width = width; rect.height = height; return rect; }
        public static Rect SetSize(this Rect rect, Vector2 size) => rect.SetSize(size.x, size.y);
        public static Rect SetSize(this Rect rect, float size) => rect.SetSize(size, size);

        public static Rect SetWidth(this Rect rect, float width) { rect.width = width; return rect; }
        public static Rect SetHeight(this Rect rect, float height) { rect.height = height; return rect; }

        public static Rect Expand(this Rect rect, float padding) => rect.Inset(-padding);
        public static Rect Expand(this Rect rect, float horizontal, float vertical) => rect.Inset(-horizontal, -vertical);
        public static Rect Expand(this Rect rect, float left, float right, float top, float bottom) => rect.Inset(-left, -right, -top, -bottom);

        public static Rect Inset(this Rect rect, float inset) => rect.Inset(inset, inset, inset, inset);
        public static Rect Inset(this Rect rect, float horizontal, float vertical) => rect.Inset(horizontal, horizontal, vertical, vertical);
        public static Rect Inset(this Rect rect, float left, float right, float top, float bottom)
        {
            rect.x += left;
            rect.y += top;
            rect.width -= (left + right);
            rect.height -= (top + bottom);
            return rect;
        }

        // Scaling
        public static Rect Scale(this Rect rect, float factor) => rect.Scale(factor, factor);
        public static Rect Scale(this Rect rect, float scaleX, float scaleY)
        {
            rect.width *= scaleX;
            rect.height *= scaleY;
            return rect;
        }

        public static Rect ScaleFromCenter(this Rect rect, float factor)
        {
            var center = rect.center;
            rect = rect.Scale(factor);
            rect.center = center;
            return rect;
        }

        // Alignment
        public static Rect AlignTo(this Rect rect, Rect parent, TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft: return rect.SetPosition(parent.x, parent.y);
                case TextAnchor.UpperCenter: return rect.SetPosition(parent.center.x - rect.width / 2, parent.y);
                case TextAnchor.UpperRight: return rect.SetPosition(parent.xMax - rect.width, parent.y);
                case TextAnchor.MiddleLeft: return rect.SetPosition(parent.x, parent.center.y - rect.height / 2);
                case TextAnchor.MiddleCenter: return rect.SetPosition(parent.center.x - rect.width / 2, parent.center.y - rect.height / 2);
                case TextAnchor.MiddleRight: return rect.SetPosition(parent.xMax - rect.width, parent.center.y - rect.height / 2);
                case TextAnchor.LowerLeft: return rect.SetPosition(parent.x, parent.yMax - rect.height);
                case TextAnchor.LowerCenter: return rect.SetPosition(parent.center.x - rect.width / 2, parent.yMax - rect.height);
                case TextAnchor.LowerRight: return rect.SetPosition(parent.xMax - rect.width, parent.yMax - rect.height);
                default: return rect;
            }
        }

        // Grid Alignment
        public static Rect SnapToGrid(this Rect rect, float gridSize)
        {
            return new Rect(
                Mathf.Floor(rect.x / gridSize) * gridSize,
                Mathf.Floor(rect.y / gridSize) * gridSize,
                Mathf.Ceil(rect.width / gridSize) * gridSize,
                Mathf.Ceil(rect.height / gridSize) * gridSize
            );
        }

        // Pixel alignment
        public static Rect AlignToPixel(this Rect rect) => GUIUtility.AlignRectToDevice(rect);
        public static Rect Round(this Rect rect)
        {
            return new Rect(
                Mathf.Round(rect.x),
                Mathf.Round(rect.y),
                Mathf.Round(rect.width),
                Mathf.Round(rect.height)
            );
        }

        // Splitting
        public static Rect[] SplitHorizontal(this Rect rect, params float[] ratios)
        {
            if (ratios == null || ratios.Length == 0) return new Rect[] { rect };

            float totalRatio = 0;
            foreach (var r in ratios) totalRatio += r;

            Rect[] result = new Rect[ratios.Length];
            float currentX = rect.x;

            for (int i = 0; i < ratios.Length; i++)
            {
                float width = rect.width * (ratios[i] / totalRatio);
                result[i] = new Rect(currentX, rect.y, width, rect.height);
                currentX += width;
            }

            return result;
        }

        public static Rect[] SplitVertical(this Rect rect, params float[] ratios)
        {
            if (ratios == null || ratios.Length == 0) return new Rect[] { rect };

            float totalRatio = 0;
            foreach (var r in ratios) totalRatio += r;

            Rect[] result = new Rect[ratios.Length];
            float currentY = rect.y;

            for (int i = 0; i < ratios.Length; i++)
            {
                float height = rect.height * (ratios[i] / totalRatio);
                result[i] = new Rect(rect.x, currentY, rect.width, height);
                currentY += height;
            }

            return result;
        }
    }
}
