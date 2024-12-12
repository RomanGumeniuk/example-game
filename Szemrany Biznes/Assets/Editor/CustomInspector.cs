using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapGeneratorSO))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapGeneratorSO mapGenerator = (MapGeneratorSO)target;
        if(GUILayout.Button("Generate map"))
        {
            mapGenerator.Generate();
        }
        /*if (GUILayout.Button("Clear lists"))
        {
            mapGenerator.ClearLists();
        }*/

    }
}
