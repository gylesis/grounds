using Fusion;

namespace Dev.Scripts.Infrastructure
{
    public interface IInputListener
    {
        void OnInput(PlayerInput input, NetworkButtons wasPressed, NetworkButtons wasReleased);
    }   
}   