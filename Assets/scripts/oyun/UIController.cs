using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Action Buttons")]
    public GameObject hitbtn;
    public GameObject standbtn;
    public GameObject splitbtn;
    public GameObject doublebtn;

    [Header("Split X Positions")]
    public float hitXAfterSplit = -420f;
    public float splitXAfterSplit = -490f;
    public float standXAfterSplit = 320f;
    public float doubleXAfterSplit = 390f;

    [Header("Result Buttons/Panels")]
    public GameObject pwinbtn;
    public GameObject dwinbtn;
    public GameObject drawbtn;
    public GameObject blackjackWinBtn;

    [Header("Split Result Panels (9 combos)")]
    public GameObject split_WW;
    public GameObject split_WL;
    public GameObject split_WP;
    public GameObject split_LW;
    public GameObject split_LL;
    public GameObject split_LP;
    public GameObject split_PW;
    public GameObject split_PL;
    public GameObject split_PP;

    public void HideSplitResults()
    {
        SetActiveSafe(split_WW, false);
        SetActiveSafe(split_WL, false);
        SetActiveSafe(split_WP, false);
        SetActiveSafe(split_LW, false);
        SetActiveSafe(split_LL, false);
        SetActiveSafe(split_LP, false);
        SetActiveSafe(split_PW, false);
        SetActiveSafe(split_PL, false);
        SetActiveSafe(split_PP, false);
    }

    public void ShowSplitOutcome(HandOutcome h1, HandOutcome h2)
    {
        HideResults();       // normal sonuçları kapat
        HideSplitResults();  // split sonuçlarını kapat

        // Key üret: W/L/P
        char a = ToChar(h1);
        char b = ToChar(h2);
        string key = $"{a}{b}";

        switch (key)
        {
            case "WW": SetActiveSafe(split_WW, true); break;
            case "WL": SetActiveSafe(split_WL, true); break;
            case "WP": SetActiveSafe(split_WP, true); break;

            case "LW": SetActiveSafe(split_LW, true); break;
            case "LL": SetActiveSafe(split_LL, true); break;
            case "LP": SetActiveSafe(split_LP, true); break;

            case "PW": SetActiveSafe(split_PW, true); break;
            case "PL": SetActiveSafe(split_PL, true); break;
            case "PP": SetActiveSafe(split_PP, true); break;
        }
    }

    char ToChar(HandOutcome o)
    {
        if (o == HandOutcome.Win) return 'W';
        if (o == HandOutcome.Push) return 'P';
        return 'L';
    }

    void SetActiveSafe(GameObject go, bool on)
    {
        if (go) go.SetActive(on);
    }

    // Split öncesi geri dönebilmek için saklıyoruz
    Vector2 hitPre, splitPre, standPre, doublePre;
    bool cachedPrePositions = false;
    public void HideResults()
    {
        if (pwinbtn) pwinbtn.SetActive(false);
        if (dwinbtn) dwinbtn.SetActive(false);
        if (drawbtn) drawbtn.SetActive(false);
        if (blackjackWinBtn) blackjackWinBtn.SetActive(false);
    }

    public void SetActionButtons(bool hit, bool stand, bool split, bool dbl)
    {
        if (hitbtn) hitbtn.SetActive(hit);
        if (standbtn) standbtn.SetActive(stand);
        if (splitbtn) splitbtn.SetActive(split);
        if (doublebtn) doublebtn.SetActive(dbl);
    }

    public void ShowOutcome(RoundOutcome outcome)
    {
        HideResults();

        if (outcome == RoundOutcome.PlayerBlackjackWin && blackjackWinBtn)
            blackjackWinBtn.SetActive(true);
        else if (outcome == RoundOutcome.PlayerWin && pwinbtn)
            pwinbtn.SetActive(true);
        else if (outcome == RoundOutcome.DealerWin && dwinbtn)
            dwinbtn.SetActive(true);
        else if (outcome == RoundOutcome.Push && drawbtn)
            drawbtn.SetActive(true);
    }

    // ✅ Split'e basınca çağır: X değerlerini sabitle
    public void ApplySplitButtonPositions()
    {
        CachePrePositionsOnce();
        SetX(hitbtn, hitXAfterSplit);
        SetX(splitbtn, splitXAfterSplit);
        SetX(standbtn, standXAfterSplit);
        SetX(doublebtn, doubleXAfterSplit);
    }

    // ✅ Yeni el başlayınca çağır: eski konumlara dön
    public void RestorePreSplitButtonPositions()
    {
        if (!cachedPrePositions) return;
        SetPos(hitbtn, hitPre);
        SetPos(splitbtn, splitPre);
        SetPos(standbtn, standPre);
        SetPos(doublebtn, doublePre);
    }

    void CachePrePositionsOnce()
    {
        if (cachedPrePositions) return;

        hitPre = GetPos(hitbtn);
        splitPre = GetPos(splitbtn);
        standPre = GetPos(standbtn);
        doublePre = GetPos(doublebtn);

        cachedPrePositions = true;
    }

    Vector2 GetPos(GameObject go)
    {
        if (!go) return Vector2.zero;
        var rt = go.GetComponent<RectTransform>();
        if (!rt) return Vector2.zero;
        return rt.anchoredPosition;
    }

    void SetX(GameObject go, float x)
    {
        if (!go) return;
        var rt = go.GetComponent<RectTransform>();
        if (!rt) return;

        var p = rt.anchoredPosition;
        p.x = x;               // sadece X değişiyor
        rt.anchoredPosition = p;
    }

    void SetPos(GameObject go, Vector2 pos)
    {
        if (!go) return;
        var rt = go.GetComponent<RectTransform>();
        if (!rt) return;

        rt.anchoredPosition = pos;
    }    
}
