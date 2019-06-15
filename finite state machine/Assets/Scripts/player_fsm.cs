using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_fsm : MonoBehaviour
{

    //private static bool isDone;
    private int Done;
    private List<GameObject> machineDone;
    private machine_fsm machine;

    private int useTime;
    private int rand;
    private GameObject machineHitByRay;

    private Camera mainCam;
    private bool doingSomething = false;

    //private int use = 2;


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
        StartCoroutine(CurrentState());

        machineDone = new List<GameObject>();

        // References the camera
        mainCam = Camera.main;

        state = PlayerState.Standby;
        // Makes sure the list is completely clear
        // machineDone.Clear();
    }

    private void Update()
    {
        // Checks if the player is done
        isDone();

        // Runs every frame, checks if the player has clicked, and if he did, return what he clicked.
        machineHitByRay = machineHit();
        
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
        //Debug.Log("Stand By Called");
        // This can be a coroutine where the player can stay afk until he decides to click on something then it exits the coroutine.
        // Movement code to move to standby area

        if (Input.GetMouseButtonDown(0) && machineHitByRay)
        {
            Debug.Log(machineHitByRay);
            state = PlayerState.InUse;
        }
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
        Debug.Log("pInUse is called");
        machine = machineSelected.GetComponent<machine_fsm>();

        // Checks if machine contains the script to interact with
        if (machine)
        {
            // Checks if the machine is in use.
            if (machine.CheckState() == machine_fsm.MachineState.InUse)
            {
                Debug.Log("Machine is currently in use, you can't use it now");
                // The state transition
                state = PlayerState.Standby;         

            }
            else
            {
                // The if statement should be outside the InUse Check
                if (machine.alreadyUsed) // I need to use a list instead
                {
                    // Player cant use and game tells the player so
                    Debug.Log("You can't use this as you have done this before");
                    state = PlayerState.Standby;
                }
                else
                {
                    Debug.Log("Sending message to machine script");
                    machine.UseMachine();
                    machineDone.Add(machineSelected);
                    
                    state = PlayerState.UseMachine;
                }
               
            }

        }
    }

    public void pUseMachine()
    {
        if(doingSomething == false)
        {
            StartCoroutine(PlayerStartUsage());
            doingSomething = true;
        }
        
        rand = Random.Range(1, 11);

        if (useTime >= 8)
        {
            if (false) //rand <= 1) // Chance ~ 10% to get injured
            {
                state = PlayerState.Injury;
            }
            else if (false) //rand <= 4) // Chance ~ 30% to get tired
            {
                state = PlayerState.Tired;
            }
            else
            {
                machineHitByRay = null;
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
        if (Input.GetMouseButtonDown(0))
        {
            // Changes: Cached the variable and RaycastHit hit is now declared outside the function
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            //Debug.Log("Mouse was clicked");

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //Debug.Log("Ray was cast");
                if (hit.collider != null)
                {
                    machineHitByRay = hit.collider.gameObject; 
                    //Debug.Log("There's a hit! This was the result: " + machineHitByRay);
                }
            }
        }
        return machineHitByRay;
    }

    IEnumerator PlayerStartUsage()
    {
        // This function is here to force the whole script to wait for the MachineInUse() coroutine to run its entire course
        yield return StartCoroutine(MachineInUse());
    }

    IEnumerator MachineInUse()
    {
        // Runs forever
        for (; ; )
        {
            if (useTime < 8)
            {
                // if usage time is less than 8, add 1 to it, then wait for 1 second
                useTime += 1;
                Debug.Log("Player script time: " + useTime);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                // This runs when useTime > 8
                Debug.Log("(PlayerScript)Machine usage is now done");
                // Resets the counter and then break the loop, effectively ending this coroutine
                doingSomething = false;
                useTime = 0;
                yield break;
            }
        }
    }


    IEnumerator CurrentState()
    {
        for (; ; )
        {
            Debug.Log(state);
            yield return new WaitForSeconds(1);
        }
    }
}
