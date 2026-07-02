using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform panelRect;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        panelRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Check every frame if the safe area has changed due to folding/unfolding
        if (Screen.safeArea != lastSafeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        lastSafeArea = Screen.safeArea;

        // Convert safe area pixel coordinates to normalized anchor coordinates
        Vector2 anchorMin = lastSafeArea.position;
        Vector2 anchorMax = lastSafeArea.position + lastSafeArea.size;
        
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Apply updated anchors to the UI panel
        panelRect.anchorMin = anchorMin;
        panelRect.anchorMax = anchorMax;
        
        Debug.Log("Safe Area Updated: Foldable state may have changed.");
    }
}