using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject hitbtn;
    public GameObject standbtn;

    public GameObject pwinbtn;
    public GameObject dwinbtn;
    public GameObject drawbtn;

    Deste deste;
    El playerEl;
    El dealerEl;

    public Transform playerCardsParent;   // PlayerCards
    public Transform dealerCardsParent;   // DealerCards
    public GameObject cardPrefab;         // CardPrefab

    // Akış kilidi (spam click engeller)
    bool elDevamEdiyor = false;

    [Header("Timings")]
    public float kartCekmeGecikmesi = 0.25f;
    public float elSonuBekleme = 1.5f;

    public void YeniElBaslat()
    {
        // Yeni elde tekrar kontrol
        StopAllCoroutines();
        elDevamEdiyor = true;

        pwinbtn.SetActive(false);
        dwinbtn.SetActive(false);
        drawbtn.SetActive(false);

        deste = new Deste(4);
        playerEl = new El();
        dealerEl = new El();

        Temizle(playerCardsParent);
        Temizle(dealerCardsParent);

        hitbtn.SetActive(true);
        standbtn.SetActive(true);

        // Başlangıç dağıtımı coroutine ile (görsel akış)
        StartCoroutine(BaslangicDagitimi());
    }

    IEnumerator BaslangicDagitimi()
    {
        OyuncuyaKartVer();
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        DealerKartVer();
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        OyuncuyaKartVer();
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        DealerKartVer();
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        int p = playerEl.Skor();
        int d = dealerEl.Skor();

        if (p == 21 || d == 21)
        {
            KilitleButonlar();

            if (p == 21 && d == 21) Debug.Log("BERABERE (Ikisi de Blackjack)");
            else if (p == 21) Debug.Log("KAZANDIN (Blackjack!)");
            else Debug.Log("KAYBETTIN (Dealer Blackjack)");

            yield return new WaitForSeconds(elSonuBekleme);
            YeniElBaslat();
            yield break;
        }

        Debug.Log("Oyuncu skor: " + p);
        Debug.Log("Dealer skor: " + d);
    }

    void KilitleButonlar()
    {
        hitbtn.SetActive(false);
        standbtn.SetActive(false);
        elDevamEdiyor = false;
    }

    void Temizle(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    void KartGoster(Transform parent, Kart kart)
    {
        GameObject obj = Instantiate(cardPrefab, parent);

        var view = obj.GetComponent<PlayerCardImage>();

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

    // HIT button
    public void Hit()
    {
        if (!elDevamEdiyor) return;

        OyuncuyaKartVer();

        int p = playerEl.Skor();

        if (p > 21)
        {
            StartCoroutine(ElBitirVeYenile("KAYBETTIN (Bust)"));
        }
        else if (p == 21)
        {
            // otomatik stand
            Stand();
        }
    }

    // STAND button
    public void Stand()
    {
        if (!elDevamEdiyor) return;

        KilitleButonlar();
        StartCoroutine(DealerOynasinVeSonuc());
    }

    IEnumerator DealerOynasinVeSonuc()
    {
        // Dealer 17'ye kadar çeker (kartlar aralıklı görünsün)
        while (dealerEl.Skor() < 17)
        {
            DealerKartVer();
            yield return new WaitForSeconds(kartCekmeGecikmesi);
        }

        // Sonucu yazdır
        int p = playerEl.Skor();
        int d = dealerEl.Skor();

        Debug.Log("Oyuncu: " + p + " | Dealer: " + d);

        string mesaj;
        if (p > 21)
        {
            mesaj = "KAYBETTIN (Bust)";
            dwinbtn.SetActive(true);
        } 
        else if (d > 21)
        {
            mesaj = "KAZANDIN (Dealer bust)";
            pwinbtn.SetActive(true);
        }
        else if (p > d)
        {
            mesaj = "KAZANDIN";
            pwinbtn.SetActive(true);
        }
        else if (p < d)
        {
            mesaj = "KAYBETTIN";
            dwinbtn.SetActive(true);
        }
        else
        {
            mesaj = "BERABERE (Push)";
            drawbtn.SetActive(true);
        }

        Debug.Log(mesaj);

        yield return new WaitForSeconds(elSonuBekleme);
        YeniElBaslat();
    }

    IEnumerator ElBitirVeYenile(string mesaj)
    {
        KilitleButonlar();
        Debug.Log(mesaj);
        yield return new WaitForSeconds(elSonuBekleme);
        YeniElBaslat();
    }
}
