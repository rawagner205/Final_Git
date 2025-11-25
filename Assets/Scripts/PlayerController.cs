using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static ItemController;
using UnityEngine.UIElements;
using System;
using Unity.VisualScripting;
using static UIManager;
using System.ComponentModel;
using NUnit.Framework;

public class PlayerController : MonoBehaviour
{

    // Starting speed of player movement
    [SerializeField] public float forwardSpeed = 1f;
    [SerializeField] public int jumpHeight = 7;
    float movement = 0f;

    UIManager uiManager;
    [SerializeField] UIDocument uiDocument;

    //Item inventory
    Dictionary<int, string> inventory = new Dictionary<int, string>();
    [SerializeField] float inventorySize = 3;

    int slot = 1;

    Rigidbody2D rb;

    Animator walk;

    public GameObject explosionEffect; 

    //list of all possible items in the game
    public Dictionary<string, Item> itemList = new Dictionary<string, Item>();

    //list of all possible "locked" objects in the game (objects activated when player has correct item)
    //1st string is the name of the locked object, 2nd string is the name of the key object
    public Dictionary<string, string> lockList = new Dictionary<string, string>();

    //list of possible abilities to be activated, according to key given to item that can activate them
    public Dictionary<string, TrackingBool> abilityList = new Dictionary<string, TrackingBool>();


    //Allows for detection of collisions
    //Must match what these layers are set to in Unity
    [SerializeField] int groundLayer = 3;
    [SerializeField] int itemLayer = 6;
    [SerializeField] int obstacleLayer = 7;
    [SerializeField] int nonCollectibleLayer = 8;

    //variables for controlling ability to jump
    TrackingBool canJump = new TrackingBool(false);
    float jumpTimer = 0f;
    bool isTouchingGround;
    bool timerOn = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        walk = GetComponent<Animator>();

        uiManager = new UIManager(uiDocument);

        //set up empty inventory
        for (int i = 1; i <= inventorySize; i++)
        {
            inventory.Add(i, null);
        }

        //set up itemList
        Item NoteFile = new TextItem("NoteFile", "This is a text item.");
        itemList.Add(NoteFile.name, NoteFile);
        Item newKey = new KeyItem("newKey", "Try touching that triangle now.");
        itemList.Add(newKey.name, newKey);
        Item Terminal_1 = new NonCollectible ("Terminal_1", "Use 'V' to navigate inventory, 'C' to use item, 'X' to drop");
        itemList.Add(Terminal_1.name, Terminal_1);
        Item JumpTool = new AbilityItem("JumpTool", "Press spacebar to jump", "jump");
        itemList.Add(JumpTool.name, JumpTool);
        Item FinalKey = new KeyItem("FinalKey", "Go find a door!");
        itemList.Add(FinalKey.name, FinalKey);

        //set up lockList
        lockList.Add("evilTriangle", "newKey");
        lockList.Add("Door", "FinalKey");

        //set up abilityList
        abilityList.Add("jump", canJump);
    }

    void Update()
    {
        MovePlayer();
        UseItem();

        //jump functionality
        if (Keyboard.current.spaceKey.isPressed && canJump.trackedBool == true && isTouchingGround == true)
        {
            timerOn = true;
        }

        if (timerOn == true)
        {
            jumpTimer += Time.deltaTime;
        }   

        if (jumpTimer < .1 && timerOn == true && canJump.trackedBool == true)
            {
                isTouchingGround = false;
                rb.AddForce(new Vector2 (0, jumpHeight), ForceMode2D.Impulse);
            }
        
        if (isTouchingGround == true)
        {
            timerOn = false;
            jumpTimer = 0f;
        }

        
    }

    void MovePlayer()
    {
        //move character forward
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            movement += .1f;
            walk.enabled = true;
            walk.Play(Animator.StringToHash("WalkCycle"));
        }
        //move character backward
        else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            movement -= .1f;
            walk.enabled = true;
            walk.Play(Animator.StringToHash("WalkCycleBackwards"));
        }
        else
        {
            movement = 0f;
            walk.enabled = false;
        }

        //apply movement 
        float moveAmount = movement * forwardSpeed * Time.deltaTime;
        transform.Translate(moveAmount, 0, 0);
    }

    void UseItem()
    {

        //navigate inventory using 'v' key
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            if (slot < inventorySize)
            {
                slot += 1;
            }
            else
            {
                slot = 1;
            }
            uiManager.SlotText(slot, inventory[slot]);

        }

        //drop object from inventory using 'x' key
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            if (inventory[slot] != null)
            {
                //respawn object in level
                Vector3 objectLocation = new Vector3(0.0f, 0.0f, 0.0f);
                objectLocation.Set(transform.position.x -2, transform.position.y, transform.position.z);
                string currentObject = inventory[slot];
                Instantiate(Resources.Load(currentObject), objectLocation, transform.rotation);

                //deactivate ability if item is AbilityItem
                Item currentItem = itemList[inventory[slot]];
                if (currentItem.isAbility == true)
                {
                    abilityList[currentItem.targetAbility].trackedBool = false;
                }

                //reset inventory, display drop message
                inventory[slot] = null;
                uiManager.DropText(slot);
                slot = 1;
            }

            else
            {
                uiManager.DropTextErr();
            }
            
        }

        //use item from inventory using 'c' key
        if (Keyboard.current.cKey.wasPressedThisFrame && inventory[slot] != null)
        {
            Item currentItem = itemList[inventory[slot]];
            string itemText = currentItem.UseItem();

            //turn on ability if item is AbilityItem
            if (currentItem.isAbility == true)
            {
                abilityList[currentItem.targetAbility].trackedBool = true;
            }

            if (itemText != null)
            {
                uiManager.PrintToScreen(itemText);
            }
            else
            {
                uiManager.ActivateText(slot);
            }
            slot = 1;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == itemLayer)
        {

            for (int i = 1; i <= inventorySize; i++)
            {
                if (inventory[i] == null)
                {
                    inventory[i] = collision.gameObject.tag;
                    Destroy(collision.gameObject);
                    uiManager.AddText();
                    return;
                }
            }
            uiManager.AddTextErr();
        }
        else if (collision.gameObject.layer == nonCollectibleLayer)
        {
            string message = itemList[collision.gameObject.tag].itemText;
            uiManager.PrintToScreen(message);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            Time.timeScale = 0;
            uiManager.PrintToScreen("Level Complete!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == obstacleLayer)
        {
            string collisionName = collision.gameObject.name;

            for (int i = 1; i <= inventorySize; i++)
            {
                if (inventory[i] == lockList[collisionName])
                {
                    Instantiate(explosionEffect, collision.transform.position, collision.transform.rotation);
                    Destroy(collision.gameObject);
                }
            }
        }
        else if (collision.gameObject.layer == groundLayer)
        {
            isTouchingGround = true;
        }
    }

}


