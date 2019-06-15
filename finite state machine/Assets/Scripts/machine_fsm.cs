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

    MachineState state;
    private bool InUse;

    private int useTime = 0;
    private bool startUse;
    private int rand;

    public bool alreadyUsed;

    private void Start()
    {
        state = MachineState.Standby;
    }

    void Update()
    {
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
    //****************************************************
    // To be called by other classes
    public MachineState CheckState()
    {
        // Returns the current state
        return state;
    }

    public void UseMachine()
    {
        state = MachineState.InUse;
    }
    //****************************************************


    void mStandBy()
    {
        // the machine waits for player input
        // Debug.Log("Machine current state is stand by");
    }

    public void mInUse()
    {
        Debug.Log("Machine current state is InUse");
        // Starts the coroutine that waits for the MachineInUse to be completed, since the game requires the player to wait for 
        // the character to finish the machine, which basically is just 8 seconds of waiting.
        StartCoroutine(StartUsage());

        // A random number is generated to simulate chance
        rand = Random.Range(1, 11);

        alreadyUsed = true;

        // If the rand no. is less than or equals to 2 (so technically 20%), transition state to broken, else just go back to being 
        //on Standby for player input 
        if(false) //rand <= 2)
        {
            state = MachineState.Broken;
        }
        else
        {
            Debug.Log("Called");
            state = MachineState.Standby;
        }
    }

    void mBroken()
    {
        // Might use another coroutine here, need to wait for the worker to be here then it starts getting repaired.
        //if(worker_fsm is here)
        //{
        //    state = MachineState.Repair;
        //}
        //else
        //{
        //    Wait for worker
        //}
    }

    void mRepair()
    {
        // Pseudo-code: if worker is here, begin repair process (3 seconds). Once done, go back to standby. 
        // else wait for worker to be done.

        //if(worker_fsm is here)
        //{
        //    mRepair coroutine that is 3 seconds
        //}
        //else if(worker_fsm is done)
        //{
        //    state = MachineState.Standby;
        //}
        //else
        //{
        //    Wait for worker to be done
        //}
    }

    IEnumerator StartUsage()
    {
        // This function is here to force the whole script to wait for the MachineInUse() coroutine to run its entire course
        yield return StartCoroutine(MachineInUse());
    }

    IEnumerator MachineInUse()
    {
        for(; ; )
        {
            if (useTime < 8)
            {
                // if usage time is less than 8, add 1 to it, then wait for 1 second
                useTime += 1;
                Debug.Log("Machine Script time: " + useTime);
                yield return new WaitForSeconds(1);
            }
            else
            {
                // This runs when useTime > 8
                Debug.Log("(MachineScript)Machine usage is now done");
                useTime = 0;
                yield break;
            }
        }
    }
}
