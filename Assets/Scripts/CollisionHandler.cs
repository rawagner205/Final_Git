using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler
{
    PlayerController player;
    int inventorySize;

    public GameObject explosionEffect; 

    public CollisionHandler(PlayerController playerController, InventoryManager manager)
    {
        player = playerController;
        inventorySize = player.inventorySize;
        explosionEffect = player.explosionEffect;
    }

    public void CollectItem(Collider2D collision)
    {
        for (int i = 1; i <= inventorySize; i++)
        {
            if (player.inventoryManager.inventory[i] == null)
            {
                player.inventoryManager.inventory[i] = collision.gameObject.tag;
                GameObject.Destroy(collision.gameObject);
                player.uiManager.AddText();
                return;
            }
        }
        player.uiManager.AddTextErr();
    }

    public void ViewNonCollectible(Collider2D collision)
    {
        string message = player.itemList[collision.gameObject.tag].itemText;
        player.uiManager.PrintToScreen(message);
    }

    public void FinishLevel()
    {
        player.uiManager.PrintToScreen("Level Complete!");
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            player.uiManager.PrintToScreen("Game Complete!");
            Time.timeScale = 0;
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DestroyObstacle (Collision2D collision)
    {
        string collisionName = collision.gameObject.name;

        for (int i = 1; i <= inventorySize; i++)
        {
            if (player.inventoryManager.inventory[i] == player.lockList[collisionName])
            {
                GameObject.Instantiate(explosionEffect, collision.transform.position, collision.transform.rotation);
                GameObject.Destroy(collision.gameObject);
            }
        }
    }

}
