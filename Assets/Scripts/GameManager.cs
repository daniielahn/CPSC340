using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button betBtn;   // chip button
    bool bettingLocked = false;


    [Header("HUD Texts")]
    public Text scoreText;
    public Text dealerScoreText;
    public Text betsText;
    public Text cashText;
    public Text mainText;
    public Text standBtnText;

    [Header("Table / Visuals")]
    public GameObject hideCard;

    [Header("Scripts")]
    public PlayerScript playerScript;
    public PlayerScript dealerScript;

    private int playerBet = 0; // only the player’s stake

    void Awake()
    {
        // Make sure buttons are wired once
        dealBtn.onClick.RemoveAllListeners();
        hitBtn.onClick.RemoveAllListeners();
        standBtn.onClick.RemoveAllListeners();
        betBtn.onClick.RemoveAllListeners();

        dealBtn.onClick.AddListener(DealClicked);
        hitBtn.onClick.AddListener(HitClicked);
        standBtn.onClick.AddListener(StandClicked);
        betBtn.onClick.AddListener(BetClicked);
    }

    void Start()
    {
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
        dealBtn.gameObject.SetActive(true);
        dealerScoreText.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        UpdateScoresHud();
        UpdateBetsHud();
        UpdateCashHud();
    }

    // =========================
    // ROUND FLOW
    // =========================
    public void DealClicked()
    {
        const int defaultBet = 20;

        // If player hasn't placed a bet yet, auto-bet $20
        if (playerBet == 0)
        {
            if (playerScript.GetMoney() >= defaultBet)
            {
                playerBet = defaultBet;
                playerScript.AdjustMoney(-defaultBet);
            }
            else
            {
                mainText.text = "Not enough cash to bet.";
                mainText.gameObject.SetActive(true);
                return;
            }
        }

        // Reset hands
        playerScript.ResetHand();
        dealerScript.ResetHand();

        dealerScoreText.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);

        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();

        playerScript.StartHand();
        dealerScript.StartHand();

        // Hide dealer hole card
        if (hideCard) hideCard.GetComponent<Renderer>().enabled = true;

        // Buttons
        dealBtn.gameObject.SetActive(false);
        hitBtn.gameObject.SetActive(true);
        standBtn.gameObject.SetActive(true);
        standBtnText.text = "Stand";
        // Lock betting during active hand
        bettingLocked = true;
        betBtn.interactable = false;


        UpdateScoresHud();
        UpdateBetsHud();
        UpdateCashHud();

        // Check for natural blackjacks
        if (CheckNaturalsAfterDeal())
            EndRoundUI();
    }

    public void HitClicked()
    {
        if (playerScript.cardIndex <= 10)
        {
            playerScript.GetCard();
            scoreText.text = "Hand: " + playerScript.handValue;

            if (playerScript.handValue > 21)
                RoundOver(); // bust
        }
    }

    public void StandClicked()
    {
        HitDealer();
        RoundOver();
    }

    void HitDealer()
    {
        while (dealerScript.handValue < 17 && dealerScript.cardIndex < 10)
        {
            dealerScript.GetCard();
            dealerScoreText.text = "Hand: " + dealerScript.handValue;
            if (dealerScript.handValue > 21) break;
        }
    }

    // =========================
    // ROUND RESULTS
    // =========================
    void RoundOver()
    {
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;

        if (hideCard) hideCard.GetComponent<Renderer>().enabled = false;
        dealerScoreText.gameObject.SetActive(true);

        if (playerBust && dealerBust)
        {
            mainText.text = "All Bust — Push";
            RefundPlayerBet(); // return bet
        }
        else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue))
        {
            mainText.text = "Dealer wins!";
            // lose bet (do nothing)
        }
        else if (dealerBust || playerScript.handValue > dealerScript.handValue)
        {
            mainText.text = "You win!";
            PayoutPlayer(playerBet * 2); // get back bet + winnings
        }
        else // push
        {
            mainText.text = "Push — Bets returned";
            RefundPlayerBet();
        }

        EndRoundUI();
        playerBet = 0;
        UpdateBetsHud();
        UpdateCashHud();
        // Unlock betting for next round
        bettingLocked = false;
        betBtn.interactable = true;

    }

    bool CheckNaturalsAfterDeal()
    {
        bool playerBJ = HasBlackjack(playerScript);
        bool dealerBJ = HasBlackjack(dealerScript);

        if (!playerBJ && !dealerBJ) return false;

        if (hideCard) hideCard.GetComponent<Renderer>().enabled = false;
        dealerScoreText.gameObject.SetActive(true);

        if (playerBJ && dealerBJ)
        {
            mainText.text = "Both Blackjack — Push";
            RefundPlayerBet();
        }
        else if (playerBJ)
        {
            mainText.text = "Blackjack! Paid 3:2";
            int payout = Mathf.RoundToInt(playerBet * 2.5f); // bet + 1.5× winnings
            PayoutPlayer(payout);
        }
        else
        {
            mainText.text = "Dealer Blackjack";
            // lose bet
        }

        playerBet = 0;
        UpdateBetsHud();
        UpdateCashHud();
        return true;
    }

    bool HasBlackjack(PlayerScript ps)
    {
        return ps.handValue == 21 && ps.cardIndex == 2;
    }

    // =========================
    // BUTTON PAYOUT HELPERS
    // =========================
    void PayoutPlayer(int amount)
    {
        playerScript.AdjustMoney(amount);
        cashText.text = "$" + playerScript.GetMoney();
    }

    void RefundPlayerBet()
    {
        playerScript.AdjustMoney(playerBet);
        cashText.text = "$" + playerScript.GetMoney();
    }

    // =========================
    // BETTING CHIP
    // =========================
    public void BetClicked()
    {
        int chipValue = 20; // each click adds $20
        if (bettingLocked)
            return;  // Ignore bet presses during a hand

        if (playerScript.GetMoney() < chipValue)
        {
            mainText.text = "Not enough cash.";
            mainText.gameObject.SetActive(true);
            return;
        }

        playerBet += chipValue;
        playerScript.AdjustMoney(-chipValue);
        UpdateBetsHud();
        UpdateCashHud();
    }

    // =========================
    // UI HELPERS
    // =========================
    void EndRoundUI()
    {
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
        dealBtn.gameObject.SetActive(true);
        mainText.gameObject.SetActive(true);
        dealerScoreText.gameObject.SetActive(true);
    }

    void UpdateScoresHud()
    {
        scoreText.text = "Hand: " + playerScript.handValue;
        dealerScoreText.text = "Hand: " + dealerScript.handValue;
    }

    void UpdateBetsHud()
    {
        betsText.text = "Bet: $" + playerBet;
    }

    void UpdateCashHud()
    {
        cashText.text = "$" + playerScript.GetMoney();
    }
}
