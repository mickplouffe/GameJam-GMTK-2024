using System;
using System.Collections.Generic;
using CristiEventSystem.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Serialization;

public class DialogueController : MonoBehaviour
{
    public TMP_Text textBox;
    public AudioClip typingClip;
    public AudioSourceGroup audioSourceGroup;

    ///Aviv's Addition/Changes
    public bool DialogueIsActive;
    [SerializeField] int CurrentTextActive;
    [TextArea]
    public string[] dialogue1;
    public bool[] WhichCharacter;
    public GameObject ProfilePicture1;
    public GameObject ProfilePicture2;
    private DialogueVertexAnimator dialogueVertexAnimator;
    public TMP_Text NameText;
    public Animator Anim;
    public Animator ProfileOneAnim;
    public Animator ProfileTwoAnim;

    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    
    public bool IsPlaying { get; set; }

    private void OnEnable()
    {
        transform.parent.gameObject.SetActive(true);
        transform.gameObject.SetActive(true);
    }

    private void Awake()
    { 
        transform.parent.gameObject.SetActive(true);
        transform.gameObject.SetActive(true);

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsPlaying)
        {
            CurrentTextActive++;
            GoToNextText();
        }
    }
    [Button]
    public void StartDialogue()
    {
        IsPlaying = true;
        CurrentTextActive = 0;
        dialogueVertexAnimator = new DialogueVertexAnimator(textBox, audioSourceGroup);
        GoToNextText();
        Anim.SetBool("Active", true);
    }

    void GoToNextText()
    {
        if (CurrentTextActive <= dialogue1.Length - 1)
        {
            PlayDialogue(dialogue1[CurrentTextActive]);


            if (WhichCharacter[CurrentTextActive] == true)
            {
                NameText.text = "Alexiares";
                ProfilePicture1.SetActive(true);
                // ProfilePicture2.SetActive(false);
                ProfileOneAnim.SetBool("IsTalking", true);
                ProfileTwoAnim.SetBool("IsTalking", false);
            }
            else
            {
                NameText.text = "Anicetus";
                ProfilePicture2.SetActive(true);
                // ProfilePicture1.SetActive(false);
                ProfileOneAnim.SetBool("IsTalking", false);
                ProfileTwoAnim.SetBool("IsTalking", true);
            }
        }
        else
        {
            IsPlaying = false;
            CurrentTextActive = 0;
            ProfileOneAnim.SetTrigger("Out");
            ProfileTwoAnim.SetTrigger("Out");
            Anim.SetBool("Active", false);
            Invoke("DisableUI", 1);
        }

    }
    void DisableUI()
    {
        gameManagerEventChannel.RaiseDialogueEnd();
        transform.parent.gameObject.SetActive(false);
        Destroy(transform.parent.gameObject);
    }


    private Coroutine typeRoutine;
    void PlayDialogue(string message) {
        this.EnsureCoroutineStopped(ref typeRoutine);
        dialogueVertexAnimator.textAnimating = false;
        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(message, out string totalTextMessage);
        typeRoutine = StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, typingClip, null));
    }
}
