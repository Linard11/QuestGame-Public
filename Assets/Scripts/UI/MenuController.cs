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

    [SerializeField] private string startScene = "Scenes/SampleScene";

    [SerializeField] private string menuScene = "Scenes/MainMenu";

    [SerializeField] private Menu baseMenu;

    [SerializeField] private bool preventBaseClosing;

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

        // Reset timescale
        Time.timeScale = 1;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void Start()
    {
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

    #region Menu Controls

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
        openMenus.Push(menu);
    }

    public void CloseMenu()
    {
        if (openMenus.Count == 0) { return; }

        if (preventBaseClosing &&
            openMenus.Count == 1 &&
            openMenus.Peek() == baseMenu)
        {
            return;
        }

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
        if (!baseMenu.gameObject.activeSelf && openMenus.Count == 0)
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
