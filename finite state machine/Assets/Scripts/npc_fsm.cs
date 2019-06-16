using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_fsm : MonoBehaviour
{
    public enum NPCState
    {
        Standby,
        ChooseMachine,
        InUse,
        UseMachine,
        Exit
    }

    NPCState state;

    //testing
    private int elapsedTime = 0;
    private int mention = 0;

    private int machinesDone = 0;
    public List<GameObject> MachinesChoice;

    private bool doingSomething;

    // InUse variables
    private int rand;
    private GameObject machineChosen;
    private machine_fsm machineScript;

    // Coroutines
    private int useTime;

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(CurrentState());
    }

    void Update()
    {
        switch (state)
        {
            case NPCState.Standby:
                nStandby();
                break;
            case NPCState.ChooseMachine:
                nChooseMachine();
                break;
            case NPCState.InUse:
                nInUse();
                break;
            case NPCState.UseMachine:
                nUseMachine();
                break;
            case NPCState.Exit:
                Exit();
                break;
            default:
                Debug.Log("NPC Error - No State!");
                break;

        }
    }

    public NPCState CheckState()
    {
        return state;
    }

    void nStandby()
    {
        if (IsDone())
        {
            state = NPCState.Exit;
        }
        else
        {
            state = NPCState.ChooseMachine;
        }

    }

    void nChooseMachine()
    {
        Debug.Log("NPC is choosing machine...");
        rand = Random.Range(0, MachinesChoice.Count);

        machineChosen = MachinesChoice[rand];
        Debug.Log("NPC has chosen " + machineChosen);

        state = NPCState.InUse;
    }

    void nInUse()
    {
        machineScript = machineChosen.GetComponent<machine_fsm>();

        // Checks if current state of machine is "InUse", if it is, npc can't use, else use the machine
        if (machineScript.CheckState() == machine_fsm.MachineState.InUse)
        {
            Debug.Log("NPC can't use machine, it is in use");

            state = NPCState.Standby;
        }
        else
        {
            Debug.Log("Machines done by npc " + machinesDone);
            machinesDone += 1;

            // Calls the NPC's method in machine_fsm script
            machineScript.NPCUseMachine();

            // removes the machine from the list of possible machines to use
            Debug.Log("removing machine from list");
            MachinesChoice.Remove(machineChosen);

            // State transition to useMachine state
            state = NPCState.UseMachine;
        }
    }

    void nUseMachine()
    {
        //Debug.Log("npc UseMachine: " + doingSomething + " and useTime: " + useTime);
        //if (doingSomething == false && useTime < 8)
        //{
        //    Debug.Log("NPC is now using machine " + "doingSomething is: " + doingSomething);
        //    StartCoroutine(StartUsage());
        //    doingSomething = true;
        //}

        //if (useTime >= 8)
        //{
        //    state = NPCState.Standby;
        //}
        if (doingSomething == false)
        {
            StartCoroutine(StartUsage());
            doingSomething = true;
        }

    }

    void Exit()
    {
        if(mention == 0)
        {
            Debug.Log("NPC is exiting");
            mention += 1;
        }
        
        // Go to the exit of the gym and destroy itself
    }

    bool IsDone()
    {
        if (machinesDone > 2)
        {
            return true;
        }

        return false;
    }

    IEnumerator StartUsage()
    {
        // This function is here to force the whole script to wait for the MachineInUse() coroutine to run its entire course
        yield return StartCoroutine(MachineInUse());

        Debug.Log("StartUsage() useTime: " + useTime);
        
        state = NPCState.Standby;

    }

    IEnumerator MachineInUse()
    {
        // Runs forever
        for (; ; )
        {
            if (useTime < 8)
            {
                // if usage time is less than 8, add 1 to it, then wait for 1 second
                useTime += 2;
                Debug.Log("NPC script time: " + useTime);
                yield return new WaitForSeconds(2f);
            }
            else
            {
                // This runs when useTime > 8
                Debug.Log("NPC Machine usage is now done, useTime: " + useTime);
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
            //elapsedTime += 1;
            //Debug.Log(elapsedTime);
            Debug.Log("NPC state: " + state);
            yield return new WaitForSeconds(1);
        }
    }
}
