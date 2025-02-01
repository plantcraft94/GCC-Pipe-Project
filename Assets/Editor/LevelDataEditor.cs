using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelData levelData = (LevelData)target;
        levelData.Row = EditorGUILayout.IntField("Row", levelData.Row);
        levelData.Column = EditorGUILayout.IntField("Column", levelData.Column);
        if (GUILayout.Button("Generate Data"))
        {
            GenerateLevelData(levelData);
        }
        if (GUILayout.Button("Clear Data"))
        {
            levelData.Data.Clear();
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(levelData);
        }
        base.OnInspectorGUI();
    }

    private void GenerateLevelData(LevelData levelData)
    {
        levelData.Data.Clear();
        int totalCells = levelData.Row * levelData.Column;

        levelData.Data.Add(11);
        for (int i = 1; i < totalCells-1; i++)
        {
            int rotation = Random.Range(0, 4);
            int pipeType = Random.Range(3, 6);
            int pipeData = rotation * 10 + pipeType;
            levelData.Data.Add(pipeData);
        }
        levelData.Data.Add(32);
        EditorUtility.SetDirty(levelData);
    }
    private void CheckLevel(LevelData levelData)
    {
        for (int i = 0; i < levelData.Row; i++)
        {
            for(int j = 0; j < levelData.Column; j++)
            {

            }
        }
    }
}
