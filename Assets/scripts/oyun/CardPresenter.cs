using System.Collections.Generic;
using UnityEngine;

public class CardPresenter : MonoBehaviour
{
    [Header("Roots")]
    public RectTransform playerHandsRoot;   // PlayerCards
    public Transform dealerCardsParent;
    public GameObject cardPrefab;

    [Header("Hand Layout")]
    public float handStepX = 170f;          // Hand_i x = i * 170
    public float handY = 0f;                // Hand_i y = 0

    [Header("Player Card Stack")]
    public Vector2 playerStartOffset = Vector2.zero;
    public Vector2 playerStackOffset = new Vector2(40f, 25f);

    // runtime
    readonly List<RectTransform> handParents = new List<RectTransform>();
    readonly List<List<RectTransform>> handCards = new List<List<RectTransform>>();

    public void ClearAll()
    {
        // Dealer kartlarını temizle
        ClearChildren(dealerCardsParent);

        // Hand içlerini temizle + hand listelerini sıfırla
        for (int i = 0; i < handParents.Count; i++)
            ClearChildren(handParents[i]);

        // Hand container'ları da sıfırdan başlatmak istersen aç:
        // ClearChildren(playerHandsRoot);
        // handParents.Clear();

        handCards.Clear();
        for (int i = 0; i < handParents.Count; i++)
            handCards.Add(new List<RectTransform>());
    }

    void ClearChildren(Transform parent)
    {
        if (!parent) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    // ✅ Hand container'ları sadece gerektiği kadar oluşturur, SİLMEZ
    public void EnsurePlayerHandAreas(int handCount)
    {
        if (!playerHandsRoot)
        {
            Debug.LogError("[CardPresenter] playerHandsRoot NULL");
            return;
        }

        if (handCount < 1) handCount = 1;

        // Eksik hand varsa ekle
        while (handParents.Count < handCount)
        {
            int i = handParents.Count;

            var go = new GameObject($"Hand_{i}", typeof(RectTransform));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(playerHandsRoot, false);

            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            handParents.Add(rt);
        }

        // handCards listesi hand sayısıyla uyumlu olsun
        while (handCards.Count < handParents.Count)
            handCards.Add(new List<RectTransform>());

        // Fazla hand varsa (istenirse) pasif yap ya da sil
        // Şimdilik silmiyoruz; sadece görünmez yapalım:
        for (int i = 0; i < handParents.Count; i++)
            handParents[i].gameObject.SetActive(i < handCount);

        // ✅ Pozisyonları deterministik yap: yeni el her zaman en sağda
        for (int i = 0; i < handCount; i++)
            handParents[i].anchoredPosition = new Vector2(i * handStepX, handY);
    }

    // ✅ Bu fonksiyon artık Ensure çağırmaz (GameManager çağıracak)
    public void RenderAllPlayerHands(BlackjackRound round)
    {
        // Önce hand içlerini temizle
        for (int i = 0; i < round.playerHands.Count; i++)
        {
            ClearChildren(handParents[i]);
            handCards[i].Clear();
        }

        // Sonra çiz
        for (int h = 0; h < round.playerHands.Count; h++)
        {
            foreach (var k in round.playerHands[h].kartlar)
                ShowPlayerCard(h, k);
        }
    }

    public void ShowPlayerCard(int handIndex, Kart kart)
    {
        if (handIndex < 0 || handIndex >= handParents.Count) return;
        if (!handParents[handIndex].gameObject.activeSelf) return;

        var parent = handParents[handIndex];

        RectTransform rt = SpawnCard(parent, kart);
        if (!rt) return;

        rt.SetAsLastSibling();

        handCards[handIndex].Add(rt);
        ApplyStack(rt, handCards[handIndex].Count - 1);
    }

    void ApplyStack(RectTransform rt, int index)
    {
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = playerStartOffset + playerStackOffset * index;
    }

    public void ShowDealerCard(Kart kart)
    {
        var rt = SpawnCard(dealerCardsParent, kart);
        if (!rt) return;
        rt.SetAsLastSibling();
    }

    RectTransform SpawnCard(Transform parent, Kart kart)
    {
        if (!cardPrefab || !parent) return null;

        GameObject obj = Instantiate(cardPrefab);
        obj.transform.SetParent(parent, false);

        var view = obj.GetComponent<PlayerCardImage>();
        if (view) view.Goster(kart);

        return obj.GetComponent<RectTransform>();
    }
}
