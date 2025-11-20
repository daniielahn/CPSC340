using UnityEngine;

public class HL_DeckScript : MonoBehaviour
{
    // 0 = back, 1..52 = faces
    public Sprite[] cardSprites;

    int currentIndex = 1;

    void Awake()
    {
        if (cardSprites == null || cardSprites.Length < 2)
        {
            Debug.LogError("HL_DeckScript: Need at least 2 sprites (0 = back).");
        }
    }

    public void Shuffle()
    {
        if (cardSprites == null || cardSprites.Length < 2) return;

        // Fisher–Yates shuffle (keep bck at index 0)
        for (int i = cardSprites.Length - 1; i > 1; i--)
        {
            int j = Random.Range(1, i + 1);
            (cardSprites[i], cardSprites[j]) = (cardSprites[j], cardSprites[i]);
        }

        currentIndex = 1;
    }

    public int DealCard(CardScript slot)
    {
        if (cardSprites == null || cardSprites.Length < 2) return 0;
        if (currentIndex >= cardSprites.Length) Shuffle();

        Sprite face = cardSprites[currentIndex];
        int rank = GetRankFromSpriteName(face.name);

        slot.SetSprite(face);
        slot.SetValue(rank);

        currentIndex++;
        return rank;
    }

    public Sprite GetCardBack() => (cardSprites != null && cardSprites.Length > 0) ? cardSprites[0] : null;

    // ================================
    // Converts sprite name to rank (2–14)
    // ================================
    int GetRankFromSpriteName(string name)
    {
        // Expecting formats like:
        // cardHearts2, cardSpadesJ, cardDiamondsA, cardClubs10

        name = name.ToLower();

        // Aces high = 14
        if (name.EndsWith("a")) return 14;
        if (name.EndsWith("k")) return 13;
        if (name.EndsWith("q")) return 12;
        if (name.EndsWith("j")) return 11;

        // Try to extract numeric value (2–10)
        for (int i = 2; i <= 10; i++)
        {
            if (name.EndsWith(i.ToString()))
                return i;
        }

        // If none match, log for debugging
        Debug.LogWarning($"HL_DeckScript: Unrecognized card name '{name}', defaulting rank=2.");
        return 2;
    }
}
