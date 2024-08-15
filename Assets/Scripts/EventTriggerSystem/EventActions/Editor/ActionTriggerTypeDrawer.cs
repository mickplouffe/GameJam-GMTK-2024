using System.Collections.Generic;
using EventTriggerSystem.EventActions;
using UnityEditor;
using UnityEngine;

namespace EventTriggerSystem.EventActions.Editor
{
    [CustomPropertyDrawer(typeof(EventActionType))]
    public class ActionTriggerTypeDrawer : PropertyDrawer
    {
        private static readonly Dictionary<int, bool> FoldoutStates = new Dictionary<int, bool>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
        
            SerializedProperty isTriggeredProp = property.FindPropertyRelative("isTriggered");
            SerializedProperty triggerProp = property.FindPropertyRelative("trigger");
            SerializedProperty triggerScriptProp = property.FindPropertyRelative("triggerScript");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;
            float buttonWidth = 60f;

            int propertyHash = property.propertyPath.GetHashCode();
            if (!FoldoutStates.ContainsKey(propertyHash))
            {
                FoldoutStates[propertyHash] = false;
            }

            // Draw fields
            Rect isTriggeredRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
            EditorGUI.PropertyField(isTriggeredRect, isTriggeredProp);

            Rect triggerRect = new Rect(position.x, position.y + 2 * (lineHeight + spacing), position.width, lineHeight);
            EditorGUI.PropertyField(triggerRect, triggerProp);

            Rect triggerScriptRect = new Rect(position.x, position.y + 3 * (lineHeight + spacing), position.width - buttonWidth - spacing, lineHeight);
            Rect buttonRect = new Rect(position.x + position.width - buttonWidth, position.y + 3 * (lineHeight + spacing), buttonWidth, lineHeight);

            EditorGUI.ObjectField(triggerScriptRect, triggerScriptProp, GUIContent.none);
            if (GUI.Button(buttonRect, "New"))
            {
                CreateNewTriggerScript(triggerProp.enumValueIndex, triggerScriptProp);
            }

            // Handle foldout
            if (triggerScriptProp.objectReferenceValue != null)
            {
                FoldoutStates[propertyHash] = EditorGUI.Foldout(new Rect(position.x, position.y + 4 * (lineHeight + spacing), position.width, lineHeight), FoldoutStates[propertyHash], "Settings");

                if (FoldoutStates[propertyHash])
                {
                    EditorGUI.indentLevel++;
                    SerializedObject serializedObject = new SerializedObject(triggerScriptProp.objectReferenceValue);
                    SerializedProperty prop = serializedObject.GetIterator();
                    float yOffset = 5 * (lineHeight + spacing);
                    while (prop.NextVisible(true))
                    {
                        if (prop.name == "m_Script") continue;
                        float propertyHeight = EditorGUI.GetPropertyHeight(prop, true);
                        Rect propertyRect = new Rect(position.x, position.y + yOffset, position.width, propertyHeight);
                        EditorGUI.PropertyField(propertyRect, prop, true);
                        yOffset += propertyHeight + spacing;
                    }
                    serializedObject.ApplyModifiedProperties();
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;

            SerializedProperty triggerScriptProp = property.FindPropertyRelative("triggerScript");

            int propertyHash = property.propertyPath.GetHashCode();
            if (!FoldoutStates.ContainsKey(propertyHash))
            {
                FoldoutStates[propertyHash] = false;
            }

            float totalHeight = 4 * (lineHeight + spacing);

            if (triggerScriptProp.objectReferenceValue != null)
            {
                totalHeight += lineHeight + spacing; // Add space for the foldout
                if (FoldoutStates[propertyHash])
                {
                    SerializedObject serializedObject = new SerializedObject(triggerScriptProp.objectReferenceValue);
                    SerializedProperty prop = serializedObject.GetIterator();
                    while (prop.NextVisible(true))
                    {
                        if (prop.name == "m_Script") continue;
                        totalHeight += EditorGUI.GetPropertyHeight(prop, true) + spacing;
                    }
                }
            }

            return totalHeight;
        }

        private void CreateNewTriggerScript(int triggerIndex, SerializedProperty triggerScriptProp)
        {
            EventActionSettings newInstance = null;

            switch ((EventActionTypes)triggerIndex)
            {
                case EventActionTypes.PlaySoundClip:
                    newInstance = ScriptableObject.CreateInstance<AtPlaySoundClip>();
                    break;
                case EventActionTypes.PlayAnimation:
                    newInstance = ScriptableObject.CreateInstance<AtPlayAnimation>();
                    break;
            }

            if (newInstance)
            {
                string path = $"Assets/ActionTriggerSo/{((EventActionTypes)triggerIndex).ToString()}TriggerSettings.asset";
                AssetDatabase.CreateAsset(newInstance, AssetDatabase.GenerateUniqueAssetPath(path));
                AssetDatabase.SaveAssets();

                triggerScriptProp.objectReferenceValue = newInstance;
                triggerScriptProp.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
