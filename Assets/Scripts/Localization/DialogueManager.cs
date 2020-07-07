using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance
    {
        get
        {
            if(s_Instance != null)
            {
                return s_Instance;
            }

            s_Instance = FindObjectOfType<DialogueManager>();

            if (s_Instance != null)
            {
                return s_Instance;
            }

            return Create();
        }
    }

    private static DialogueManager s_Instance;

    static DialogueManager Create()
    {
        GameObject gameObject = new GameObject("Localization");
        return s_Instance = gameObject.AddComponent<DialogueManager>();
    }

    public List<OriginalPhrases> phrases = new List<OriginalPhrases>();
    public string path;

    private void Awake()
    {
        CSVReader.Read(ref phrases, path);

        for(int i=0; i<phrases[0].phrases.Count; i++)
        {
            print(phrases[0].phrases[i]);
        }
    }

}
