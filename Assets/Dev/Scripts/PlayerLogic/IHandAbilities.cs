namespace Dev.Scripts.PlayerLogic
{
    public interface IHandAbilities
    {
        public void PrepareToSwing();

        public void Swing();

        public void Throw();

        public void UseItem();
    }
}