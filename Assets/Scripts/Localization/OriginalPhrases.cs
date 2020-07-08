using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OriginalPhrases : ScriptableObject
{
    public string key;
    public List<Phrase> phrases = new List<Phrase>();
}

[Serializable]
public class Phrase
{
    [HideInInspector]
    public string name;
    public string value;

    public Phrase(string name, string value)
    {
        this.name = name;
        this.value = value;
    }
}