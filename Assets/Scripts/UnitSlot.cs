using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Holds the unit data ID. -1 means empty.
    public int unitId = -1;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Logic for picking up the unit icon
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Logic for moving the icon with the cursor
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Logic for dropping the unit into an active slot or returning it to the pool
    }
}