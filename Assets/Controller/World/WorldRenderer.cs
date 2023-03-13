namespace Bserg.Controller.World
{
    
    /// <summary>
    /// Worldview shows stuff in the world, updtes every frame
    /// </summary>
    public abstract class WorldRenderer
    {
        public abstract void OnUpdate(int ticks, float dt);
    }
}