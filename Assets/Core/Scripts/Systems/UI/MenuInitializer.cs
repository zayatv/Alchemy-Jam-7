using UnityEngine;

namespace Core.Systems.UI
{
    public class MenuInitializer : MonoBehaviour
    {
        [SerializeField]
        private Menu[] menus;
        
        private IUIService _uiService;

        private void Awake()
        {
            foreach (var menu in menus)
            {
                menu.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (_uiService == null)
                ServiceLocator.ServiceLocator.TryGet(out _uiService);

            foreach (var menu in menus)
            {
                _uiService.AddMenu(menu);
            }
        }
        
        private void OnDestroy()
        {
            foreach (var menu in menus)
            {
                _uiService.RemoveMenu(menu);
            }
        }
    }
}