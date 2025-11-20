using UnityEngine;
using UnityEngine.UI;

public class HiLowManager : MonoBehaviour
{
    [Header("Refs")]
    public HL_DeckScript deck;
    public CardScript currentCard;
    public CardScript nextCard;

    [Header("UI")]
    public Button dealButton;
    public Button higherButton;
    public Button lowerButton;
    public Text msgText;
    public Text cashText;
    public Text betText;

    [Header("Money Settings")]
    public int betAmount = 20;

    // runtime
    int curRank;
    bool inRound = false;
    Sprite backSprite;

    void Start()
    {
        // start bankroll
        // shuffle deck and setup backs
        deck.Shuffle();
        backSprite = deck.GetCardBack();

        currentCard.SetBack(backSprite);
        nextCard.SetBack(backSprite);
        currentCard.ResetCard();
        nextCard.ResetCard();

        UpdateHUD("Press Deal to start.");

        higherButton.interactable = false;
        lowerButton.interactable = false;
        dealButton.interactable = true;
    }

    // ==========================
    // Deal Button
    // ==========================
    public void OnDeal()
    {
        if (inRound) return; // ignore spam
        if (!BankrollManager.I.TryWithdraw(betAmount))
        {
            UpdateHUD("Not enough money!");
            return;
        }

        // place bet
        Debug.Log($"[HL] betAmount = {betAmount}");


        // reset visuals
        nextCard.ResetCard();

        // draw first card
        curRank = deck.DealCard(currentCard);
        currentCard.GetComponent<Renderer>().enabled = true;

        // update UI
        UpdateHUD("Guess Higher or Lower");
        inRound = true;

        dealButton.interactable = false;
        higherButton.interactable = true;
        lowerButton.interactable = true;
    }
    
    public void OnHigher() { OnGuess(true); }
    public void OnLower()  { OnGuess(false); }

    // ==========================
    // Higher / Lower Buttons
    // ==========================
    public void OnGuess(bool isHigher)
    {
        if (!inRound) return;

        int nextRank = deck.DealCard(nextCard);
        nextCard.GetComponent<Renderer>().enabled = true;

        string result;
        // OnGuess(bool isHigher)
        if (nextRank == curRank) {
            // PUSH
            BankrollManager.I.Deposit(betAmount);                // return stake
            result = "Equal — Push!";
        }
        else if ((nextRank > curRank && isHigher) || (nextRank < curRank && !isHigher)) {
            // WIN
            BankrollManager.I.Deposit(betAmount * 2);            // stake + winnings
            result = $"You win +${betAmount}!";
        } else {
            // LOSE -> nothing to deposit
            result = $"You lose -${betAmount}.";
        }
        UpdateHUD("");


        // round ends
        curRank = nextRank;
        inRound = false;

        higherButton.interactable = false;
        lowerButton.interactable = false;
        dealButton.interactable = true;

        UpdateHUD(result + "\nPress Deal for next round.");
    }

    // ==========================
    // Helpers
    // ==========================
    void UpdateHUD(string msg)
    {
        if (msgText)  msgText.text = msg;
        if (cashText) cashText.text = $"Cash: ${BankrollManager.I.Balance}";
        if (betText)  betText.text  = $"Bet: ${betAmount}";
    }

}
