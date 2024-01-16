using Dev.UI;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class QuickTabReactiveButton : ReactiveButton<QuickTab>
    {
        private QuickTab _quickTab;
        
        protected override QuickTab Value => _quickTab;

        public void Setup(QuickTab quickTab)
        {
            _quickTab = quickTab;
        }
    }
}