using Entity.Components.DashMonsterComponents;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(DashEffect))]
    public class DashEffectEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            if (!Application.isPlaying)
            {
                DashEffect dashEffect = (DashEffect)target;
            
                if (dashEffect.transform.hasChanged)
                {
                    Undo.RecordObject(dashEffect, "Update Origin Position");
                    SerializedObject serializedObject = new SerializedObject(dashEffect);
                    SerializedProperty originPos = serializedObject.FindProperty("originPos");
                    originPos.vector3Value = dashEffect.transform.position;
                    serializedObject.ApplyModifiedProperties();
                    dashEffect.transform.hasChanged = false;
                }
            }
        }
    }
}