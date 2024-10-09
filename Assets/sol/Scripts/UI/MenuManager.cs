using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private List<Menu> menus = new List<Menu>();

    public void Awake()
    {
        Instance = this;
        OpenMenu("loading"); 
    }

    public void OpenMenu(string menuName)
    {
        OpenMenu(menuName, true);
    }

    public void OpenMenu(string menuName, bool onlyThisMenu)
    {
        bool foundMenu = false;
        foreach (Menu menu in menus)
        {
            if (menu.GetName().Equals(menuName))
            {
                menu.Open();
                foundMenu = true;
            }
            else if (menu.enabled && onlyThisMenu)
            {
                CloseMenu(menu);
            }
        }

        if (!foundMenu)
        {
            string caller = new StackFrame(1).GetMethod().Name;
            UnityEngine.Debug.LogError("Menu Manager could not find menu by the name \"" + menuName + "\" from method: " + caller);
        }
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void OnTextChange(TMP_InputField input)
    {
        if (input.text.EndsWith("\n"))
        {
            input.text.Remove(input.text.Length - 1);
            PhotonLauncher.Instance.JoinRoom();
        }
    }
}
