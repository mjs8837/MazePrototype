using UnityEngine;

/// <summary>
/// Abstract class used to represent collectibles.
/// </summary>
public abstract class Collectible : MonoBehaviour
{
    protected int points;

    public int Points
    {
        get { return points; }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // If collding with the player, set this game object inactive and add points to the player.
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);

            if (other.TryGetComponent(out Player player))
            {
                player.GrabCollectible(this);
            }
        }
    }
}