using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InventoryUI))]
public class InventoryUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InventoryUI inventoryUI = (InventoryUI)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Inventory Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Open Inventory", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                inventoryUI.OpenShop();
            }
            else
            {
                Debug.LogWarning("Inventory can only be opened in Play Mode!");
            }
        }

        if (GUILayout.Button("Close Inventory", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                inventoryUI.CloseShop();
            }
            else
            {
                Debug.LogWarning("Inventory can only be closed in Play Mode!");
            }
        }

        EditorGUILayout.EndHorizontal();

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to test the inventory animations.", MessageType.Info);
        }
    }
}