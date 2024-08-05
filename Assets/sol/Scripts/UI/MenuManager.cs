using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] private List<Menu> menus = new List<Menu>();

    public void Awake()
    {
        instance = this;
        OpenMenu("loading"); // !!! THIS TO BE CHANGED this is just a placeholder for opening the loading screen
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
}
