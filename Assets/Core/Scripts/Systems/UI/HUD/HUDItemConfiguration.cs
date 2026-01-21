using System;
using System.Collections.Generic;
using System.Linq;
using Core.Systems.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Systems.UI.HUD
{
    [Serializable]
    public class HUDItemConfiguration
    {
        #region Fields
        
        [Header("Event Configuration")]
        [Tooltip("Events that will show this HUD item")]
        [ValueDropdown("GetEventTypes")]
        [SerializeField]
        private List<string> showOnEvents = new List<string>();
    
        [Tooltip("Events that will hide this HUD item")]
        [ValueDropdown("GetEventTypes")]
        [SerializeField]
        private List<string> hideOnEvents = new List<string>();
    
        [Tooltip("Events that will update this HUD item's data")]
        [ValueDropdown("GetEventTypes")]
        [SerializeField]
        private List<string> updateOnEvents = new List<string>();

        [Header("Behavior")]
        [Tooltip("Should this item be visible on scene start?")]
        [SerializeField]
        private bool visibleOnStart = true;
    
        [Tooltip("Should this item disable its GameObject when hidden?")]
        [SerializeField]
        private bool disableWhenHidden = true;
        
        #endregion
        
        #region Properties
        
        public List<string> ShowOnEvents => showOnEvents;
        public List<string> HideOnEvents => hideOnEvents;
        public List<string> UpdateOnEvents => updateOnEvents;
        public bool VisibleOnStart => visibleOnStart;
        public bool DisableWhenHidden => disableWhenHidden;
        
        #endregion

        #region Editor Helpers

        private static IEnumerable<ValueDropdownItem> GetEventTypes()
        {
            var eventType = typeof(IEvent);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => eventType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => new ValueDropdownItem(t.Name, t.AssemblyQualifiedName));
        }

        #endregion
    }
}
