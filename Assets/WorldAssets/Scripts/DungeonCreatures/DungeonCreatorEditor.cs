using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(DungeonCreator))]
public class DungeonCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DungeonCreator dungeonCreator = (DungeonCreator)target;
        if (GUILayout.Button("Create Dungeon"))
        {
            dungeonCreator.CreateDungeon();
        }
    }
}
