using System;
using System.Collections.Generic;
using Core.Systems.ServiceLocator;

namespace Core.Systems.UI
{
    public interface IUIService : IService
    {
        /// <summary>
        /// Represents a stack of menus currently managed by the UI system.
        /// This property provides a mechanism to track the order in which menus are opened
        /// and to ensure proper handling of menu transitions and closures.
        /// </summary>
        /// <remarks>
        /// The stack is used internally by the UI system to manage the active menus.
        /// Menus are pushed onto the stack when opened and popped when closed.
        /// This allows for LIFO (Last In, First Out) behavior, ensuring that the most recently
        /// opened menu is the first one to be closed.
        /// </remarks>
        Stack<Menu> MenuStack { get; }

        /// <summary>
        /// Opens a menu of the specified type. Allows customization for closing the previous menu
        /// and animating transitions between menus.
        /// </summary>
        /// <param name="menuType">
        /// The type of the menu to open.
        /// </param>
        /// <param name="closePrevious">
        /// If true, the currently opened menu will be closed before the new menu is opened.
        /// </param>
        /// <param name="animatePreviousOut">
        /// If true, the previous menu will animate out during its closing process.
        /// </param>
        void OpenMenu(Type menuType, bool closePrevious = false, bool animatePreviousOut = true);

        /// <summary>
        /// Opens a menu of the specified generic type. Allows customization for closing the previous menu
        /// and animating transitions between menus.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the menu to open. Must inherit from the Menu class.
        /// </typeparam>
        /// <param name="closePrevious">
        /// If true, the currently opened menu will be closed before the new menu is opened.
        /// </param>
        /// <param name="animatePreviousOut">
        /// If true, the previous menu will animate out during its closing process.
        /// </param>
        void OpenMenu<T>(bool closePrevious = false, bool animatePreviousOut = true) where T : Menu;

        /// <summary>
        /// Opens a menu using its specific class name. Searches for a menu type matching the given name
        /// and opens it, allowing for operations to be performed on the identified menu type.
        /// </summary>
        /// <param name="menuName">
        /// The exact class name of the menu to open (e.g., "PauseMenu").
        /// </param>
        void OpenMenuByName(string menuName);

        /// <summary>
        /// Closes the currently active menu if one is open. Handles any necessary
        /// cleanup actions and triggers transition animations during the process.
        /// </summary>
        /// <remarks>
        /// If no menus are open or a transition is currently in progress, the method will not perform any operations.
        /// </remarks>
        void CloseMenu();

        /// <summary>
        /// Closes all menus currently in the menu stack. Updates system states if any menus
        /// were previously open to ensure consistent behavior after closing all menus.
        /// </summary>
        /// <param name="animateTop">
        /// If true, the top-most menu will animate its closure.
        /// </param>
        void CloseAllMenus(bool animateTop = true);

        /// <summary>
        /// Retrieves a menu of the specified type from the existing collection of menus.
        /// If no menu of the given type is found, the method will return null.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the menu to retrieve. Must be a subclass of the Menu class.
        /// </typeparam>
        /// <returns>
        /// The first menu instance of the specified type if found; otherwise, null.
        /// </returns>
        T GetMenu<T>() where T : Menu;
    }
}