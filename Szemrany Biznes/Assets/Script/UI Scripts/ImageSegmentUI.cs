using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageSegment : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RawImage rawImage;
    private Texture2D texture;
    private Color originalColor;
    public Color highlightColor = Color.yellow;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage.texture != null)
        {
            texture = rawImage.texture as Texture2D;
        }
        originalColor = rawImage.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsMouseOverOpaque(eventData))
        {
            rawImage.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rawImage.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsMouseOverOpaque(eventData))
        {
            Debug.Log("Klikniêto aktywn¹ czêœæ obrazu: " + gameObject.name);
            FindObjectOfType<BooksUI>().OnBookClicked();
        }
    }

    private bool IsMouseOverOpaque(PointerEventData eventData)
    {
        if (texture == null) return false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        Rect rect = rawImage.rectTransform.rect;

        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (x < 0 || x >= texture.width || y < 0 || y >= texture.height) return false;

        Color pixelColor = texture.GetPixel(x, y);
        return pixelColor.a > 0.1f;
    }
}
