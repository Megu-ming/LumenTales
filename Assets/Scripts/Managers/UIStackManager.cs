using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UIStackManager : MonoBehaviour
{
    public static UIStackManager Instance { get; private set; }

    [Header("Sorting")]
    public int baseSorting = 100;
    public int step = 10;
    [Header("UI")]
    public GameObject Menu;

    readonly List<UIBase> UIStack = new();

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !CloseTopIfAllowed())
        {
            ToggleMenu();
        }
            
    }

    public void Push(UIBase ui)
    {
        int index = UIStack.IndexOf(ui);
        if (index >= 0) { BringToFront(ui);return; }
        UIStack.Add(ui);
        Reorder();
        ui.OnFocus();
    }

    public void Pop(UIBase ui)
    {
        int index = UIStack.IndexOf(ui);
        if (index < 0) return;
        UIStack.RemoveAt(index);
        Reorder();
    }

    public void BringToFront(UIBase ui)
    {
        int index = UIStack.IndexOf(ui);
        if (index < 0) return;
        UIStack.RemoveAt(index);
        UIStack.Add(ui);
        Reorder();
        ui.OnFocus();
    }

    public UIBase Top => UIStack.Count > 0 ? UIStack[^1] : null;

    public bool CloseTopIfAllowed()
    {
        var top = Top;
        if (top != null && top.closeOnEsc) { top.Close(); return true; }
        else return false;
    }

    void Reorder()
    {
        for (int i = 0; i < UIStack.Count; i++)
            UIStack[i].SetSortingOrder(baseSorting + (i + 1) * step);
    }

    private void ToggleMenu()
    {
        if (Menu != null)
        {
            if (Menu.activeSelf)
            {
                Menu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                Menu.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}

