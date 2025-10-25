/// <summary>
/// Class that represents a basic collectible that does nothing but reward the player with points.
/// </summary>
public class BasicCollectible : Collectible
{
    private void Start()
    {
        points = 10;
    }
}