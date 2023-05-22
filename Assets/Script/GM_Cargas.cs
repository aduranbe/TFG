using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM_Cargas : MonoBehaviour
{
 public void ChangeScene(int pg)
    {
        SceneManager.LoadScene(pg);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
