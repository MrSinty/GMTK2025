using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject optionsPanel;
    public Button optionButtonPrefab;
    public Button continueButton;

    private Dialogue currentDialogue;
    private DialogueSentence currentSentence;

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting conversation with " + dialogue.name);
        nameText.text = dialogue.name;
        currentDialogue = dialogue;
        
        // Initialize the sentence map for efficient lookups
        dialogue.InitializeMap();
        
        // Start with the designated start sentence
        if (dialogue.sentenceMap != null && dialogue.sentenceMap.ContainsKey(dialogue.startSentenceId))
        {
            currentSentence = dialogue.sentenceMap[dialogue.startSentenceId];
            DisplayCurrentSentence();
        }
        else
        {
            Debug.LogError("Start sentence with ID " + dialogue.startSentenceId + " not found!");
            EndDialogue();
        }
    }

    void DisplayCurrentSentence()
    {
        // Clear previous options
        foreach (Transform child in optionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        optionsPanel.SetActive(false);

        dialogueText.text = currentSentence.sentence;

        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinueButtonClicked);
    }

    void OnContinueButtonClicked()
    {
        continueButton.gameObject.SetActive(false);

        if (currentSentence.options != null && currentSentence.options.Length > 0)
        {
            optionsPanel.SetActive(true);
            foreach (var option in currentSentence.options)
            {
                var button = Instantiate(optionButtonPrefab, optionsPanel.transform);
                button.GetComponentInChildren<TMP_Text>().text = option.optionText;
                button.onClick.AddListener(() => OnOptionSelected(option));
            }
        }
        else if (currentSentence.nextSentenceId != -1)
        {
            // Progress to the next sentence in sequence
            if (currentDialogue.sentenceMap.ContainsKey(currentSentence.nextSentenceId))
            {
                currentSentence = currentDialogue.sentenceMap[currentSentence.nextSentenceId];
                DisplayCurrentSentence();
            }
            else
            {
                Debug.LogWarning("Next sentence with ID " + currentSentence.nextSentenceId + " not found. Ending dialogue.");
                EndDialogue();
            }
        }
        else
        {
            // No options and no next sentence, end the dialogue
            EndDialogue();
        }
    }

    public void AdvanceToNextSentence()
    {
        // This method is no longer needed as we are using a dictionary for progression
        // The logic for advancing to the next sentence based on options is now handled in OnOptionSelected
        // For now, we'll just end the dialogue if there are no options and no next sentence
        if (currentSentence.options == null || currentSentence.options.Length == 0)
        {
            EndDialogue();
        }
    }

    void OnOptionSelected(DialogueOption option)
    {
        if(option.effect == DialogueOptionEffect.EndDialogue)
        {
            EndDialogue();
            return;
        }

        if (option.customer != null)
        {
            var reciever = option.customer as IDialogueOptionReciever;
            if (reciever != null)
            {
                Debug.Log("Sending " + option.optionText);
                reciever.OnDialogueOptionChosen(option);
            }
            else
            {
                Debug.LogWarning("Character " + option.customer.name + " does not implement IDialogueOptionReciever");
            }
        }

        // Branching logic: if nextSentenceId is set, jump to that sentence
        if (option.nextSentenceId != -1 && currentDialogue != null)
        {
            if (currentDialogue.sentenceMap.ContainsKey(option.nextSentenceId))
            {
                currentSentence = currentDialogue.sentenceMap[option.nextSentenceId];
                DisplayCurrentSentence();
                return;
            }
            Debug.LogWarning("No sentence found with id " + option.nextSentenceId + ". Ending dialogue.");
            EndDialogue();
            return;
        }
        else
        {
            // If no nextSentenceId is set, end the dialogue
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        Debug.Log("End of conversation");
    }
}
