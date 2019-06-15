using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_fsm : MonoBehaviour
{

    //private static bool isDone;
    private int Done;
    private List<GameObject> machineDone;
    public machine_fsm machine;

    private int useTime;
    private int rand;
    private GameObject machineHitByRay;
    private Vector3 raypos;
    private Camera mainCam;
    private RaycastHit hit;


   public enum PlayerState
    {
        Standby,
        InUse,
        UseMachine,
        Tired,
        GetFood,
        Eat,
        Injury,
        FirstAid,
        Hospital,
        Exit
    }

    static PlayerState state;

    // Start is called before the first frame update
    void Start()
    {
        // References the camera
        mainCam = Camera.main;

        // Makes sure the list is completely clear
        machineDone.Clear();
    }

    private void Update()
    {
        // Checks if the player is done
        isDone();

        // Runs every frame, checks if the player has clicked, and if he did, return what he clicked.
        machineHit();
        
        switch (state)
        {
            case PlayerState.Standby:
                pStandBy();
                break;

            case PlayerState.InUse:
                pInUse(machineHitByRay);
                break;

            case PlayerState.UseMachine:
                pUseMachine();
                break;

            case PlayerState.Tired:
                pTired();
                break;

            case PlayerState.GetFood:
                pGetFood();
                break;

            case PlayerState.Eat:
                pEat();
                break;

            case PlayerState.Injury:
                pInjury();
                break;

            case PlayerState.FirstAid:
                pFirstAid();
                break;

            case PlayerState.Hospital:
                pHospital();
                break;

            case PlayerState.Exit:
                break;

            default:
                pStandBy();
                Debug.Log("Player Error, No state");
                break;
        }
    }

    public PlayerState CheckState()
    {
        return state;
    }

    void pStandBy()
    {
        // This can be a coroutine where the player can stay afk until he decides to click on something then it exits the coroutine.
        // Movement code to move to standby area
        //pInUse();

        // Code that makes the player move around the area randomly
    }

    void isDone()
    {
        
        //Done = machineDone.Count;

        if(Done == 8)
        {
            state = PlayerState.Exit;
        }
        
    }

    void pInUse(GameObject machineSelected)
    {
        machine = machineSelected.GetComponent<machine_fsm>();
        // Checks if machine contains the script to interact with
        if (machine)
        {
            // Checks if the machine is in use.
            if (machine.CheckState() == machine_fsm.MachineState.InUse)
            {
                // Player cant use and game tells the player so

                // Add the selected machine to the list of machines the player has done
                machineDone.Add(machineSelected);

                // The state transition
                state = PlayerState.Standby;
                
            }
            else
            {
                // State Transition
                state = PlayerState.UseMachine;
                
            }
        }
    }

    void pUseMachine()
    {
        StartCoroutine(StartUsage());

        rand = Random.Range(1, 11);

        if (useTime >= 8)
        {
            if (rand <= 1) // Chance ~ 10% to get injured
            {
                state = PlayerState.Injury;
            }
            else if (rand <= 4) // Chance ~ 30% to get tired
            {
                state = PlayerState.Tired;
            }
            else
            {
                state = PlayerState.Standby;
            }
        }
    }

    // Tired
    void pTired()
    {
        //Player is now tired it tells the player it is now tired
        // Movement code to move to the standby area
        state = PlayerState.GetFood;
    }

    void pGetFood()
    {
        // Player has to click on the vending machine to get the food.
        // Movement code
        state = PlayerState.Eat;
    }

    void pEat()
    {
        // Character eats the food
        // Nom nom words, and UI 
        state = PlayerState.Standby;
    }

    // Injured
    void pInjury()
    {
        //Use the pain spray
        rand = Random.Range(1, 11);
        if (rand <= 2 )
        {
            state = PlayerState.Hospital;
        }
        else
        {
            state = PlayerState.FirstAid;
        }
    }

    void pFirstAid()
    {
        // Use the pain spray, a ui pops to prompt the player to use the pain spray
        state = PlayerState.Standby;
    }

    void pHospital()
    {
        // Game fades to black, and reappears after 10 seconds (coroutine?)
        state = PlayerState.Standby;
    }

    void pExit()
    {
        //Player is done and moves to the exit.
    }

    private GameObject machineHit()
    {
        // Changes: Cached the variable and renamed it to be more obvious what it contains
        machineHitByRay = null;

        if (Input.GetMouseButtonDown(0))
        {
            // Changes: Cached the variable and RaycastHit hit is now declared outside the function
            raypos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            if (Physics.Raycast(raypos, Vector3.forward, out hit, Mathf.Infinity))
            {
                if (hit.collider != null)
                {
                    machineHitByRay = hit.collider.gameObject; //returns the frame hit

                }
            }
        }
        return machineHitByRay;
    }

    IEnumerator StartUsage()
    {
        // This function is here to force the whole script to wait for the MachineInUse() coroutine to run its entire course
        yield return StartCoroutine(MachineInUse());
    }

    IEnumerator MachineInUse()
    {
        for (; ; )
        {
            if (useTime < 8)
            {
                // if usage time is less than 8, add 1 to it, then wait for 1 second
                useTime += 1;
                yield return new WaitForSeconds(1f);
            }
            else
            {
                // This runs when useTime > 8
                yield break;
            }
        }
    }
}
