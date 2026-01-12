public enum RoundState
{
    Dealing,
    PlayerTurn,
    DealerTurn,
    Resolving,
    Idle
}

public enum RoundOutcome
{
    None,
    PlayerWin,
    DealerWin,
    Push,
    PlayerBlackjackWin
}
public enum HandOutcome
{
    Lose,
    Push,
    Win
}

public class SplitHandResult
{
    public HandOutcome hand1Outcome;
    public HandOutcome hand2Outcome;

    public int hand1Score;
    public int hand2Score;
    public int dealerScore;

    public bool dealerBust;
    public string message;
}

public class HandResult
{
    public int playerScore;
    public int dealerScore;

    public bool playerBust;
    public bool dealerBust;

    public bool playerBlackjack;
    public bool dealerBlackjack;

    public RoundOutcome outcome;
    public string message;
}