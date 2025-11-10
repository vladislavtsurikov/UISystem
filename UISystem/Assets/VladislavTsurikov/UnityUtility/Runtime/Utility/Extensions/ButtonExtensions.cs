using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ButtonExtensions
{
    public static void SimulateClick(this Button button)
    {
        if (button == null || !button.interactable)
        {
            return;
        }

        var pointerEventData = new PointerEventData(null) { button = PointerEventData.InputButton.Left };
        button.OnPointerClick(pointerEventData);
    }

    public static void SimulatePointerDown(this Button button)
    {
        if (button == null || !button.interactable)
        {
            return;
        }

        var pointerEventData = new PointerEventData(null) { button = PointerEventData.InputButton.Left };
        button.OnPointerDown(pointerEventData);
    }
}
