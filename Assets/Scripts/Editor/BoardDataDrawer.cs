using NUnit.Framework.Internal;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BoardData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class BoardDataDrawer : Editor {

    private BoardData GameDataInstance => target as BoardData;
    private ReorderableList _datalist;
    private void OnEnable() {
        InitializeReordableList(ref _datalist, "SearchWords", "Searching Words");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        DrawColumnsRowsInputFields();
        EditorGUILayout.Space();
        ConvertToUpperButton();
        GUILayout.BeginHorizontal();
        ClearBoardButton();
        FillUpWithRandomLettersButton();
        GUILayout.EndHorizontal();

        if (GameDataInstance.Board != null && GameDataInstance.Columns > 0 && GameDataInstance.Rows > 0)
            DrawBoardTable();

        EditorGUILayout.Space();
        _datalist.DoLayoutList();
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

    private void InitializeReordableList(ref ReorderableList list, string propertyName, string listLabel) {
        list = new ReorderableList(serializedObject, elements: serializedObject.FindProperty(propertyName),
        draggable: true, displayHeader: true, displayAddButton: true, displayRemoveButton: true);
        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, listLabel);
        };
        var l = list;
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
            position: new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("Word"), GUIContent.none);

        };
    }

    private void ConvertToUpperButton() {
        if (GUILayout.Button("To Upper")) {

            for (var i = 0; i < GameDataInstance.Columns; i++) {

                for (var j = 0; j < GameDataInstance.Rows; j++) {
                    var errorCounter = Regex.Matches(GameDataInstance.Board[i].Row[j], pattern: @"[a-z]").Count;

                    if (errorCounter > 0)
                        GameDataInstance.Board[i].Row[j] = GameDataInstance.Board[i].Row[j].ToUpper();
                }
            }
            foreach (var searchWord in GameDataInstance.SearchWords) {
                var errorCounter = Regex.Matches(input: searchWord.Word, pattern: @"[a-z]").Count;
                if (errorCounter > 0)
                    searchWord.Word = searchWord.Word.ToUpper();
            }

        }
    }

    private void ClearBoardButton() {
        if (GUILayout.Button("Clear Board")) {
            for (int i = 0; i < GameDataInstance.Columns; i++) {
                for (int j = 0; j < GameDataInstance.Rows; j++) {
                    GameDataInstance.Board[i].Row[j] = "";
                }
            }

        }
    }
    private void FillUpWithRandomLettersButton() {
        if (GUILayout.Button(text: "Fill Up With Random"))
            for (int i = 0; i < GameDataInstance.Columns; i++) {
                for (int j = 0; j < GameDataInstance.Rows; j++) {
                    int errorCounter = Regex.Matches(GameDataInstance.Board[i].Row[j], pattern: @"[a-zA-Z]").Count;
                    string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    int index = Random.Range(0, letters.Length);
                    if (errorCounter == 0) {
                        GameDataInstance.Board[i].Row[j] = letters[index].ToString();
                    }
                }
            }

    }
}

