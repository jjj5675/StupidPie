using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Phrases 0", menuName = "Dialogue")]
public class Phrases : ScriptableObject
{
    public string key;
    public List<Phrase> phrases = new List<Phrase>();
}

[Serializable]
public class Phrase
{
    public string name;
    public string portrait;
    public string value;

    public Phrase(string name, string portrait, string value)
    {
        this.name = name;
        this.portrait = portrait;
        this.value = value;
    }
}