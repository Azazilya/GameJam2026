using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CurrencySystem))]
public class CurrencySystemEditor : Editor
{
    private int amountToAdd = 100;
    private int amountToRemove = 50;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CurrencySystem currencySystem = (CurrencySystem)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Currency Controls", EditorStyles.boldLabel);

        // Add Money Section
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Amount to Add:", GUILayout.Width(100));
        amountToAdd = EditorGUILayout.IntField(amountToAdd);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button($"Add {amountToAdd} Money", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                currencySystem.AddMoney(amountToAdd);
            }
            else
            {
                Debug.LogWarning("Currency can only be modified in Play Mode!");
            }
        }

        EditorGUILayout.Space(5);

        // Remove Money Section
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Amount to Remove:", GUILayout.Width(100));
        amountToRemove = EditorGUILayout.IntField(amountToRemove);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button($"Remove {amountToRemove} Money", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                currencySystem.RemoveMoney(amountToRemove);
            }
            else
            {
                Debug.LogWarning("Currency can only be modified in Play Mode!");
            }
        }

        EditorGUILayout.Space(5);

        // Quick Action Buttons
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+10", GUILayout.Height(25)))
        {
            if (Application.isPlaying) currencySystem.AddMoney(10);
            else Debug.LogWarning("Currency can only be modified in Play Mode!");
        }

        if (GUILayout.Button("+100", GUILayout.Height(25)))
        {
            if (Application.isPlaying) currencySystem.AddMoney(100);
            else Debug.LogWarning("Currency can only be modified in Play Mode!");
        }

        if (GUILayout.Button("+1000", GUILayout.Height(25)))
        {
            if (Application.isPlaying) currencySystem.AddMoney(1000);
            else Debug.LogWarning("Currency can only be modified in Play Mode!");
        }

        EditorGUILayout.EndHorizontal();

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to test currency operations.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox($"Current Money: {currencySystem.currentMoney}", MessageType.None);
        }
    }
}