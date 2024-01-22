using Dev.PlayerLogic;
using Zenject;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayerHealth : Health
    {
        private PlayerCharacter _playerCharacter;

        protected override float Mass => _playerCharacter.Kcc.Settings.Mass;
        protected override float Speed => _playerCharacter.Kcc.Data.RealVelocity.magnitude;

        [Inject]
        private void Construct(PlayerCharacter playerCharacter)
        {
            _playerCharacter = playerCharacter;
        }
    }
}