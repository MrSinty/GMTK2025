using UnityEngine;


public abstract class Interactable : MonoBehaviour
{
    public Color tintColor = new Color(1, 1, 0.5f, 1);
    private Color _originalColor = Color.white;

    protected internal bool IsInteractable = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Tint()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = tintColor;
        }
    }

    public void Untint()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = _originalColor;
        }
    }

    public abstract void Interact();
}