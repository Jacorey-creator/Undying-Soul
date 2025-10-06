using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(DungeonCreator))]
public class DungeonCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DungeonCreator dungeonCreator = (DungeonCreator)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dungeon Controls", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Create Dungeon"))
        {
            dungeonCreator.CreateDungeon();
        }
        
        /*EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawning Controls", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Clear All Spawned Items"))
        {
            var enemySpawners = dungeonCreator.GetComponents<EnemySpawner>();
            var objectSpawners = dungeonCreator.GetComponents<ObjectSpawner>();
            
            foreach (var spawner in enemySpawners)
            {
                if (spawner != null) spawner.ClearEnemies();
            }
            foreach (var spawner in objectSpawners)
            {
                if (spawner != null) spawner.ClearObjects();
            }
        }
        
        if (GUILayout.Button("Clear Enemies Only"))
        {
            var enemySpawners = dungeonCreator.GetComponents<EnemySpawner>();
            foreach (var spawner in enemySpawners)
            {
                if (spawner != null) spawner.ClearEnemies();
            }
        }
        
        if (GUILayout.Button("Clear Objects Only"))
        {
            var objectSpawners = dungeonCreator.GetComponents<ObjectSpawner>();
            foreach (var spawner in objectSpawners)
            {
                if (spawner != null) spawner.ClearObjects();
            }
        }*/
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawner Management", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Add Enemy Spawner"))
        {
            dungeonCreator.gameObject.AddComponent<EnemySpawner>();
        }
        
        if (GUILayout.Button("Add Object Spawner"))
        {
            dungeonCreator.gameObject.AddComponent<ObjectSpawner>();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Toggle Corridor Debug"))
        {
            // This will be handled by the debug logging in CorridorsGenerator
            Debug.Log("Corridor debug logging is enabled. Check console for corridor generation details.");
        }
    }
}
