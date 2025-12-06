using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Unity.VisualScripting;

public class InventoryManager
{
    PlayerController player;
    public int slot = 1;
    public int inventorySize;

    //the player's Item inventory
    public Dictionary<int, string> inventory = new Dictionary<int, string>();
    public InventoryManager(PlayerController playerController)
    {
        player = playerController;
        inventorySize = player.inventorySize;

        //set up empty inventory
        for (int i = 1; i <= inventorySize; i++)
        {
            inventory.Add(i, null);
        }
    }
    
    public void UseInventory()
    {
        //navigate inventory using 'v' key
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            NavigateSlots();
        }

        //drop object from inventory using 'x' key
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            DropItem();
        }

        //use item from inventory using 'c' key
        if (Keyboard.current.cKey.wasPressedThisFrame && inventory[slot] != null)
        {
            ActivateItem();
        }
    }

    public bool isFull()
    {
        for (int i = 1; i <= inventorySize; i++)
        {
            if (inventory[i] == null)
            {
                return false;
            }
        }
        
        return true;
    }

    void NavigateSlots()
    {
        if (slot < inventorySize)
        {
            slot += 1;
        }
        //if end of inventory is reached, wrap around back to first slot
        else
        {
            slot = 1;
        }
        player.uiManager.SlotText(slot, inventory[slot]);
    }

    void DropItem()
    {
        if (inventory[slot] != null)
        {
            //respawn object in level
            Vector3 objectLocation = new Vector3(0.0f, 0.0f, 0.0f);
            objectLocation.Set(player.transform.position.x -2, player.transform.position.y, player.transform.position.z);
            string currentObject = inventory[slot];
            GameObject.Instantiate(Resources.Load(currentObject), objectLocation, player.transform.rotation);

            //deactivate ability if item is AbilityItem
            Item currentItem = player.itemList[inventory[slot]];
            if (currentItem.isAbility == true)
            {
                player.abilityList[currentItem.targetAbility].trackedBool = false;
            }

            //reset inventory, display drop message
            inventory[slot] = null;
            player.uiManager.DropText(slot);
            slot = 1;
        }

        else
        {
            player.uiManager.DropTextErr();
        }
    }

    void ActivateItem()
    {
        Item currentItem = player.itemList[inventory[slot]];
        string itemText = currentItem.UseItem();

        //turn on ability if item is AbilityItem
        if (currentItem.isAbility == true)
        {
            player.abilityList[currentItem.targetAbility].trackedBool = true;
        }

        //trigger motion if item is MotionItem
        else if (currentItem.isMotion == true)
        {
            string targetObject = player.motionList[currentItem.name];
            GameObject target = GameObject.Find(targetObject);
            target.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }

        if (itemText != null)
        {
            player.uiManager.PrintToScreen(itemText);
        }
        else
        {
            player.uiManager.ActivateText(slot);
        }
        
        slot = 1;
}
}
