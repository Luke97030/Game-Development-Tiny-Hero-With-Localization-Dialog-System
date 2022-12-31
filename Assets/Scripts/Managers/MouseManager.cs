using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// MouseManager will controller character movement, and a lot of other functions
public class MouseManager : Singleton<MouseManager>
{


    private RaycastHit hitDetail;

    // the different mouse cursor type
    public Texture2D regularCursor, targetCursor, doorCursor, mainMenuCursor, npcCursor;

    // creating an event for mouseClicked
    public event Action<Vector3> OnMouseClickedGround;
    // creating an event for mouse click on enemy
    // we want to get all information about Enemy object
    public event Action<GameObject> OnMouseClickedEnemy;

    protected override void Awake()
    {
        base.Awake();
        // adding some extra function on the base class Awake function
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetCursorTexture();
        MouseControl();   
    }

    // Set different CursorTexture depends on what player clicked
    private void SetCursorTexture()
    {
        // get the Ray from where you click the mouse  camera to 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(transform.position, forward, Color.green);

        if (Physics.Raycast(ray, out hitDetail))
        {
            if (hitDetail.collider.gameObject.CompareTag("Ground"))
            {
                Cursor.SetCursor(regularCursor, new Vector2(16, 20), CursorMode.Auto);
            }
            else if (hitDetail.collider.gameObject.CompareTag("Enemy"))
            {
                // when mouse click on the ground, return the point to the Navmash Agent
                // execute all the functions which were added to the OnMouseClickced event
                Cursor.SetCursor(targetCursor, new Vector2(16, 20), CursorMode.Auto);
            }
            else if (hitDetail.collider.gameObject.CompareTag("Portal"))
            {
                // when mouse click on the ground, return the point to the Navmash Agent
                // execute all the functions which were added to the OnMouseClickced event
                Cursor.SetCursor(doorCursor, new Vector2(16, 20), CursorMode.Auto);
            }
            else if (hitDetail.collider.gameObject.CompareTag("NPC"))
            {
                // when mouse click on the NPC,
                // execute all the functions which were added to the OnMouseClickced event
                Cursor.SetCursor(npcCursor, new Vector2(16, 20), CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(mainMenuCursor, new Vector2(16, 20), CursorMode.Auto);
            }
        }
    }

    private void MouseControl()
    {
        // when you click mouse on something with collider
        // only the mouse is not clicking on any game object which have collider
        if (Input.GetMouseButtonDown(0) && hitDetail.collider != null)
        {

                if (hitDetail.collider.gameObject.CompareTag("Ground"))
                {
                    // when mouse click on the ground, return the point to the Navmash Agent
                    // execute all the functions which were added to the OnMouseClickedGround event
                    OnMouseClickedGround?.Invoke(hitDetail.point);
                }
                if (hitDetail.collider.gameObject.CompareTag("Enemy"))
                {
                    // when mouse click on the enemyh, return gameObject of Enemy's collider
                    // execute all the functions which were added to the OnMouseClickedEnemy event
                    OnMouseClickedEnemy?.Invoke(hitDetail.collider.gameObject);
                }
                if (hitDetail.collider.gameObject.CompareTag("Attackable"))
                {
                    // when mouse click on the attackable object rock
                    OnMouseClickedEnemy?.Invoke(hitDetail.collider.gameObject);
                }
                if (hitDetail.collider.gameObject.CompareTag("Portal"))
                {
                    // when mouse click on the portal,
                    // execute all the functions which were added to the OnMouseClickedPortal event
                    OnMouseClickedGround?.Invoke(hitDetail.point);
                }
        }
    }
}
