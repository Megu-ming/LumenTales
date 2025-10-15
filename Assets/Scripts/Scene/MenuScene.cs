using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();
        // 기본 씬 설정
        sceneType = SceneType.Menu;
        Cursor.lockState = CursorLockMode.None;

        UIManager.instance.InitUI();

        UIManager.instance.mainMenuPanel.Init();
    }
}
