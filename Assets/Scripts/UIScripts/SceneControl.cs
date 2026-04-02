using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
public void NextScene1()
    {
        SceneManager.LoadScene("Level 1 (Tutorial)");
    }
    public void NextScene2()
    {
        SceneManager.LoadScene("Level 2");
    }
    public void NextScene3()
    {
        SceneManager.LoadScene("Level 3");
    }
    public void NextScene0()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
