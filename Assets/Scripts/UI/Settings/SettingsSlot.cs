/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Apple Basket
 * Creation Date: 2/10/2023 2:13:12 PM
 * 
 * Description: Handles the functionality for a single setting slot.
*********************************/
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ValidCheck;

public class SettingsSlot : MonoBehaviour
{
    #region Fields
    [Tooltip("The sprite to show when hovering over this slot")]
    [SerializeField] private Sprite hoverSprite;

    /// <summary>
    /// The default sprite of this settings slot.
    /// </summary>
    private Sprite startingSprite;

    /// <summary>
    /// The image to apply background sprites to.
    /// </summary>
    private Image slotImage;

    /// <summary>
    /// An event that is called when even this slot is clicked.
    /// </summary>
    public UnityEvent ClickEvent = new UnityEvent();

    [Tooltip("The color for when this slot is disabled")]
    [SerializeField] private Color disabledColor;

    [Tooltip("Hold true if this slot can be clicked by default")]
    [SerializeField] private bool canClickSlot = true;

    /// <summary>
    /// Holds true if this slot is currently able to be clicked on.
    /// </summary>
    public bool CanClickSlot
    {
        get
        {
            return canClickSlot;
        }

        set
        {
            canClickSlot = value;

            CheckInteractableState();
        }
    }

    [Tooltip("Hold true if this slot can be hovered over by default")]
    [SerializeField] private bool canHoverSlot = true;

    /// <summary>
    /// Holds true if this slot is currently able to be hovered over.
    /// </summary>
    public bool CanHoverSlot
    {
        get
        {
            return canHoverSlot;
        }

        set
        {
            canHoverSlot = value;

            CheckInteractableState();
        }
    }
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components and fields.
    /// </summary>
    private void Awake()
    {
        slotImage = GetComponent<Image>();
        startingSprite = slotImage.sprite;

        if (!canHoverSlot)
        {
            CheckInteractableState();

            FindObjectOfType<BodySourceManager>().SensorFoundEvent.AddListener(UpdateHoverability);
        }
    }

    private void UpdateHoverability(bool sensorFound)
    {
        CanHoverSlot = sensorFound;
    }

    /// <summary>
    ///  Checks and sets the visual for its interactable state.
    /// </summary>
    private void CheckInteractableState()
    {
        if (!canClickSlot || !canHoverSlot)
        {
            SetImageAndTextColors(disabledColor);
        }
        else
        {
            SetImageAndTextColors(Color.white);
        }
    }

    private void SetImageAndTextColors(Color newColor)
    {
        foreach (var image in slotImage.GetComponentsInChildren<Image>())
        {
            image.color = newColor;
        }

        foreach (var text in slotImage.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.color = newColor;
        }
    }

    /// <summary>
    /// Calls for this slots click event to be invoked.
    /// </summary>
    public void ClickSlot()
    {
        ClickEvent.Invoke();
    }

    /// <summary>
    /// Sets the hover state of this settings slot.
    /// </summary>
    /// <param name="shouldSet">Holds true if this slot should be hovered over currently.</param>
    public void SetHover(bool shouldSet)
    {
        if (IsntValid(slotImage))
        {
            slotImage = GetComponent<Image>();
            startingSprite = slotImage.sprite;
        }

        var newImage = shouldSet ? hoverSprite : startingSprite;
        slotImage.sprite = newImage;
    }
    #endregion
}
