using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using static PlayerController;
public abstract class Item
{
    public string name { get; set; } = "name";
    public string itemText { get; set; } = "item description";

    public bool isAbility {get; set; } = false;
    public bool isMotion {get; set; } = false;
    public string targetAbility {get; set;} = null;

    public Item(string itemName, string newItemText)
    {
        name = itemName;
        itemText = newItemText;

    }

    abstract public string UseItem();
}

public class TextItem : Item
{
    public TextItem(string itemName, string newItemText) : base(itemName, newItemText)
    {
    }

    override public string UseItem()
    {
        return itemText;
    }

}

public class KeyItem : Item
{
    public KeyItem(string itemName, string newItemText) : base(itemName, newItemText)
    {
    }

    override public string UseItem()
    {
        return itemText;
    }

}

public class NonCollectible : Item
{
    public NonCollectible(string itemName, string newItemText) : base(itemName, newItemText)
    {
    }

    public override string UseItem()
    {
        return itemText;
    }
}

public class AbilityItem : Item
{
    public AbilityItem(string itemName, string newItemText, string target) : base(itemName, newItemText)
    {
        isAbility = true;
        targetAbility = target;
    }

    public override string UseItem()
    {
        return itemText;
    }


}

public class MotionItem : Item
{
    public MotionItem(string itemName, string newItemText) : base(itemName, newItemText)
    {
        isMotion = true;
    }

    public override string UseItem()
    {
        return itemText;
    }
}

public class TrackingBool
{
    public bool trackedBool {get; set; }

    public TrackingBool(bool newBool)
    {
        trackedBool = newBool;
    }
    
}




