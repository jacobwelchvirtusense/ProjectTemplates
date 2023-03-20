/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Apple Basket
 * Creation Date: 2/10/2023 2:13:12 PM
 * 
 * Description: Handles the functionality for a single setting slot.
*********************************/
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
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components and fields.
    /// </summary>
    private void Awake()
    {
        slotImage = GetComponent<Image>();
        startingSprite = slotImage.sprite;
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
