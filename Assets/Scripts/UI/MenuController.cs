using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    #region Inspector

    [SerializeField] private string startScene = "Scenes/Sandbox";
    [SerializeField] private string menuScene = "Scenes/MainMenu";

    [SerializeField] private Menu baseMenu;

    [Tooltip("Prevent the base menu from being closed. E.g. in the main menu.")]
    [SerializeField] private bool preventBaseClosing;
    
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
    }

    private void Start()
    {
        // Add base menu to stack in case it as open on start. E.g. the main menu.
        if (baseMenu.gameObject.activeSelf)
        {
            openMenus.Push(baseMenu);
        }
    }

    private void OnEnable()
    {
        input.Enable();
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

    #region Menu Mangement

    public void OpenMenu(Menu menu)
    {
        menu.Open();
        // Add menu to the stack.
        openMenus.Push(menu);
    }

    public void CloseMenu()
    {
        if (openMenus.Count == 0) { return; }
        
        // Prevent base from closing.
        if (preventBaseClosing &&
            openMenus.Count == 1 &&
            openMenus.Peek() == baseMenu) // Look at the top menu on the stack without removing it.
        {
            return;
        }
        
        // Remove top most menu from the stack.
        Menu closingMenu = openMenus.Pop();
        closingMenu.Close();
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
