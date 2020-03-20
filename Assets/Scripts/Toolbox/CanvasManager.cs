using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    // Create a manager that holds a reference to all the canvas objects in the scene that need to carry over to the 
    // new scene upon load. This manager will not be destroyed.

    private CinematicCanvasController cinCanCon;

    public void SetCinematicCanvasCon(CinematicCanvasController _cinCanCon)
    {
        cinCanCon = _cinCanCon;
    }

    public CinematicCanvasController GetCinematicCanvasController()
    {
        return cinCanCon;
    }
}
