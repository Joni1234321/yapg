namespace Bserg.Model.Core.Operators
{
    /// <summary>
    /// Operator is an open gate between Controller and Model
    /// </summary>
    public class GameOperator
    {
        public readonly Game Game;

        public GameOperator(Game game)
        {
            Game = game;
        }
    }
}