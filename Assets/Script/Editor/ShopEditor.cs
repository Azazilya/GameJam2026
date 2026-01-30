using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShopUI))]
public class ShopUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShopUI shopUI = (ShopUI)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Shop Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Open Shop", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                shopUI.OpenShop();
            }
            else
            {
                Debug.LogWarning("Shop can only be opened in Play Mode!");
            }
        }

        if (GUILayout.Button("Close Shop", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                shopUI.CloseShop();
            }
            else
            {
                Debug.LogWarning("Shop can only be closed in Play Mode!");
            }
        }

        EditorGUILayout.EndHorizontal();

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to test the shop animations.", MessageType.Info);
        }
    }
}