using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void YesQuitButton()
    {
        Application.Quit();
        Debug.Log("PlayerQuit");
    }

}
