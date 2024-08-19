using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text textBox;
    public AudioClip typingClip;
    public AudioSourceGroup audioSourceGroup;




    ///Aviv's Addition/Changes
    public bool DialougeIsActive;
    [SerializeField] int CurrentTextActive = 0;
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
    void Awake() {
        dialogueVertexAnimator = new DialogueVertexAnimator(textBox, audioSourceGroup);
        GoToNextText();
        Anim.SetBool("Active", true);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentTextActive++;
            GoToNextText();
        }
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
                ProfilePicture2.SetActive(false);
            }
            else
            {
                NameText.text = "Anicetus";
                ProfilePicture2.SetActive(true);
                ProfilePicture1.SetActive(false);
            }
        }
        else
        {
            ProfileOneAnim.SetTrigger("Out");
            ProfileTwoAnim.SetTrigger("Out");
            Anim.SetBool("Active", false);
            print("Start timer");
            //Start the round Timer
        }

    }


    private Coroutine typeRoutine = null;
    void PlayDialogue(string message) {
        this.EnsureCoroutineStopped(ref typeRoutine);
        dialogueVertexAnimator.textAnimating = false;
        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(message, out string totalTextMessage);
        typeRoutine = StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, typingClip, null));
    }
}
