using UnityEngine;

public class CardPresenter : MonoBehaviour
{
    [Header("Roots")]
    public RectTransform playerHand1Parent;   // Split yokken / hand1
    public RectTransform playerHand2Parent;   // Split sonrası 1. el
    public RectTransform playerHand3Parent;   // Split sonrası 2. el
    public Transform dealerCardsParent;       // DealerCards

    public GameObject cardPrefab;

    [Header("Player Layout")]
    public float step = 45f;
    public float stackYOffset = 15f;

    public void ClearAll()
    {
        ClearChildren(playerHand1Parent);
        ClearChildren(playerHand2Parent);
        ClearChildren(playerHand3Parent);
        ClearChildren(dealerCardsParent);
    }

    void ClearChildren(Transform parent)
    {
        if (!parent) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    /// <summary>
    /// slotIndex:
    /// 0 = hand1
    /// 1 = hand2
    /// 2 = hand3
    /// </summary>
    public void ShowPlayerCard(int slotIndex, Kart kart)
    {
        if (kart == null) return;

        RectTransform parent =
            slotIndex == 0 ? playerHand1Parent :
            slotIndex == 1 ? playerHand2Parent :
            playerHand3Parent;

        if (!parent) return;

        RectTransform rt = SpawnCard(parent, kart);
        if (!rt) return;

        rt.SetAsLastSibling();
        CenterStackPlayerCards(parent);
    }

    public void ShowDealerCard(Kart kart)
    {
        if (!dealerCardsParent || kart == null) return;

        RectTransform rt = SpawnCard(dealerCardsParent, kart);
        if (!rt) return;

        rt.SetAsLastSibling();
    }

    // ✅ Dealer ilk kart kapalı
    public void ShowDealerCardHidden(Kart kart)
    {
        if (!dealerCardsParent || kart == null) return;

        RectTransform rt = SpawnCard(dealerCardsParent, kart);
        if (!rt) return;

        var view = rt.GetComponent<PlayerCardImage>();
        if (view) view.GosterKapali();

        rt.SetAsLastSibling();
    }

    // ✅ Dealer kapalı kartı aç
    public void RevealDealerFirstCard(Kart realCard)
    {
        if (!dealerCardsParent) return;
        if (dealerCardsParent.childCount <= 0) return;

        var first = dealerCardsParent.GetChild(0);
        var view = first.GetComponent<PlayerCardImage>();
        if (view) view.Ac(realCard);
    }

    // 🔒 KONUM KODU SADECE BURADA
    void CenterStackPlayerCards(RectTransform parent)
    {
        int n = parent.childCount;
        if (n == 0) return;

        float total = (n - 1) * step;
        float startX = -total * 0.5f;

        for (int i = 0; i < n; i++)
        {
            RectTransform cardRt = parent.GetChild(i) as RectTransform;
            if (!cardRt) continue;

            float x = startX + i * step;
            float y = i * stackYOffset;
            cardRt.anchoredPosition = new Vector2(x, y);
        }
    }

    RectTransform SpawnCard(Transform parent, Kart kart)
    {
        if (!cardPrefab || !parent || kart == null) return null;

        GameObject obj = Instantiate(cardPrefab);
        var rt = obj.GetComponent<RectTransform>();
        if (!rt)
        {
            Destroy(obj);
            return null;
        }

        obj.transform.SetParent(parent, false);

        var view = obj.GetComponent<PlayerCardImage>();
        if (view) view.Goster(kart);

        return rt;
    }
}
