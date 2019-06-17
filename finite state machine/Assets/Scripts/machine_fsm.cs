using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class machine_fsm : MonoBehaviour
{
    public enum MachineState
    {
        Standby,
        InUse,
        Broken,
        Repair

    }
    MachineState state;                                 // The var containing the enum, and the "State" this FSM is in right now

    private bool doingSomething;                        // This is a boolean that determines if the script is "doing something or not"

    private int useTime = 0;                            // Variable integer that holds the usage time of the machine
    private int repairTime = 0;                         // Variable interger that holds the repair time of the machine
    private int rand;                                   // Contains a pseudo-random number to determine the next state after InUse
    private worker_fsm worker;                          // Reference to the Worker gameobject in the scene

    public bool player_used;                            // Determines if the player has this particular gameObject machine

    private void Start()
    {
        // Enter state
        state = MachineState.Standby;                   

        // Setting reference to the worker in the game scene
        worker = GameObject.Find("Worker").GetComponent<worker_fsm>();
    }

    void Update()
    {
        // Switch-Functions FSM style
        switch (state)
        {
            case MachineState.Standby:
                mStandBy();
                break;

            case MachineState.InUse:
                mInUse();
                break;

            case MachineState.Broken:
                mBroken();
                break;

            case MachineState.Repair:
                mRepair();
                break;

            default:
                Debug.Log("Machine Error - no state");
                break;
        }
    }

    // To be called by other classes
    public MachineState CheckState()
    {
        // Returns the current state
        return state;
    }

    void mStandBy()
    {
        // the machine waits for player input and is idle
        
    }

    public void mInUse()
    {
        // Checks if the script is doing something at the moment
        if (doingSomething == false)
        {
            // Start the MachineInUse coroutine
            StartCoroutine(StartUsage());
            // Sets doingSomething status of the script to true
            doingSomething = true;
        }

    }

    void mBroken()
    {
        // Checks if the script is doing something at the moment
        if (doingSomething == false)
        {
            // This is calling the worker over to the machine
            worker.RepairMachine(gameObject);
            doingSomething = true;
        }
    }

    void mRepair()
    {
        // Pseudo-code: if worker is here, begin repair process (3 seconds). Once done, go back to standby. 
        // else wait for worker to be done.
        // Checks if the script is doing something at the moment
        if (doingSomething == false)
        {
            //If worker is here, begin repair process (3 seconds). Once done, go back to standby. 
            StartCoroutine(StartRepair());
            doingSomething = true;
        }
    }

    // This method is called by the NPC class
    public void NPCUseMachine()
    {
        state = MachineState.InUse;
    }

    // This method is called by the Player class
    public void PlayerUseMachine()
    {
        // Sets this object to have "been used by the player"
        player_used = true;
        state = MachineState.InUse;
    }

    public void GetRepaired()
    {
        // Resets the script to not doing anything as earlier in mBroken(then to the worker class, and then back here to this method)
        // , it was waiting for the worker to come
        doingSomething = false;
        state = MachineState.Repair;
    }

    IEnumerator StartUsage()
    {
        // This function is here to force the whole script to wait for the MachineInUse() coroutine to run its entire course
        yield return StartCoroutine(MachineInUse());

        // A random number is generated to simulate chance
        rand = Random.Range(1, 11);

        // If the rand no. is less than or equals to 2 (so technically 20%), transition state to broken, else just go back to being 
        //on Standby for player input 
        if (rand <= 2 && worker.CheckState() == worker_fsm.WorkerState.Standby)
        {
            state = MachineState.Broken;
        }
        else
        {
            state = MachineState.Standby;
        }
    }

    IEnumerator MachineInUse()
    {
        for(; ; )
        {
            if (useTime < 8)
            {
                // if usage time is less than 8, add 1 to it, then wait for 1 second
                useTime += 1;
                // Wait for the specified number of seconds then proceed
                yield return new WaitForSeconds(1);
            }
            else
            {
                // This runs when useTime > 8
                // Resets doingSomething to false and useTime back to 0
                doingSomething = false;
                useTime = 0;
                // breaks the for loop prematurely
                yield break;
            }
        }
    }

    IEnumerator StartRepair()
    {
        // This function is here to force the whole script to wait for the MachineInUse() coroutine to run its entire course
        yield return StartCoroutine(RepairMachine());

        state = MachineState.Standby;
    }

    IEnumerator RepairMachine()
    {
        // Infinite loop
        for (; ; )
        {
            // Counts the length of time spent on repairing, if more than 3, the machine is now repaired
            if (repairTime < 3)
            {
                repairTime += 1;
                yield return new WaitForSeconds(1);
            }

            else
            {
                // Resets repair time to 0
                repairTime = 0;
                // Breaks the loop
                yield break;
            }
        }
    }

}
