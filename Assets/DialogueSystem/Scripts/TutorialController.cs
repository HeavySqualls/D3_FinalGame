using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    public bool isOpen = false;

    [Header("Tutorial Image:")]
    [Tooltip("The DisplayPanel GameObject of the tutorial panel.")]
    [SerializeField] GameObject imagePanel;
    [Tooltip("The image component of the DisplayPanel game object.")]
    [SerializeField] Image tutorialImage;

    [Space]
    [Header("Tutorial Title:")]
    [Tooltip("The TitleText GameObject of the tutorial panel.")]
    [SerializeField] GameObject title;
    [Tooltip("The TextMeshPro component of the TitleText game object.")]
    [SerializeField] TextMeshProUGUI tutorialTitle;

    [Space]
    [Header("Tutorial Information:")]
    [Tooltip("The InfoText GameObject of the tutorial panel.")]
    [SerializeField] GameObject info;
    [Tooltip("The TextMeshPro component of the InfoText game object.")]
    [SerializeField] TextMeshProUGUI tutorialInfo;

    [Space]
    [Header("Tutorial Input Buttons:")]
    [SerializeField] GameObject purpleButton;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject closeButton;

    [Space]
    [Header("Tutorial References:")]
    [Tooltip("The GameObject of the tutorial panel.")]
    [SerializeField] GameObject tutorialPanel;
    [Tooltip("The Animator of the tutorial panel.")]
    [SerializeField] Animator animator;
    [Tooltip("The NarrativeController component of the Dialogue game object.")]
    [SerializeField] NarrativeController conversationController;

    PlayerController pCon;

    private void Start()
    {
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
    }

    private void Update()
    {
        if ((Input.GetButtonDown(pCon.controls.interact) || Controls.IsRight) && isOpen)
        {
            HideTutorial();
        }
    }

    public void DisplayTutorial(sTutorial _tutorialData)
    {
        tutorialPanel.SetActive(!tutorialPanel.activeSelf);

        animator.SetBool("isOpen", true);

        isOpen = true;

        tutorialTitle.text = _tutorialData.tutorialTitle;
        tutorialInfo.text = _tutorialData.tutorialInfo;
        tutorialImage.sprite = _tutorialData.tutorialImage;

        StartCoroutine(DisplayDelay());

        // TODO: track that the tutorial has been seen here??
    }

    public void CloseConversationAndTutorial()
    {
        HideTutorial();
        conversationController.EndResetController();
    }

    public void HideTutorial()
    {
        title.SetActive(false);
        info.SetActive(false);
        imagePanel.SetActive(false);

        purpleButton.SetActive(false);
        continueButton.SetActive(false);
        closeButton.SetActive(false);

        animator.SetBool("isOpen", false);
        StartCoroutine(HideDelay());
    }

    IEnumerator DisplayDelay()
    {
        yield return new WaitForSeconds(0.5f);

        title.SetActive(true);
        info.SetActive(true);
        imagePanel.SetActive(true);
        purpleButton.SetActive(true);
        continueButton.SetActive(true);
        closeButton.SetActive(true);

        yield break;
    }

    IEnumerator HideDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (!conversationController.CheckIfConversationIsFinished())
        {
            conversationController.AdvanceConversation();
        }

        tutorialPanel.SetActive(false);
        isOpen = false;

        yield break;
    }
}
