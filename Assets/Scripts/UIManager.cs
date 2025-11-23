using UnityEngine;
using UnityEngine.UIElements;

public class UIManager
{
    public UIDocument uiDocument;
    public Label uiConsole;

    public UIManager(UIDocument newDocument)
    {
        uiDocument = newDocument;
        uiConsole = uiDocument.rootVisualElement.Q<Label>("UIConsole");
    }

    public void SlotText(int slot, string contents)
    {
        uiConsole.text = "slot " + slot + " : " + contents;
    }

    public void DropText(int slot)
    {
        uiConsole.text = "item dropped from slot " + slot;
    }

    public void DropTextErr()
    {
        uiConsole.text = "slot already empty";
    }

    public void AddText()
    {
        uiConsole.text = "added to inventory";
    }

    public void AddTextErr()
    {
        uiConsole.text = "inventory full!";
    }

    public void ActivateText(int slot)
    {
        uiConsole.text = "item activated from slot " + slot;
    }

    public void PrintToScreen(string text)
    {
        uiConsole.text = text;
    }
}
