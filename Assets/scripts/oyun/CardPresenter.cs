using System.Collections.Generic;
using UnityEngine;

public class CardPresenter : MonoBehaviour
{
    [Header("Roots")]
    public RectTransform playerHandsRoot;   // PlayerCards (sabit root)
    public RectTransform handsContainer;    // PlayerCards/HandsContainer (hand'lerin gerçek parent'ı)
    public Transform dealerCardsParent;
    public GameObject cardPrefab;

    [Header("Hand Layout")]
    public float handStepX = 170f;          // Hand_i x = i * 170
    public float handY = 0f;                // Hand_i y = 0

    [Header("HandsContainer Shift")]
    public float splitShiftX = 85f;         // her extra hand için container sola kayma

    [Header("Card Centering (within a hand)")]
    public float firstWidth = 100f;
    public float step = 45f;

    [Header("Card Vertical Stack")]
    public float stackYOffset = 15f;

    // runtime
    readonly List<RectTransform> handParents = new List<RectTransform>();

    // ✅ base position cache (drift olmaması için)
    Vector2 baseHandsPos;
    bool baseHandsPosCached = false;

    // =========================
    // Public API
    // =========================
    public void ClearAll()
    {
        // ✅ yeni turda tekrar cache’lensin
        baseHandsPosCached = false;

        // Dealer kartlarını temizle
        ClearChildren(dealerCardsParent);

        // Hand objelerini temizle (istersen tamamen sil)
        if (handsContainer) ClearChildren(handsContainer);
        else if (playerHandsRoot) ClearChildren(playerHandsRoot);

        handParents.Clear();
    }

    // ✅ Hand container'ları sadece gerektiği kadar oluşturur (reuse mantığıyla da yapılır, ama burada sade tuttum)
    public void EnsurePlayerHandAreas(int handCount)
    {
        if (!playerHandsRoot)
        {
            Debug.LogError("[CardPresenter] playerHandsRoot NULL");
            return;
        }

        if (!handsContainer)
            handsContainer = playerHandsRoot;

        // ✅ her zaman 1 el
        handCount = 1;

        while (handParents.Count < 1)
        {
            var go = new GameObject("Hand_0", typeof(RectTransform));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(handsContainer, false);

            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            rt.anchoredPosition = new Vector2(0f, handY);
            handParents.Add(rt);
        }

        // varsa diğerlerini kapat (önceden ürettiysen)
        for (int i = 1; i < handParents.Count; i++)
            if (handParents[i]) handParents[i].gameObject.SetActive(false);

        handParents[0].gameObject.SetActive(true);
        handParents[0].anchoredPosition = new Vector2(0f, handY);
    }


    public void RenderAllPlayerHands(BlackjackRound round)
    {
        int handCount = round.playerHands.Count;

        EnsurePlayerHandAreas(handCount);

        // Hand'leri temizle
        for (int i = 0; i < handCount; i++)
        {
            if (!handParents[i]) continue;
            ClearChildren(handParents[i]);
        }

        // Kartları çiz
        for (int h = 0; h < handCount; h++)
        {
            foreach (var k in round.playerHands[h].kartlar)
                ShowPlayerCard(h, k, recenterAfterAdd: false);

            CenterCardsInHand(h);
        }
    }

    public void ShowPlayerCard(int handIndex, Kart kart)
    {
        ShowPlayerCard(handIndex, kart, recenterAfterAdd: true);
    }

    // dışarıdan istersen çağır
    public void RecenterHand(int handIndex)
    {
        CenterCardsInHand(handIndex);
    }

    public void ShowDealerCard(Kart kart)
    {
        var rt = SpawnCard(dealerCardsParent, kart);
        if (!rt) return;
        rt.SetAsLastSibling();
    }

    // =========================
    // Internals
    // =========================
    void ShowPlayerCard(int handIndex, Kart kart, bool recenterAfterAdd)
    {
        if (handIndex < 0) return;

        if (handIndex >= handParents.Count)
            EnsurePlayerHandAreas(handIndex + 1);

        if (!handParents[handIndex] || !handParents[handIndex].gameObject.activeSelf) return;

        var parent = handParents[handIndex];

        RectTransform rt = SpawnCard(parent, kart);
        if (!rt) return;

        rt.SetAsLastSibling();

        if (recenterAfterAdd)
            CenterCardsInHand(handIndex);
    }

    void CenterCardsInHand(int handIndex)
    {
        if (handIndex < 0 || handIndex >= handParents.Count) return;
        var hand = handParents[handIndex];
        if (!hand) return;

        int n = hand.childCount;
        if (n == 0) return;

        // ✅ toplam genişlik: (n-1)*step, başlangıç: -total/2
        float total = (n - 1) * step;
        float startX = -total * 0.5f;

        for (int i = 0; i < n; i++)
        {
            RectTransform cardRt = hand.GetChild(i) as RectTransform;
            if (!cardRt) continue;

            float x = startX + i * step;
            float y = i * stackYOffset; // ✅ 2. kart 15 yukarı, 3. kart 30...

            cardRt.anchoredPosition = new Vector2(x, y);
        }
    }


    void ClearChildren(Transform parent)
    {
        if (!parent) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    RectTransform SpawnCard(Transform parent, Kart kart)
    {
        if (!cardPrefab || !parent) return null;

        GameObject obj = Instantiate(cardPrefab);
        var rt = obj.GetComponent<RectTransform>();
        if (!rt) { Destroy(obj); return null; }

        obj.transform.SetParent(parent, false);

        // ✅ her şeyi parent merkezine göre sabitle
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        rt.anchoredPosition3D = Vector3.zero; // ✅ X,Y,Z sıfır
        rt.sizeDelta = rt.sizeDelta; // dokunma ama layout override'ı tetiklememek için stabil

        var view = obj.GetComponent<PlayerCardImage>();
        if (view) view.Goster(kart);

        return rt;
    }

}
