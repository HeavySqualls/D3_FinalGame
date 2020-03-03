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
    [SerializeField] GameObject closeButton;

    [SerializeField] Animator animator;

    public void DisplayTutorial(sTutorial _tutorialData)
    {
        tutorialPanel.SetActive(!tutorialPanel.activeSelf);

        animator.SetBool("isOpen", true);

        tutorialTitle.text = _tutorialData.tutorialTitle;
        tutorialInfo.text = _tutorialData.tutorialInfo;
        tutorialImage.sprite = _tutorialData.tutorialImage;

        StartCoroutine(DisplayDelay());

        // TODO: track that the tutorial has been seen here??
    }

    public void HideTutorial()
    {
        title.SetActive(false);
        info.SetActive(false);
        imagePanel.SetActive(false);
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
        closeButton.SetActive(true);

        yield break;
    }

    IEnumerator HideDelay()
    {
        yield return new WaitForSeconds(0.5f);

        tutorialPanel.SetActive(false);

        yield break;
    }
}
