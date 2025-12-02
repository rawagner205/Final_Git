using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static ItemController;
using static PlayerMovement;
using static InventoryManager;
using static CollisionHandler;
using UnityEngine.UIElements;
using System;
using Unity.VisualScripting;
using static UIManager;
using System.ComponentModel;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using NUnit.Framework.Constraints;

public class PlayerController : MonoBehaviour
{

    // Starting speed of player movement
    [SerializeField] public float forwardSpeed = 1f;
    [SerializeField] public float jumpHeight = .7f;

    public UIManager uiManager;
    [SerializeField] UIDocument uiDocument;


    [SerializeField] public int inventorySize = 3;

    Rigidbody2D rb;

    [SerializeField] public GameObject explosionEffect;

    Animator walk;
    PlayerMovement moveControl;
    SpriteRenderer sprite;
    public InventoryManager inventoryManager;

    CollisionHandler collisionHandler;

    //list of all possible items in the game
    public Dictionary<string, Item> itemList = new Dictionary<string, Item>();

    //list of all possible "locked" objects in the game (objects activated when player has correct item)
    //1st string is the name of the locked object, 2nd string is the name of the key object
    public Dictionary<string, string> lockList = new Dictionary<string, string>();

    //list of possible abilities to be activated, according to key given to item that can activate them
    public Dictionary<string, TrackingBool> abilityList = new Dictionary<string, TrackingBool>();

    //list of possible items that can trigger motion in environment - 1st string is item name, 2nd string is name of object to move
    public Dictionary<string, string> motionList = new Dictionary<string, string>();


    //Allows for detection of collisions
    //Must match what these layers are set to in Unity
    [SerializeField] int groundLayer = 3;
    [SerializeField] int itemLayer = 6;
    [SerializeField] int obstacleLayer = 7;
    [SerializeField] int nonCollectibleLayer = 8;

    //variables for controlling ability to jump
    TrackingBool canJump = new TrackingBool(false);
    TrackingBool canTallJump = new TrackingBool(false);
    float jumpTimer = 0f;
    bool isTouchingGround;
    bool timerOn = false;

    TrackingBool isGravityFlipped = new TrackingBool(false);


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        walk = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();


        uiManager = new UIManager(uiDocument);
        moveControl = new PlayerMovement(walk, forwardSpeed, jumpHeight);
        inventoryManager = new InventoryManager(this);
        collisionHandler = new CollisionHandler(this, inventoryManager);

        //set up itemList
        Item NoteFile = new TextItem("NoteFile", "This is a text item.");
        itemList.Add(NoteFile.name, NoteFile);
        Item Hammer = new KeyItem("Hammer", "Look for something yellow...");
        itemList.Add(Hammer.name, Hammer);
        Item Terminal_1 = new NonCollectible ("Terminal_1", "Use 'V' to navigate inventory, 'C' to use item, 'X' to drop");
        itemList.Add(Terminal_1.name, Terminal_1);
        Item Spring = new AbilityItem("Spring", "Press spacebar to jump", "jump");
        itemList.Add(Spring.name, Spring);
        Item FinalKey = new KeyItem("FinalKey", "Go find a door!");
        itemList.Add(FinalKey.name, FinalKey);
        Item TrapdoorKey = new KeyItem("TrapdoorKey", "This door might be a little different...");
        itemList.Add(TrapdoorKey.name, TrapdoorKey);
        Item ExtraSprings = new AbilityItem("ExtraSprings", "Bet you could do something with these...", "tall jump");
        itemList.Add(ExtraSprings.name, ExtraSprings);
        Item Terminal_2 = new NonCollectible("Terminal_2", "Look at you, doing things the hard way!");
        itemList.Add(Terminal_2.name, Terminal_2);
        Item RemoteControl = new MotionItem("RemoteControl", "Check this out!");
        itemList.Add(RemoteControl.name, RemoteControl);
        Item AntigravBooster = new AbilityItem("AntigravBooster", "Hold on to your hat!","antigravity");
        itemList.Add(AntigravBooster.name, AntigravBooster);
        Item ExitKey = new KeyItem("ExitKey", "I hope you know what this is for by now.");
        itemList.Add(ExitKey.name, ExitKey);

        //set up lockList
        lockList.Add("evilTriangle", "Hammer");
        lockList.Add("Door", "FinalKey");
        lockList.Add("Trapdoor", "TrapdoorKey");
        lockList.Add("ExitDoor", "ExitKey");
        lockList.Add("Barricade", "Hammer");

        //set up abilityList
        abilityList.Add("jump", canJump);
        abilityList.Add("tall jump", canTallJump);
        abilityList.Add("antigravity", isGravityFlipped);

        //set up motionList
        motionList.Add("RemoteControl", "Plank");
    }

    void Update()
    {
        moveControl.MovePlayer(this);
        inventoryManager.UseInventory();

        //jump functionality
        if (Keyboard.current.spaceKey.wasPressedThisFrame && canJump.trackedBool == true && isTouchingGround == true)
        {
            timerOn = true;
        }

        if (timerOn == true)
        {
            jumpTimer += Time.deltaTime;
        }   

        if (jumpTimer < .05 && timerOn == true && canJump.trackedBool == true)
        {
            moveControl.Jump(rb, canTallJump);
            isTouchingGround = false;
        }
        
        if (isTouchingGround == true)
        {
            timerOn = false;
            jumpTimer = 0f;
        }

        //antigravity
        if (isGravityFlipped.trackedBool == true)
        {
            rb.gravityScale = -3;
            sprite.flipY = true;
        }
        else
        {
            rb.gravityScale = 3;
            sprite.flipY = false;
        }
        
    }
  

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == itemLayer)
        {
            collisionHandler.CollectItem(collision);
        }
        else if (collision.gameObject.layer == nonCollectibleLayer)
        {
            collisionHandler.ViewNonCollectible(collision);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            collisionHandler.FinishLevel();
        }
        else if (collision.gameObject.tag == "FloorCollider")
        {
            collisionHandler.ResetLevel();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == obstacleLayer)
        {
            collisionHandler.DestroyObstacle(collision);
        }
        else if (collision.gameObject.layer == groundLayer)
        {
            isTouchingGround = true;
        }
    }

}


