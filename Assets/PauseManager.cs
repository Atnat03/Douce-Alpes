using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool isPaused = false;

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
    }
}
