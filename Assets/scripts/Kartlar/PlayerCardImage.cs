using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardImage : MonoBehaviour
{
    public Image image;

    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public void Goster(Kart kart)
    {
        if (!image || kart == null) return;

        string spriteName = $"{kart.Rank}_{kart.Suit}";

        if (!cache.TryGetValue(spriteName, out Sprite sprite) || sprite == null)
        {
            sprite = Resources.Load<Sprite>($"Cards/{spriteName}");
            if (sprite == null)
            {
                Debug.LogError("Missing sprite: " + spriteName);
                return;
            }
            cache[spriteName] = sprite;
        }

        image.sprite = sprite;
    }
}
