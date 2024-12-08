using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapGenerator))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapGenerator mapGenerator = (MapGenerator)target;
        if(GUILayout.Button("Generate map"))
        {
            mapGenerator.Generate();
        }
    }
}
