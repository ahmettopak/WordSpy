using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BoardData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class BoardDataDrawer : Editor {

    private BoardData GameDataInstance => target as BoardData;
    private ReorderableList datalist;
    private void OnEnable() {

    }
    public override void OnInspectorGUI() {
        serializedObject.Update();
        DrawColumnsRowsInputFields();
        EditorGUILayout.Space();
        if (GameDataInstance.Board != null && GameDataInstance.Columns > 0 && GameDataInstance.Rows > 0)
            DrawBoardTable();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(GameDataInstance);
    }
    private void DrawColumnsRowsInputFields() {

        var columnsTemp = GameDataInstance.Columns;
        var rowsTemp = GameDataInstance.Rows;
        GameDataInstance.Columns = EditorGUILayout.IntField(label: "Columns", GameDataInstance.Columns);
        GameDataInstance.Rows = EditorGUILayout.IntField(label: "Rows", GameDataInstance.Rows);


        if ((GameDataInstance.Columns != columnsTemp || GameDataInstance.Rows != rowsTemp)
                && GameDataInstance.Columns > 0 && GameDataInstance.Rows > 0)
            GameDataInstance.CreateNewBoard();
    }

    private void DrawBoardTable() {
        var tableStyle = new GUIStyle(other: "box");
        tableStyle.padding = new RectOffset(left: 10, right: 10, top: 10, bottom: 10);
        tableStyle.margin.left = 32;
        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 35;
        var columnStyle = new GUIStyle();
        columnStyle.fixedWidth = 50;
        var rowstyle = new GUIStyle();
        rowstyle.fixedHeight = 25;
        rowstyle.fixedWidth = 40;
        rowstyle.alignment = TextAnchor.MiddleCenter;
        var textFieldStyle = new GUIStyle();
        textFieldStyle.normal.background = Texture2D.grayTexture;
        textFieldStyle.normal.textColor = Color.white;
        textFieldStyle.fontStyle = FontStyle.Bold;
        textFieldStyle.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.BeginHorizontal(tableStyle);



        for (var x = 0; x < GameDataInstance.Columns; x++) {
            EditorGUILayout.BeginVertical(x == -1 ? headerColumnStyle : columnStyle);
            for (var y = 0; y < GameDataInstance.Rows; y++) {
                if (x >= 0 && y >= 0) {
                    EditorGUILayout.BeginHorizontal(rowstyle);
                    var character = (string)EditorGUILayout.TextArea(GameDataInstance.Board[x].Row[y], textFieldStyle);
                    if (GameDataInstance.Board[x].Row[y].Length > 1) {
                        character = GameDataInstance.Board[x].Row[y].Substring(startIndex: 0, length: 1);
                    }
                    GameDataInstance.Board[x].Row[y] = character;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndHorizontal();
    }
}

