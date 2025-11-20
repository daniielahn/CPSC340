// CardScript.cs (patch)
using UnityEngine;

public class CardScript : MonoBehaviour
{
    [Header("Value")]
    public int value = 0;

    [Header("Rendering")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite backSprite; // set this from HL_DeckScript in scene

    void Awake() {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public int GetValueOfCard() => value;
    public void SetValue(int newValue) => value = newValue;

    public string GetSpriteName() => spriteRenderer && spriteRenderer.sprite ? spriteRenderer.sprite.name : "";

    public void SetSprite(Sprite newSprite)
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = newSprite;
        GetComponent<Renderer>().enabled = true;   // make sure it shows
    }
    
    public void SetBack(Sprite s) { backSprite = s; }

    public void ResetCard() {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = backSprite;
        value = 0;
        GetComponent<Renderer>().enabled = true;
    }
    
}