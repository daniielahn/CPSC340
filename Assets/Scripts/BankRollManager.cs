using UnityEngine;

public class BankrollManager : MonoBehaviour
{
    public static BankrollManager I { get; private set; }

    [Header("Settings")]
    public int startingCash = 1000;
    public string prefsKey = "bankroll_cash";

    public int Balance { get; private set; }

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        Balance = PlayerPrefs.GetInt(prefsKey, startingCash);
    }

    public void Deposit(int amount)
    {
        Balance += Mathf.Max(0, amount);
        Save();
    }

    public bool TryWithdraw(int amount)
    {
        if (amount <= 0) return true;
        if (Balance < amount) return false;
        Balance -= amount;
        Save();
        return true;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(prefsKey, Balance);
        PlayerPrefs.Save();
    }
}