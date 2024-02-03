using Dev.Scripts.Items;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Infrastructure.Installers
{
    public class InventoryInstaller : MonoInstaller
    {
        [SerializeField] private InventoryItemsDragHandler _inventoryItemsDragHandler;

        public override void InstallBindings()
        {
            Container.Bind<InventoryItemsDragHandler>().FromInstance(_inventoryItemsDragHandler).AsSingle();
        }
    }
}