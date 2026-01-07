using UnityEngine;
using UnityEngine.UI;

public class PlayerCardImage : MonoBehaviour
{
    public Image image;

    public void Goster(Kart kart)
    {
        string spriteAdi = kart.Rank + "_" + kart.Tur;
        Sprite sprite = Resources.Load<Sprite>("Cards/" + spriteAdi);

        if (sprite == null)
        {
            Debug.LogError("Sprite bulunamadý: " + spriteAdi);
            return;
        }

        image.sprite = sprite;
    }
}
