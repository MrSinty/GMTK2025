using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogueOptionReciever
{
    void OnDialogueOptionChosen(DialogueOption option);
}
