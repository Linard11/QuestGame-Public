using UnityEditor;

 using UnityEngine;

 [CustomPropertyDrawer(typeof(State))]
 public class StateDrawer : PropertyDrawer
 {
     private const float Ratio = 0.8f;
     private const int HorizontalPadding = 4;
     private const int VerticalPadding = 2;

     private static readonly GUIStyle GhostStyle;

     static StateDrawer()
     {
         GhostStyle = new GUIStyle(EditorStyles.miniLabel);
         GhostStyle.padding.left = 4;
         GhostStyle.normal.textColor = Color.grey;
     }

     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
     {
         using (new EditorGUI.PropertyScope(position, label, property))
         {
             // Only draw label if outside of array
             if (!property.propertyPath.EndsWith(']'))
             {
                 position = EditorGUI.PrefixLabel(position, label);
             }

             var idProperty = property.FindPropertyRelative(nameof(State.id));
             var amountProperty = property.FindPropertyRelative(nameof(State.amount));

             position.width -= HorizontalPadding;
             position.height -= VerticalPadding;

             var idRect = position;
             idRect.width *= Ratio;
             var amountRect = position;
             amountRect.width *= (1 - Ratio);
             amountRect.x += idRect.width + HorizontalPadding;

             bool emptyId = string.IsNullOrWhiteSpace(idProperty.stringValue);

             if (emptyId)
             {
                 GUI.color = Color.red;
             }
             EditorGUI.PropertyField(idRect, idProperty, GUIContent.none);
             if (emptyId)
             {
                 GUI.color = Color.white;
                 EditorGUI.LabelField(idRect, $"<{nameof(State.id).ToUpper()}>", GhostStyle);
             }

             EditorGUI.PropertyField(amountRect, amountProperty, GUIContent.none);
         }
     }

     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
     {
         return base.GetPropertyHeight(property, label) + VerticalPadding;
     }
 }