using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OriginalPhrases : ScriptableObject
{
    public string key;
    public List<object> phrases = new List<object>();

    public List<object> this[string key]
    {
        get
        {
            if(this.key == key)
            {
                return phrases;
            }

            return null;
        }
    }
}
