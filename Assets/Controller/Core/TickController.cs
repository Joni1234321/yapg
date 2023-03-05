using Bserg.Model.Core;
using Bserg.Model.Units;

namespace Bserg.Controller.Core
{
    public class TickController
    {
        public Controller Controller;

        public TickController(Controller controller)
        {
            Controller = controller;
        }
        
        private static readonly float[] TICK_TIME = { 10f / GameTick.TICKS_PER_MONTH, 5f / GameTick.TICKS_PER_MONTH, 2f / GameTick.TICKS_PER_MONTH, 1f / GameTick.TICKS_PER_MONTH, .2f / GameTick.TICKS_PER_MONTH, .001f, .0001f };
        public float DeltaTick { get; private set; }
        public int GameSpeed { private set; get; }
        public bool Running { private set; get; }
        
        /// <summary>
        /// Returns true if did tick
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool Update(Game game)
        {
            if (!Running)
                return false;
            
            DeltaTick += UnityEngine.Time.deltaTime / TICK_TIME[GameSpeed];
            if (DeltaTick >= 1)
            {
                DeltaTick = 0;
                game.DoTick();
                return true;
            }

            return false;
        }

        public void ToggleGameRunning()
        {
            Running = !Running;
            Controller.OnGameSpeedChange();
        }
        public void IncreaseGameSpeed(int deltaSpeed)
        {
            GameSpeed = UnityEngine.Mathf.Clamp(GameSpeed + deltaSpeed, 0, TICK_TIME.Length - 1);
            Controller.OnGameSpeedChange();
        }



        
    }
    

}