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

    
    private List<GameObject> machinesDone;
    public GameObject[] MachinesChoice;

    private bool doingSomething;

    // InUse variables
    private int rand;
    private GameObject machineChosen;
    private bool Used;
    private machine_fsm machineScript;

    // Coroutines
    private int useTime;

    // Start is called before the first frame update
    void Start()
    {
        machinesDone = new List<GameObject>();

        StartCoroutine(CurrentState());
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
        rand = Random.Range(0, 8);

        machineChosen = MachinesChoice[rand];
        Debug.Log("NPC has chosen " + machineChosen);

        state = NPCState.InUse;
    }

    void nInUse()
    {
        for(int i = 0; i <= machinesDone.Count; i++)
        {
            if(machinesDone.Count == 0)
            {
                break;
            }
            else if (machineChosen == machinesDone[i])
            {
                Debug.Log("Machine has been used by NPC once");
                Used = true;
                break;
            }
        }

        machineScript = machineChosen.GetComponent<machine_fsm>();

        if (!Used)
        {
            if (machineScript.CheckState() == machine_fsm.MachineState.InUse)
            {
                Debug.Log("NPC can't use machine, it is in use");

                state = NPCState.Standby;
            }
            else
            {
                machineScript.UseMachine();
                machinesDone.Add(machineChosen);

                Used = false;
                state = NPCState.UseMachine;
            }

        }
        else
        {
            Used = false;
            state = NPCState.Standby;
        }
    }

    void nUseMachine()
    {
        Debug.Log("NPC is now using machine");
        
        if(doingSomething == false)
        {
            StartCoroutine(StartUsage());
            doingSomething = true;
        }

        state = NPCState.Standby;
    }

    void Exit()
    {
        Debug.Log("NPC is exiting");
        // Go to the exit of the gym and destroy itself
    }

    bool IsDone()
    {
        if(machinesDone.Count > 2)
        {
            return true;
        }

        return false;
    }

    IEnumerator StartUsage()
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
                Debug.Log("NPC script time: " + useTime);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                // This runs when useTime > 8
                Debug.Log("NPC Machine usage is now done");
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
