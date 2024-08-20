using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public TMP_Text textBox;
    public AudioClip typingClip;
    public AudioSourceGroup audioSourceGroup;

    ///Aviv's Addition/Changes
    public bool DialougeIsActive;
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

    private void Awake()
    {
        transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsPlaying)
        {
            CurrentTextActive++;
            GoToNextText();
        }
    }

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
            }
            else
            {
                NameText.text = "Anicetus";
                ProfilePicture2.SetActive(true);
                // ProfilePicture1.SetActive(false);
            }
        }
        else
        {
            IsPlaying = false;
            CurrentTextActive = 0;
            ProfileOneAnim.SetTrigger("Out");
            ProfileTwoAnim.SetTrigger("Out");
            Anim.SetBool("Active", false);
            transform.parent.gameObject.SetActive(false);
            gameManagerEventChannel.RaiseDialogueEnd();
        }

    }


    private Coroutine typeRoutine;
    void PlayDialogue(string message) {
        this.EnsureCoroutineStopped(ref typeRoutine);
        dialogueVertexAnimator.textAnimating = false;
        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(message, out string totalTextMessage);
        typeRoutine = StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, typingClip, null));
    }
}
