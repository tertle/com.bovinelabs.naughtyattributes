namespace BovineLabs.NaughtyAttributes.Editor.PropertyDrawers
{
    using System.Globalization;
    using BovineLabs.NaughtyAttributes;
    using BovineLabs.NaughtyAttributes.Editor.Attributes;
    using BovineLabs.NaughtyAttributes.Editor.Utility;
    using BovineLabs.NaughtyAttributes.Editor.Wrappers;
    using UnityEditor;
    using UnityEngine;

    [PropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarPropertyDrawer : PropertyDrawer<ProgressBarAttribute>
    {
        /// <inheritdoc />
        protected override void DrawProperty(NonSerializedAttributeWrapper wrapper, ProgressBarAttribute attribute)
        {
            var type = wrapper.Type;

            if (type != typeof(float) && type != typeof(int))
            {
                NotIntFloat(wrapper);
                return;
            }

            var value = (float)wrapper.GetValue();
            this.DrawProgressBar(value, type == typeof(int), attribute);
        }

        /// <inheritdoc />
        protected override void DrawProperty(SerializedPropertyAttributeWrapper wrapper, ProgressBarAttribute attribute)
        {
            var property = wrapper.Property;

            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                NotIntFloat(wrapper);
                return;
            }

            var value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
            this.DrawProgressBar(value, property.propertyType == SerializedPropertyType.Integer, attribute);
        }

        private void DrawProgressBar(float value, bool isInteger, ProgressBarAttribute attribute)
        {
            var valueFormatted = isInteger ? value.ToString(CultureInfo.InvariantCulture) : $"{value:0.00}";

            var position = EditorGUILayout.GetControlRect();
            var maxValue = attribute.MaxValue;
            float lineHight = EditorGUIUtility.singleLineHeight;
            var barPosition = new Rect(position.position.x, position.position.y, position.size.x, lineHight);

            var fillPercentage = value / maxValue;
            var barLabel = (!string.IsNullOrEmpty(attribute.Name) ? "[" + attribute.Name + "] " : "") + valueFormatted + "/" + maxValue;

            var color = GetColor(attribute.Color);
            var color2 = Color.white;
            DrawBar(barPosition, Mathf.Clamp01(fillPercentage), barLabel, color, color2);
        }

        private static void DrawBar(Rect position, float fillPercent, string label, Color barColor, Color labelColor)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            var fillRect = new Rect(position.x, position.y, position.width * fillPercent, position.height);

            EditorGUI.DrawRect(position, new Color(0.13f, 0.13f, 0.13f));
            EditorGUI.DrawRect(fillRect, barColor);

            // set alignment and cache the default
            var align = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.UpperCenter;

            // set the color and cache the default
            var c = GUI.contentColor;
            GUI.contentColor = labelColor;

            // calculate the position
            var labelRect = new Rect(position.x, position.y - 2, position.width, position.height);

            // draw~
            EditorGUI.DropShadowLabel(labelRect, label);

            // reset color and alignment
            GUI.contentColor = c;
            GUI.skin.label.alignment = align;
        }

        private static Color GetColor(ProgressBarColor color)
        {
            switch (color)
            {
                case ProgressBarColor.Red:
                    return new Color32(255, 0, 63, 255);
                case ProgressBarColor.Pink:
                    return new Color32(255, 152, 203, 255);
                case ProgressBarColor.Orange:
                    return new Color32(255, 128, 0, 255);
                case ProgressBarColor.Yellow:
                    return new Color32(255, 211, 0, 255);
                case ProgressBarColor.Green:
                    return new Color32(102, 255, 0, 255);
                case ProgressBarColor.Blue:
                    return new Color32(0, 135, 189, 255);
                case ProgressBarColor.Indigo:
                    return new Color32(75, 0, 130, 255);
                case ProgressBarColor.Violet:
                    return new Color32(127, 0, 255, 255);
                default:
                    return Color.white;
            }
        }

        private static void NotIntFloat(ValueWrapper wrapper)
        {
            string warning = $"{typeof(ProgressBarAttribute).Name} can only be used on int or float fields";
            EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning);
            wrapper.DrawDefaultField();
        }
    }
}
