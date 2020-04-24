using UnityEngine;

public class ExternalLinkController : MonoBehaviour
{
    public void Facebook()
    {
        Application.OpenURL("https://www.facebook.com/DreamGrenade");
    }
    public void Website()
    {
        Application.OpenURL("https://www.dreamgrenade.com/");
    }

    public void Twitter()
    {
        Application.OpenURL("https://twitter.com/DreamGrenade");
    }
}
