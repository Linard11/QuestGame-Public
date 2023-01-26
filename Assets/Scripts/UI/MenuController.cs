using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static event Action BaseMenuOpening;
    public static event Action BaseMenuClosed;
    
    #region Inspector

    [SerializeField] private string startScene = "Scenes/Sandbox";
    [SerializeField] private string menuScene = "Scenes/MainMenu";

    [Tooltip("Base menu to be opened/closed with the ToggleMenu Action.\nThis is usually the main or pause menu.")]
    [SerializeField] private Menu baseMenu;

    [Tooltip("Prevent base menu from being closed. E.g. in the main menu.")]
    [SerializeField] private bool preventBaseClosing;

    [Tooltip("Hides the previous open menus when opening a new menu on-top.")]
    [SerializeField] private bool hidePreviousMenu;
    
    #endregion

    private GameInput input;
    private Stack<Menu> openMenus;

    #region Unity Event Functions

    private void Awake()
    {
        input = new GameInput();

        input.UI.ToggleMenu.performed += ToggleMenu;
        input.UI.GoBackMenu.performed += GoBackMenu;

        openMenus = new Stack<Menu>();

        // Reset timescale on scene start.
        Time.timeScale = 1;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void Start()
    {
        // Add base menu to stack in case it was open on Start(). E.g. the main menu.
        if (baseMenu.gameObject.activeSelf)
        {
            openMenus.Push(baseMenu);
        }
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void OnDestroy()
    {
        input.UI.ToggleMenu.performed -= ToggleMenu;
        input.UI.GoBackMenu.performed -= GoBackMenu;
    }

    #endregion

    #region Menu Functions

    public void StartGame()
    {
        SceneManager.LoadScene(startScene);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region Menu Managment

    public void OpenMenu(Menu menu)
    {
        if (menu == baseMenu)
        {
            BaseMenuOpening?.Invoke();
        }

        if (hidePreviousMenu && openMenus.Count > 0)
        {
            openMenus.Peek().Hide();
        }
        
        menu.Open();
        // Add menu to the stack.
        openMenus.Push(menu);
    }

    public void CloseMenu()
    {
        if (openMenus.Count == 0) { return; }
        
        // Prevent base menu from closing.
        if (preventBaseClosing &&
            openMenus.Count == 1 &&
            openMenus.Peek() == baseMenu) // Look at the top most menu on the stack without removing it.
        {
            return;
        }
        
        // Remove top most menu from the stack.
        Menu closingMenu = openMenus.Pop();
        closingMenu.Close();

        if (hidePreviousMenu && openMenus.Count > 0)
        {
            openMenus.Peek().Show();
        }

        if (closingMenu == baseMenu)
        {
            BaseMenuClosed?.Invoke();
        }
    }

    private void ToggleMenu(InputAction.CallbackContext _)
    {
        if (!baseMenu.gameObject.activeSelf)
        {
            OpenMenu(baseMenu);
        }
        else
        {
            GoBackMenu(_);
        }
    }

    private void GoBackMenu(InputAction.CallbackContext _)
    {
        CloseMenu();
    }

    #endregion

    #endregion
}
