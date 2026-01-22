using Core.Game.Inventory.Events;
using Core.Systems.Events;
using Core.Systems.UI.HUD;
using TMPro;
using UnityEngine;

namespace Core.Game.GameStates.DungeonState.UI.HUD
{
    public class BombCounterHUDItem : HUDItem
    {
        [SerializeField] private TextMeshProUGUI amountText;

        private int _amount;

        protected override bool ValidateData(IEvent eventData)
        {
            if (eventData is BombCountChangedEvent bombCountChangedEvent && bombCountChangedEvent.NewCount != _amount)
            {
                return true;
            }

            return false;
        }
        
        protected override void UpdateData(IEvent eventData)
        {
            if (eventData is BombCountChangedEvent bombCountChangedEvent)
            {
                _amount = bombCountChangedEvent.NewCount;
                
                UpdateAmountText();
            }
        }
        
        private void UpdateAmountText()
        {
            amountText.text = _amount.ToString();
        }
    }
}