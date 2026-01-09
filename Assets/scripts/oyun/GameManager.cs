using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public UIController ui;
    public CardPresenter presenter;

    [Header("Round Settings")]
    public int desteSayisi = 4;
    public int karistirEsigi = 100;
    public int baseBet = 10;

    [Header("Timings")]
    public float kartCekmeGecikmesi = 0.25f;
    public float elSonuBekleme = 1.5f;

    [Header("Dealer Rules")]
    public bool dealerHitsSoft17 = false;

    // =========================
    // Layout / Parent refs
    // =========================
    [Header("Player Hands Parent (PlayerCards)")]
    [SerializeField] private Transform playerHandAreasParent; // PlayerCards (RectTransform)

    [Header("Split Positioning")]
    [SerializeField] private float handStepX = 170f;     // new hand target = (170 * child_i, 0)
    [SerializeField] private float parentShiftX = 85f;   // parentX = parentX - 85
    [SerializeField] private float parentFixedY = -440f; // parentY = -440

    [Header("Card Spacing (Centering)")]
    [SerializeField] private float firstWidth = 100f; // centerX = (100 + (n-1)*45)/2
    [SerializeField] private float step = 45f;

    BlackjackRound round;
    DealerAI dealerAI = new DealerAI();

    RoundState state = RoundState.Idle;
    bool inputLocked = false;

    // =========================
    // UI helpers
    // =========================
    void SetActionButtons(bool hit, bool stand, bool split, bool dbl)
    {
        if (!ui) return;

        if (ui.hitbtn) ui.hitbtn.SetActive(hit);
        if (ui.standbtn) ui.standbtn.SetActive(stand);
        if (ui.splitbtn) ui.splitbtn.SetActive(split);
        if (ui.doublebtn) ui.doublebtn.SetActive(dbl);
    }

    void RefreshButtons()
    {
        if (!ui || round == null || inputLocked || state != RoundState.PlayerTurn)
        {
            SetActionButtons(false, false, false, false);
            return;
        }

        int h = round.activeHandIndex;

        if (round.IsHandDone(h))
        {
            SetActionButtons(false, false, false, false);
            return;
        }

        int score = round.playerHands[h].Skor();

        SetActionButtons(
            score < 21,
            true,
            round.CanSplit(h),
            round.CanDoubleDown(h)
        );
    }

    void LockActions()
    {
        inputLocked = true;
        state = RoundState.Idle;
        SetActionButtons(false, false, false, false);
    }

    // =========================
    // Parent / Hand positioning helpers
    // =========================
    float GetParentX()
    {
        if (!playerHandAreasParent) return 0f;

        if (playerHandAreasParent is RectTransform prt)
            return prt.anchoredPosition.x;

        return playerHandAreasParent.localPosition.x;
    }

    void SetParentPos(float x, float y)
    {
        if (!playerHandAreasParent) return;

        if (playerHandAreasParent is RectTransform prt)
        {
            prt.anchoredPosition = new Vector2(x, y);
        }
        else
        {
            var p = playerHandAreasParent.localPosition;
            playerHandAreasParent.localPosition = new Vector3(x, y, p.z);
        }
    }

    // Hand_i pozisyonunu root (PlayerCards) local uzayında ayarlar
    void SetHandLocalPos(int handIndex, Vector2 localPos)
    {
        if (!playerHandAreasParent) return;
        if (handIndex < 0 || handIndex >= playerHandAreasParent.childCount) return;

        Transform hand = playerHandAreasParent.GetChild(handIndex);

        if (hand is RectTransform rt)
            rt.anchoredPosition = localPos;
        else
            hand.localPosition = new Vector3(localPos.x, localPos.y, hand.localPosition.z);
    }

    // =========================
    // Card Layout (center inside a hand)
    // centerX = (100 + (n-1)*45) / 2
    // =========================
    void RepositionPlayerHandCards(int handIndex)
    {
        if (!playerHandAreasParent) return;
        if (handIndex < 0 || handIndex >= playerHandAreasParent.childCount) return;

        RectTransform handArea = playerHandAreasParent.GetChild(handIndex) as RectTransform;
        if (!handArea) return;

        int n = handArea.childCount;
        if (n < 2) return;

        float centerX = (firstWidth + (n - 1) * step) / 2f;

        for (int i = 0; i < n; i++)
        {
            RectTransform cardRt = handArea.GetChild(i) as RectTransform;
            if (!cardRt) continue;

            float x = (i * step) - centerX;
            cardRt.anchoredPosition = new Vector2(x, cardRt.anchoredPosition.y);
        }
    }

    // =========================
    // Public (UI Buttons)
    // =========================
    public void YeniElBaslat()
    {
        StopAllCoroutines();

        round = new BlackjackRound(desteSayisi, karistirEsigi, baseBet);
        state = RoundState.Dealing;
        inputLocked = true;

        if (ui)
        {
            ui.HideResults();
            SetActionButtons(false, false, false, false);
        }

        if (presenter) presenter.ClearAll();

        // Başlangıçta 1 hand oluştur
        if (presenter) presenter.EnsurePlayerHandAreas(1);

        // Root'u Y=-440'a sabitle (X aynı kalsın)
        SetParentPos(GetParentX(), parentFixedY);

        // Hand_0'ı (0,0)'a al (lokal)
        SetHandLocalPos(0, Vector2.zero);

        StartCoroutine(BaslangicDagitimi());
    }

    public void Hit()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        int h = round.activeHandIndex;

        var card = round.Hit(h);
        presenter.ShowPlayerCard(h, card);

        RepositionPlayerHandCards(h);

        int p = round.playerHands[h].Skor();

        if (p > 21)
        {
            round.Stand(h);
            AdvanceOrDealer();
            return;
        }

        if (p == 21)
        {
            Stand();
            return;
        }

        RefreshButtons();
    }

    public void Stand()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        int h = round.activeHandIndex;
        round.Stand(h);

        AdvanceOrDealer();
    }

    public void DoubleDown()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        int h = round.activeHandIndex;
        if (!round.CanDoubleDown(h)) return;

        var card = round.DoubleDown(h);
        presenter.ShowPlayerCard(h, card);

        // Double sonrası otomatik stand
        Stand();
    }

    // =========================
    // SPLIT (istediğin sıra birebir)
    // 1) yeni child hedefi (170 * child_i, 0)
    // 2) parent -> (parentX - 85, -440)
    // 3) Ensure ile child oluştur, sonra hedefe koy
    // =========================
    public void Split()
    {
        if (inputLocked || state != RoundState.PlayerTurn) return;

        int h = round.activeHandIndex;
        if (!round.CanSplit(h)) return;

        if (!round.Split(h)) return;

        inputLocked = true;
        RefreshButtons();

        // 1) yeni child index ve hedef pozisyon
        int newHandIndex = round.playerHands.Count - 1;
        Vector2 newHandTargetLocal = new Vector2(handStepX * newHandIndex, 0f);

        // 2) parent'ı (parentX - 85, -440) konumuna götür
        float parentX = GetParentX();
        SetParentPos(parentX - parentShiftX, parentFixedY);

        // 3) child'ı oluştur (Ensure) ve hedefe yerleştir
        presenter.EnsurePlayerHandAreas(round.playerHands.Count);

        // yeni hand'ı hedef konuma koy
        SetHandLocalPos(newHandIndex, newHandTargetLocal);

        // Hand_0'ı garantiye al
        if (newHandIndex == 1)
            SetHandLocalPos(0, Vector2.zero);

        // redraw
        presenter.RenderAllPlayerHands(round);

        // (Güvence) Eğer presenter redraw sırasında hand pos resetliyorsa, tekrar set et:
        SetHandLocalPos(newHandIndex, newHandTargetLocal);
        if (newHandIndex == 1) SetHandLocalPos(0, Vector2.zero);

        // split kuralı: her ele 1 kart
        StartCoroutine(SplitDealOneEach(h));
    }

    // =========================
    // Flow
    // =========================
    IEnumerator BaslangicDagitimi()
    {
        // Player 1
        presenter.ShowPlayerCard(0, round.DealToPlayer(0));
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Dealer 1
        presenter.ShowDealerCard(round.DealToDealer());
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Player 2
        presenter.ShowPlayerCard(0, round.DealToPlayer(0));

        // gecikmesiz ortala
        RepositionPlayerHandCards(0);

        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // Dealer 2
        presenter.ShowDealerCard(round.DealToDealer());
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        state = RoundState.PlayerTurn;
        inputLocked = false;

        RefreshButtons();
    }

    IEnumerator SplitDealOneEach(int splitIndex)
    {
        // İlk ele 1 kart
        var c1 = round.DealToPlayer(splitIndex);
        presenter.ShowPlayerCard(splitIndex, c1);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // İkinci ele 1 kart
        var c2 = round.DealToPlayer(splitIndex + 1);
        presenter.ShowPlayerCard(splitIndex + 1, c2);
        yield return new WaitForSeconds(kartCekmeGecikmesi);

        // iki eli de ortala
        RepositionPlayerHandCards(splitIndex);
        RepositionPlayerHandCards(splitIndex + 1);

        round.SetActiveHand(splitIndex);

        state = RoundState.PlayerTurn;
        inputLocked = false;

        RefreshButtons();
    }

    void AdvanceOrDealer()
    {
        if (round.MoveToNextPlayableHand())
        {
            inputLocked = false;
            state = RoundState.PlayerTurn;
            RefreshButtons();
            return;
        }

        inputLocked = true;
        state = RoundState.DealerTurn;
        RefreshButtons();

        StartCoroutine(DealerOynasinVeSonuc());
    }

    IEnumerator DealerOynasinVeSonuc()
    {
        while (dealerAI.ShouldHit(round.dealerHand, dealerHitsSoft17))
        {
            var c = round.DealToDealer();
            presenter.ShowDealerCard(c);
            yield return new WaitForSeconds(kartCekmeGecikmesi);
        }

        var res = round.ResolveHand(0);
        if (ui) ui.ShowOutcome(res.outcome);

        yield return new WaitForSeconds(elSonuBekleme);
        YeniElBaslat();
    }
}
