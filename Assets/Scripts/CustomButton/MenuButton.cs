using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

//MY CUSTOM BUTTON CLASS
public enum ButtonState { Normal, Highlighted, Pressed, Disabled }
public class MenuButton : UIBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
    //INSTANCE VARIABLES
    [Tooltip("Makes the button toggleable or not.")]
    public bool toggleable;

    [Tooltip("Used for multiple buttons, most of the time should be 0.")]
    public int index;

    [Tooltip("Just a tooltip lol.")]
    public string tooltip;

    [Tooltip("Set keybinds for your button.")]
    public KeyCode keybind;

    [Header("Coloring")]
    public Color normalTint;
    public Color highlightedTint;
    public Color pressedTint;
    public Color disabledTint;

    [Space]

    public UIBehaviour child;
    public Color childNormalTint;
    public Color childHighlightedTint;
    public Color childPressedTint;
    public Color childDisabledTint;

    public UnityEvent onClick;
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;

    [Header("Testing")]
    public ButtonState currentSelectionState;

    //run every gui frame
    public void OnGUI() {
        if (Input.GetKeyDown(keybind)) {
            onClick.Invoke();
            
            foreach (MenuButton button in FindObjectsOfType<MenuButton>()) {
                if (button.index == index)
                    button.currentSelectionState = ButtonState.Normal;
            }

            currentSelectionState = ButtonState.Pressed;
        }

        SetButtonVisuals();
    }

    //change button visuals
    private void SetButtonVisuals() {
        switch (currentSelectionState) {
            case ButtonState.Disabled:
            GetComponent<Image>().color = disabledTint;

            if (child.GetComponent<Text>())
                child.GetComponent<Text>().color = childDisabledTint;
            else
                child.GetComponent<Image>().color = childDisabledTint;
            break;
            case ButtonState.Highlighted:
            GetComponent<Image>().color = highlightedTint;

            if (child.GetComponent<Text>())
                child.GetComponent<Text>().color = childHighlightedTint;
            else
                child.GetComponent<Image>().color = childHighlightedTint;
            break;
            case ButtonState.Normal:
            GetComponent<Image>().color = normalTint;

            if (child.GetComponent<Text>())
                child.GetComponent<Text>().color = childNormalTint;
            else
                child.GetComponent<Image>().color = childNormalTint;
            break;
            case ButtonState.Pressed:
            GetComponent<Image>().color = pressedTint;

            if (child.GetComponent<Text>())
                child.GetComponent<Text>().color = childPressedTint;
            else
                child.GetComponent<Image>().color = childPressedTint;
            break;
        }
    }

    //detect on click
    public void OnPointerClick(PointerEventData eventData) {
        onClick.Invoke();
    }

    //detect on mouse button down
    public void OnPointerDown(PointerEventData eventData) {
        if (currentSelectionState == ButtonState.Disabled) return;

        foreach (MenuButton button in FindObjectsOfType<MenuButton>()) {
            if (button.index == index)
                button.currentSelectionState = ButtonState.Normal;
        }

        currentSelectionState = ButtonState.Pressed;
    }

    //detect on mouse button up
    public void OnPointerUp(PointerEventData eventData) {
        if (currentSelectionState == ButtonState.Disabled || toggleable) return;

        currentSelectionState = ButtonState.Highlighted;
    }

    //detect on mouse enter
    public void OnPointerEnter(PointerEventData eventData) {
        //if (currentSelectionState == ButtonState.Disabled) return;

        if (currentSelectionState != ButtonState.Pressed && currentSelectionState != ButtonState.Disabled)
            currentSelectionState = ButtonState.Highlighted;

        onMouseEnter.Invoke();
    }

    //detect on mouse exit
    public void OnPointerExit(PointerEventData eventData) {
        //if (currentSelectionState == ButtonState.Disabled) return;

        if (currentSelectionState != ButtonState.Pressed && currentSelectionState != ButtonState.Disabled)
            currentSelectionState = ButtonState.Normal;

        onMouseExit.Invoke();
    }
}
