using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class UserInterface : MonoBehaviour
{
    public Player player;
    public InventoryObject inventory;
    public Dictionary<GameObject, InventorySlot> itemDisplayed = new Dictionary<GameObject, InventorySlot>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            inventory.Container.Items[i].parent = this;
        }
        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });

    }

    // Update is called once per frame
    void Update()
    {
        UpdateSlots();
    }

    public void UpdateSlots()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemDisplayed)
        {
            if (_slot.Value.ID >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";

            }
        }
    }

    public abstract void CreateSlots();

    public void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        player.mouseItem.hoverObj = obj;

        if (itemDisplayed.ContainsKey(obj))
        {
            player.mouseItem.hoverItem = itemDisplayed[obj];
        }
    }

    public void OnExit(GameObject obj)
    {
        player.mouseItem.hoverObj = null;
        player.mouseItem.hoverItem = null;
    }

    public void OnEnterInterface(GameObject obj)
    {
        player.mouseItem.ui = obj.GetComponent<UserInterface>();
    }

    public void OnExitInterface(GameObject obj)
    {

        player.mouseItem.ui = null;
    }

    public void OnDragStart(GameObject obj)
    {
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        mouseObject.transform.SetParent(transform.parent);

        if (itemDisplayed[obj].ID >= 0)
        {
            var img = mouseObject.AddComponent<Image>();
            img.sprite = inventory.database.GetItem[itemDisplayed[obj].ID].uiDisplay;
            img.raycastTarget = false;
        }
        player.mouseItem.obj = mouseObject;
        player.mouseItem.item = itemDisplayed[obj];
    }

    public void OnDragEnd(GameObject obj)
    {

        var itemOnMouse = player.mouseItem;
        var mouseHoverItem = itemOnMouse.hoverItem;
        var mouseHoverObj = itemOnMouse.hoverObj;
        var GetItemObject = inventory.database.GetItem;

        if (itemOnMouse.ui != null)
        {
            if (mouseHoverObj)
            {
                if (mouseHoverItem.CanPlaceInSlot(GetItemObject[itemDisplayed[obj].ID]) &&
                (mouseHoverItem.item.Id <= -1 ||
                (mouseHoverItem.item.Id >= 0 && itemDisplayed[obj].CanPlaceInSlot(GetItemObject[mouseHoverItem.item.Id]))))
                {
                    inventory.MoveItem(itemDisplayed[obj], mouseHoverItem.parent.itemDisplayed[itemOnMouse.hoverObj]);
                }

            }
        }
        else
        {
            inventory.RemoveItem(itemDisplayed[obj].item);
        }

        Destroy(itemOnMouse.obj);
        itemOnMouse.item = null;
    }

    public void OnDrag(GameObject obj)
    {
        if (player.mouseItem.obj != null)
        {
            player.mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

}

public class MouseItem
{
    public UserInterface ui;
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;

}

