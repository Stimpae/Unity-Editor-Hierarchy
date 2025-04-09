using UnityEngine;
using UnityEditor;
using System;

namespace Hierarchy.Utils {
    /// <summary>
    /// Static utility class for Rect transformations with a fluent API, similar to CSS/USS layout manipulation.
    /// </summary>
    public static class RectUtils
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
    }
}
