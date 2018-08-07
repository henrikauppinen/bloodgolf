using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScatterTool: EditorWindow {
    private GUIStyle boldStyle;
    private GUIStyle warnStyle;

    private float yOffset = 0;
    private float maxGroundDist = 100;
    private Object scatterTemplate;
    private int scatterCount = 50;
    private GUIStyle groupBoxStyle;

    [MenuItem("ScatterTool/Open")]
    public static void OpenScatterTool()
    {
        GetWindow<ScatterTool>();
    }

    void OnEnable()
    {
        boldStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold
        };
        warnStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold
        };
        warnStyle.normal.textColor = new Color(0.8f, 0.3f, 0.0f);
        groupBoxStyle = new GUIStyle
        {
            padding = new RectOffset(10, 10, 10, 10),
        };
    }

    void OnSelectionChange()
    {
        Repaint();
    }

    void OnGUI()
    {
        GUILayout.Label("Warning: these tools come with no undo at present.", warnStyle);
        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
        {
            GUILayout.BeginHorizontal(groupBoxStyle);
            if (GUILayout.Button("Randomize All Rotation"))
            {
                ScatterUtils.RandomizeAllRotation(Selection.gameObjects);
            }
            if (GUILayout.Button("Randomize Y Rotation"))
            {
                ScatterUtils.RandomizeYRotation(Selection.gameObjects);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginVertical(groupBoxStyle);
        GUILayout.Label("Place On Ground", boldStyle);
        yOffset = EditorGUILayout.FloatField("Y Offset", yOffset);
        maxGroundDist = EditorGUILayout.FloatField("Max Distance", maxGroundDist);
        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
        {
            if (GUILayout.Button("Place Selected Items On Ground"))
            {
                ScatterUtils.PlaceObjectsOnGround(Selection.gameObjects, maxGroundDist: maxGroundDist, yOffset: yOffset);
            }
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical(groupBoxStyle);
        GUILayout.Label("Scatter", boldStyle);
        scatterTemplate = EditorGUILayout.ObjectField("Object", scatterTemplate, typeof(GameObject), true);
        scatterCount = EditorGUILayout.IntField("Count", scatterCount);
        using (new EditorGUI.DisabledScope(!(Selection.gameObjects.Length > 0 && scatterTemplate)))
        {
            if (GUILayout.Button("Scatter on Selected Object"))
            {
                var objs = ScatterUtils.ScatterOnObject(
                    targetObject: Selection.activeGameObject,
                    templateObject: scatterTemplate as GameObject,
                    count: scatterCount
                );
                Selection.objects = objs.ToArray();
            }
        }
        GUILayout.EndVertical();

    }

}
