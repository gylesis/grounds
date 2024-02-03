using Dev.Scripts.PlayerLogic;

namespace Dev.Scripts.Items
{
    public struct ItemDynamicData
    {
        public PlayerCharacter PlayerCharacter;
        public Health LastOwner;

        public void UpdateOwner(PlayerCharacter owner)
        {
            PlayerCharacter = owner;
            LastOwner = owner.Health;
        }
    }
}