namespace Bserg.Model.Core.Systems
{
    /// <summary>
    /// Game systems only manipulates its own data, and only modifies stuff in Model
    /// Game systems shouldnt be modified
    /// </summary>
    public abstract class GameSystem
    {
        public readonly Game Game;

        public GameSystem(Game game)
        {
            Game = game;
        }
    }
}