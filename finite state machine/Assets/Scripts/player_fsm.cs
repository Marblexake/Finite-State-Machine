using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_fsm : MonoBehaviour
{

    //private static bool isDone;
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
        pStandby,
        pInUse,
        pUseMachine,
        pTired,
        pGetFood,
        pEat,
        pInjury,
        pFirstAid,
        pHospital,
        pExit
    }

    static PlayerState state;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // Checks if the player is done
        isDone();

        machineHit();
        
        switch (state)
        {
            case PlayerState.pStandby:
                pStandBy();
                break;

            case PlayerState.pInUse:
                pInUse(machineHitByRay);
                break;

            case PlayerState.pUseMachine:
                pUseMachine();
                break;

            case PlayerState.pTired:
                pTired();
                break;

            case PlayerState.pGetFood:
                pGetFood();
                break;

            case PlayerState.pEat:
                pEat();
                break;

            case PlayerState.pInjury:
                pInjury();
                break;

            case PlayerState.pFirstAid:
                pFirstAid();
                break;

            case PlayerState.pHospital:
                pHospital();
                break;

            case PlayerState.pExit:
                break;

            default:
                pStandBy();
                Debug.Log("No state");
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
        if(Done == 8)
        {
            state = PlayerState.pExit;
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
                state = PlayerState.pStandby;
                
            }
            else
            {
                //Player can use the machine
                state = PlayerState.pUseMachine;
                
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
                state = PlayerState.pInjury;
            }
            else if (rand <= 4) // Chance ~ 30% to get tired
            {
                state = PlayerState.pTired;
            }
            else
            {
                state = PlayerState.pStandby;
            }
        }
    }

    // Tired
    void pTired()
    {
        //Player is now tired it tells the player it is now tired
        // Movement code to move to the standby area
        state = PlayerState.pGetFood;
    }

    void pGetFood()
    {
        // Player has to click on the vending machine to get the food.
        // Movement code
        state = PlayerState.pEat;
    }

    void pEat()
    {
        // Character eats the food
        // Nom nom words, and UI 
        state = PlayerState.pStandby;
    }

    // Injured
    void pInjury()
    {
        //Use the pain spray
        rand = Random.Range(1, 11);
        if (rand <= 2 )
        {
            state = PlayerState.pHospital;
        }
        else
        {
            state = PlayerState.pFirstAid;
        }
    }

    void pFirstAid()
    {
        // Use the pain spray, a ui pops to prompt the player to use the pain spray
        state = PlayerState.pStandby;
    }

    void pHospital()
    {
        // Game fades to black, and reappears after 10 seconds (coroutine?)
        state = PlayerState.pStandby;
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
