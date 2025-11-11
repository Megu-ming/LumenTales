using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootScene : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
