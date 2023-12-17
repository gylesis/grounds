using Fusion;

namespace Dev.Infrastructure
{
    public interface IInputListener
    {
        void OnInput(PlayerInput input, NetworkButtons wasPressed, NetworkButtons wasReleased);
    }   
}   