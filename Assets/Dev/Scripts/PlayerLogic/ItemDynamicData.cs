using Dev.PlayerLogic;

namespace Dev.Scripts.PlayerLogic
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