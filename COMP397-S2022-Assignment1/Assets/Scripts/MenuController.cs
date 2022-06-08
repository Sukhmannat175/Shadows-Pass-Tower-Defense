// MenuController.cs
// Charlie Han Bi - 301176547
// 05/29/2022
// Load Main Menu
// Initial Script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{



    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}