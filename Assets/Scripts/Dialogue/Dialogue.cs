using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueOptionEffect
{
    None,
    GiveAHint,
    EndDialogue,
    ServeDish
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;

    public DialogueOptionEffect effect;
    public Customer customer;

    public int nextSentenceId;
}

[System.Serializable]
public class DialogueSentence
{
    public int id;
    [TextArea(3, 10)]
    public string sentence;
    public DialogueOption[] options;
    public int nextSentenceId = -1; // ID of the next sentence to display (for sentences without options)
}

[System.Serializable]
public class Dialogue
{
    public string name;
    public int startSentenceId; // ID of the first sentence to display
    public DialogueSentence[] sentences; // Keep for Unity serialization
    
    // Runtime dictionary for efficient lookups
    [System.NonSerialized]
    public Dictionary<int, DialogueSentence> sentenceMap;

    public Sprite customerImage;
    
    // Initialize the map from the array (call this after loading)
    public void InitializeMap()
    {
        sentenceMap = new Dictionary<int, DialogueSentence>();
        if (sentences != null)
        {
            foreach (var sentence in sentences)
            {
                sentenceMap[sentence.id] = sentence;
            }
        }
    }
}
