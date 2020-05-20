using UnityEngine;

public class ExternalLinkController : MonoBehaviour
{
    public void Itch()
    {
        Application.OpenURL("https://dreamgrenadegames.itch.io/wind-digger");
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
