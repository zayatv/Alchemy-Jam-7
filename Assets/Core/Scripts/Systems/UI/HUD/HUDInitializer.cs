using UnityEngine;

namespace Core.Systems.UI.HUD
{
    public class HUDInitializer : MonoBehaviour
    {
        [SerializeField]
        private HUDItem[] hudItems;

        private IHUDService _hudService;

        private void Awake()
        {
            foreach (var item in hudItems)
            {
                item.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (_hudService == null)
                ServiceLocator.ServiceLocator.TryGet(out _hudService);

            foreach (var item in hudItems)
            {
                _hudService.RegisterItem(item);
            }
        }
        
        private void OnDestroy()
        {
            foreach (var item in hudItems)
            {
                _hudService.UnregisterItem(item);
            }
        }
    }
}