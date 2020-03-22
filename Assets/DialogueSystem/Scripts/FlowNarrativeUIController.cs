using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlowNarrativeUIController : MonoBehaviour
{
    [Header("UI Base References:")]
    public GameObject speechBubble;
    public TextMeshProUGUI dialogue;
    //public Text dialogue;
    public GameObject speechTarget;

    [Header("Speech Bubble Offset:")]
    [Tooltip("Desired offset position for the speech bubble to be positioned from center of the unit talking.")]
    [SerializeField] Vector3 positionOffset;

    // Set the dialogue text based on what is assigned
    public string Dialogue
    {
        get { return dialogue.text; }
        set { dialogue.text = value; }
    }

    private void Update()
    {
        if (speechTarget != null)
        {
            gameObject.transform.position = speechTarget.transform.position + positionOffset;
        }
    }

    public void ShowFNSpeechBubble(GameObject _target)
    {
        speechBubble.SetActive(true);
        speechTarget = _target;
    }

    public void ShowSpeechBubble()
    {
        speechBubble.SetActive(true);
    }

    public void HideFNSpeechBubble()
    {
        speechBubble.SetActive(false);
    }

    public void RemoveTargetRef()
    {
        speechTarget = null;
    }
}
