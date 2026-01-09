using UnityEngine;
using UnityEngine.UI;

public class PlayerCardImage : MonoBehaviour
{
    public Image image;

    public void Goster(Kart kart)
    {
        string spriteName = $"{kart.Rank}_{kart.Suit}";
        Sprite sprite = Resources.Load<Sprite>($"Cards/{spriteName}");

        if (sprite == null)
        {
            Debug.LogError("Missing sprite: " + spriteName);
            return;
        }

        image.sprite = sprite;
    }
}
