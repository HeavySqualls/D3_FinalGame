using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tutorialTitle;
    [SerializeField] Image tutorialImage;
    [SerializeField] TextMeshProUGUI tutorialInfo;

    [SerializeField] GameObject title;
    [SerializeField] GameObject info;
    [SerializeField] GameObject imagePanel;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject purpleButton;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject closeButton;

    [SerializeField] Animator animator;
    [SerializeField] DialogueController conversationController;

    public bool isOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isOpen)
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
