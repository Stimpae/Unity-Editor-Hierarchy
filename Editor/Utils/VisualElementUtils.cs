using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementUtils {
    public struct ElementLength4 {
        public StyleLength bottomFloat;
        public StyleLength rightFloat;
        public StyleLength topFloat;
        public StyleLength leftFloat;
        
        public ElementLength4(float bottomFloat, float rightFloat, float topFloat, float leftFloat) {
            this.bottomFloat = new StyleLength(bottomFloat);
            this.rightFloat = new StyleLength(rightFloat);
            this.topFloat = new StyleLength(topFloat);
            this.leftFloat = new StyleLength(leftFloat);
        }
    }
    
    public struct ElementFloat4 {
        public StyleFloat bottomFloat;
        public StyleFloat rightFloat;
        public StyleFloat topFloat;
        public StyleFloat leftFloat;
        
        public ElementFloat4(float bottomFloat, float rightFloat, float topFloat, float leftFloat) {
            this.bottomFloat = new StyleFloat(bottomFloat);
            this.rightFloat = new StyleFloat(rightFloat);
            this.topFloat = new StyleFloat(topFloat);
            this.leftFloat = new StyleFloat(leftFloat);
        }
    }
    
    public static void SetBorderColor(this VisualElement element, Color color) {
        element.style.borderBottomColor = color;
        element.style.borderRightColor = color;
        element.style.borderTopColor = color;
        element.style.borderLeftColor = color;
    }
    
    public static void SetPadding(this VisualElement element, ElementLength4 elementLength) {
        element.style.paddingBottom = elementLength.bottomFloat;
        element.style.paddingRight = elementLength.rightFloat;
        element.style.paddingTop = elementLength.topFloat;
        element.style.paddingLeft = elementLength.leftFloat;
    }
    
    public static void SetMargin(this VisualElement element, ElementLength4 elementLength) {
        element.style.marginBottom = elementLength.bottomFloat;
        element.style.marginRight = elementLength.rightFloat;
        element.style.marginTop = elementLength.topFloat;
        element.style.marginLeft = elementLength.leftFloat;
    }
    
    public static void SetBorderWidth(this VisualElement element, ElementFloat4 elementFloat) {
        element.style.borderBottomWidth = elementFloat.bottomFloat;
        element.style.borderRightWidth = elementFloat.rightFloat;
        element.style.borderTopWidth = elementFloat.topFloat;
        element.style.borderLeftWidth = elementFloat.leftFloat;
    }
    
    public static void SetBorderRadius(this VisualElement element, ElementLength4 elementLength) {
        element.style.borderBottomRightRadius = elementLength.bottomFloat;
        element.style.borderBottomLeftRadius = elementLength.rightFloat;
        element.style.borderTopRightRadius = elementLength.topFloat;
        element.style.borderTopLeftRadius = elementLength.leftFloat;
    }
    
    
}
