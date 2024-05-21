using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO; // Add this using directive for file operations

[CustomEditor(typeof(AchievementDatabase))]
public class AchievementEditor : Editor
{
    private AchievementDatabase database;

    private void OnEnable()
    {
        database = target as AchievementDatabase;
    }

    public override void OnInspectorGUI() // Correct the method name
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Enum", GUILayout.Height(30)))
        {
            GenerateEnum();
        }
    }

    private void GenerateEnum()
    {
        string filePath = Path.Combine(Application.dataPath, "Achievements.cs");
        string code = "public enum Achievements {\n";
        foreach (Achievement achievement in database.achievements) // Assuming Achievement is the correct type
        {
            code += $"    {achievement.id},\n"; // Properly format the code with indentation and newlines
        }
        code += "}\n";
        File.WriteAllText(filePath, code);
        AssetDatabase.ImportAsset("Assets/Achievements.cs"); // Correct the method name
        AssetDatabase.Refresh(); // Refresh the AssetDatabase to ensure the new script is recognized
    }
}
