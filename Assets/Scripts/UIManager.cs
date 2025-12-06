using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class UIManager
{
    public UIDocument uiDocument;
    public Label uiConsole;

    public Button restartButton;

    public UIManager(UIDocument newDocument)
    {
        uiDocument = newDocument;
        uiConsole = uiDocument.rootVisualElement.Q<Label>("UIConsole");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");

        restartButton.clicked += RestartGame;
        HideRestartButton();
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

    public void DisplayRestartButton()
    {
        restartButton.style.display = DisplayStyle.Flex;
    }

    public void HideRestartButton()
    {
        restartButton.style.display = DisplayStyle.None;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        HideRestartButton();
        Time.timeScale = 1;
    }
}

