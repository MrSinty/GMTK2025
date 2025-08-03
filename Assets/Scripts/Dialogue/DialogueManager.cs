using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public GameObject optionsPanel;
    public Button optionButtonPrefab;
    public Button continueButton;

    public Image playerImageUI;
    public Image customerImageUI;

    public Animator playerAnimator;
    public Animator customerAnimator;
    public Animator dialogueAnimator;

    [Header("Events")]
    public UnityEvent onDialogueEnded; // Event triggered when any dialogue ends
    public UnityEvent<DialogueOptionEffect> onDialogueOptionChosen; // Event triggered when a dialogue option is chosen

    private Dialogue currentDialogue;
    private DialogueSentence currentSentence;

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueAnimator.SetBool("IsOpen", true);
        customerAnimator.SetBool("IsOpen", true);
        playerAnimator.SetBool("IsOpen", true);

        Debug.Log("Starting conversation with " + dialogue.name);
        nameText.text = dialogue.name;
        customerImageUI.overrideSprite = dialogue.customerImage;
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

        // Customer is speaking
        HighlightSpeaker(false);
        nameText.text = currentDialogue.name;

        Debug.Log("Displaying current sentence: " + currentSentence.sentence);
        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinueButtonClicked);
    }

    void OnContinueButtonClicked()
    {
        Debug.Log("OnContinueButtonClicked");
        continueButton.gameObject.SetActive(false);

        dialogueText.text = "";

        if (currentSentence.options != null && currentSentence.options.Length > 0)
        {
            // Player is speaking (options available)
            HighlightSpeaker(true);
            nameText.text = "You";

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
        // Broadcast the dialogue option effect to all subscribers
        onDialogueOptionChosen?.Invoke(option.effect);
        Debug.Log("Broadcasting Dialogue option chosen: " + option.effect);
        
        if(option.effect == DialogueOptionEffect.EndDialogue)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = "";

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

    void HighlightSpeaker(bool isPlayerSpeaking)
    {
        if (isPlayerSpeaking)
        {
            // Player is speaking
            playerImageUI.color = Color.white; // Normal
            playerImageUI.rectTransform.localScale = Vector3.one * 1.1f; // Slightly larger

            customerImageUI.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Darkened
            customerImageUI.rectTransform.localScale = Vector3.one; // Normal size
        }
        else
        {
            // Customer is speaking
            customerImageUI.color = Color.white;
            customerImageUI.rectTransform.localScale = Vector3.one * 1.1f;

            playerImageUI.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            playerImageUI.rectTransform.localScale = Vector3.one;
        }
    }

    public void EndDialogue()
    {
        playerImageUI.color = Color.white;
        playerImageUI.rectTransform.localScale = Vector3.one;
        customerImageUI.color = Color.white;
        customerImageUI.rectTransform.localScale = Vector3.one;

        Debug.Log("End of conversation");
        dialogueAnimator.SetBool("IsOpen", false);
        customerAnimator.SetBool("IsOpen", false);
        playerAnimator.SetBool("IsOpen", false);
        onDialogueEnded.Invoke(); // Trigger the event
    }

}
