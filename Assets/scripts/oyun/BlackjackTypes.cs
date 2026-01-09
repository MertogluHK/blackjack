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
    Push
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
