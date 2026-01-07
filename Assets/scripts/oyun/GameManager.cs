using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject hitbtn;
    public GameObject standbtn;

    Deste deste;

    El playerEl;
    El dealerEl;

    public Transform playerCardsParent;   // PlayerCards
    public Transform dealerCardsParent;   // DealerCards
    public GameObject cardPrefab;         // CardPrefab

    void Start()
    {
        YeniElBaslat();
    }

    void YeniElBaslat()
    {
        deste = new Deste(4);
        playerEl = new El();
        dealerEl = new El();

        Temizle(playerCardsParent);
        Temizle(dealerCardsParent);

        // Baþlangýç: oyuncu 2, dealer 2
        OyuncuyaKartVer();
        DealerKartVer();
        OyuncuyaKartVer();
        DealerKartVer();

        hitbtn.SetActive(true);
        standbtn.SetActive(true);

        int p = playerEl.Skor();
        int d = dealerEl.Skor();

        if (p == 21 || d == 21)
        {
            hitbtn.SetActive(false);
            standbtn.SetActive(false);

            if (p == 21 && d == 21) Debug.Log("BERABERE (Ikisi de Blackjack)");
            else if (p == 21) Debug.Log("KAZANDIN (Blackjack!)");
            else Debug.Log("KAYBETTIN (Dealer Blackjack)");

            return;
        }

        Debug.Log("Oyuncu skor: " + playerEl.Skor());
        Debug.Log("Dealer skor: " + dealerEl.Skor());
    }

    void Temizle(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    void KartGoster(Transform parent, Kart kart)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("cardPrefab BOS! Inspector'dan CardPrefab ata.");
            return;
        }

        if (parent == null)
        {
            Debug.LogError("Kart parent BOS! Inspector'dan PlayerCards/DealerCards ata.");
            return;
        }

        GameObject obj = Instantiate(cardPrefab, parent);

        var view = obj.GetComponent<PlayerCardImage>();
        if (view == null)
        {
            Debug.LogError("CardPrefab ustunde PlayerCardImage script'i YOK!");
            return;
        }

        view.Goster(kart);
    }

    void OyuncuyaKartVer()
    {
        Kart k = deste.KartCek();
        playerEl.Ekle(k);
        KartGoster(playerCardsParent, k);

    }

    void DealerKartVer()
    {
        Kart k = deste.KartCek();
        dealerEl.Ekle(k);
        KartGoster(dealerCardsParent, k);
    }

    // HIT button -> bunu baðla
    public void Hit()
    {
        OyuncuyaKartVer();
        int p = playerEl.Skor();
        if (p > 21)
        {
            hitbtn.SetActive(false);
            standbtn.SetActive(false);
            Debug.Log("KAYBETTIN (Bust)");
            YeniElBaslat();
        }
        else if (p == 21)
        {
            Stand(); // otomatik stand (istersen)
        }
    }

    // STAND button -> bunu baðla
    public void Stand()
    {
        hitbtn.SetActive(false);
        standbtn.SetActive(false);

        // Dealer 17'ye kadar çeker
        while (dealerEl.Skor() < 17)
            DealerKartVer();

        SonucuBelirle();
    }

    void SonucuBelirle()
    {
        int p = playerEl.Skor();
        int d = dealerEl.Skor();

        Debug.Log("Oyuncu: " + p + " | Dealer: " + d);

        if (p > 21) 
        { 
            hitbtn.SetActive(false);
            standbtn.SetActive(false);
            Debug.Log("KAYBETTIN (Bust)"); 
            return; 
        }
        if (d > 21) { Debug.Log("KAZANDIN (Dealer bust)"); return; }

        if (p > d) Debug.Log("KAZANDIN");
        else if (p < d) Debug.Log("KAYBETTIN");
        else Debug.Log("BERABERE (Push)");
        YeniElBaslat();
    }
}
