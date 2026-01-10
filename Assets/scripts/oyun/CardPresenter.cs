using UnityEngine;

public class CardPresenter : MonoBehaviour
{
    [Header("Roots")]
    public RectTransform playerCardsParent;   // PlayerCards (RectTransform)
    public Transform dealerCardsParent;       // DealerCards
    public GameObject cardPrefab;

    [Header("Player Layout")]
    public float step = 45f;          // X aralığı (merkezleme için)
    public float stackYOffset = 15f;  // her yeni kartın Y kayması

    public void ClearAll()
    {
        ClearChildren(playerCardsParent);
        ClearChildren(dealerCardsParent);
    }

    void ClearChildren(Transform parent)
    {
        if (!parent) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    public void ShowPlayerCard(Kart kart)
    {
        if (!playerCardsParent || kart == null) return;

        RectTransform rt = SpawnCard(playerCardsParent, kart);
        if (!rt) return;

        rt.SetAsLastSibling();
        CenterStackPlayerCards();
    }

    public void ShowDealerCard(Kart kart)
    {
        if (!dealerCardsParent || kart == null) return;

        RectTransform rt = SpawnCard(dealerCardsParent, kart);
        if (!rt) return;

        rt.SetAsLastSibling();
    }

    void CenterStackPlayerCards()
    {
        int n = playerCardsParent.childCount;
        if (n == 0) return;

        float total = (n - 1) * step;
        float startX = -total * 0.5f;

        for (int i = 0; i < n; i++)
        {
            var cardRt = playerCardsParent.GetChild(i) as RectTransform;
            if (!cardRt) continue;

            float x = startX + i * step;
            float y = i * stackYOffset;
            cardRt.anchoredPosition = new Vector2(x, y);
        }
    }

    RectTransform SpawnCard(Transform parent, Kart kart)
    {
        if (!cardPrefab || !parent || kart == null) return null;

        // Parent ile instantiate: UI için daha stabil
        GameObject obj = Object.Instantiate(cardPrefab, parent, false);

        var rt = obj.GetComponent<RectTransform>();
        if (!rt) { Object.Destroy(obj); return null; }

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        rt.anchoredPosition = Vector2.zero;

        var view = obj.GetComponent<PlayerCardImage>();
        if (view) view.Goster(kart);

        return rt;
    }
}