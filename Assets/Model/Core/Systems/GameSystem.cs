namespace Bserg.Model.Core.Systems
{
    public abstract class GameSystem
    {
        public readonly Game Game;

        public GameSystem(Game game)
        {
            this.Game = game;
        }
    }
}