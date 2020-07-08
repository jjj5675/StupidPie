using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DialogueManager dialogueManager = target as DialogueManager;

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Read File", GUILayout.Width(150), GUILayout.Height(30)))
        {
            if (!CSVReader.Read(ref dialogueManager.phrases, dialogueManager.textFilePath))
            {
                Debug.LogError("텍스트 읽어오기에 실패했습니다.");
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

}
