namespace LatronArs.Engine.Scene.Objects
{
    public interface ILightBox
    {
        Tile CurrentTile { get; }

        (double noise, double time) SwitchLight();
    }
}