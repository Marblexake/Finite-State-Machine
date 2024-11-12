using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class npc_fsm : MonoBehaviour
{
    public enum NPCState
    {
        Standby,
        ChooseMachine,
        InUse,
        UseMachine,
        Moving
    }

    NPCState state;                                     // The var containing the enum, and the "State" this FSM is in right now

    public NavMeshAgent NPC_Agent;                      // Reference to the NPC's NavMeshAgent
    public GameObject StandByArea;                      // Reference to the yellow mat in the middle of the game level
                    
    public List<GameObject> MachinesChoice;             // Reference to the list of machines the NPC can use

    private bool doingSomething;                        // This is a boolean that determines if the script is "doing something or not"

    // InUse variables
    private int rand;                                   // Contains a pseudo-random number to determine the next state
    private GameObject machineChosen;                   // Reference to the machine chosen randomly by the NPC
    private machine_fsm machineScript;                  // Reference to the script on said machine
    private GameObject machineCollided;                 // Reference to the machine this object has collided with

    private GameObject textObject; 
    public GameObject textPrefab;

    // Coroutines
    private int useTime;                                // Variable integer that holds the usage time of the machine

    // Start is called before the first frame update
    void Start()
    {
        // Testing purposes
        // StartCoroutine(CurrentState());

        textObject = Instantiate(textPrefab, transform.position, transform.rotation);

        // Enter state
        state = NPCState.Standby;
    }

    void Update()
    {
        Transform textTrans = textObject.GetComponent<Transform>();
        textTrans.position = transform.position;
        textTrans.Translate(Vector3.up * 1.0f);
        textTrans.Translate(Vector3.left * 1.5f);

        StateTextScript textScript = textObject.GetComponent<StateTextScript>();

        // Switch-Functions FSM style
        switch (state)
        {
            case NPCState.Moving:
                textScript.UpdateStateText("Current State: Moving");
                nUseMachine();
                break;
            case NPCState.Standby:
                nStandby();
                textScript.UpdateStateText("Current State: Standby");
                break;
            case NPCState.ChooseMachine:
                nChooseMachine();
                textScript.UpdateStateText("Current State: ChooseMachine");
                break;
            case NPCState.InUse:
                nInUse();
                textScript.UpdateStateText("Current State: InUse");
                break;
            case NPCState.UseMachine:
                textScript.UpdateStateText("Current State: UseMachine");
                break;
            default:
                Debug.Log("NPC Error - No State!");
                break;

        }
    }

    // To be called by other classes - returns the current state of the player
    public NPCState CheckState()
    {
        return state;
    }

    // Default idle state
    void nStandby()
    {
        // Checks if the script is doing something at the moment
        if (MachinesChoice.Count != 0)
        {
            state = NPCState.ChooseMachine;
        }
        else
        {
            // This triggers idle functions, where the character just moves around a set area randomly
            if (doingSomething == false)
            {
                StartCoroutine(StartIdle());
                doingSomething = true;
            }
            
        }

    }

    // Chooses the random machine this npc is going to do
    void nChooseMachine()
    {
        // Contains a random number
        rand = Random.Range(0, MachinesChoice.Count);

        // Random machine from index
        machineChosen = MachinesChoice[rand];

        state = NPCState.InUse;
    }

    void nInUse()
    {
        machineScript = machineChosen.GetComponent<machine_fsm>();


        // Checks if current state of machine is "InUse", if it is, npc can't use, else use the machine
        if (machineScript.CheckState() == machine_fsm.MachineState.InUse)
        {

            state = NPCState.Standby;
        }
        // Checks if the machine is under repair or broken
        else if (machineScript.CheckState() == machine_fsm.MachineState.Repair || machineScript.CheckState() == machine_fsm.MachineState.Broken)
        {
            state = NPCState.Standby;
        }
        else
        {
            // Moves the NPC to the machine to be used
            Moving(machineChosen);

            // State transition to useMachine state
            state = NPCState.Moving;
        }
    }

    void nUseMachine()
    {
        // Checks if the collided object is the same as the object hit by the ray, and also if script is doing anything
        if (machineCollided == machineChosen && doingSomething == false)
        {
            // Starts the NPC usage of the machine
            StartCoroutine(StartUsage());
            doingSomething = true;

            state = NPCState.UseMachine;
        }

    }

    // Moves NPC to the randomly chosen machine
    void Moving(GameObject machineSelected)
    {
        NPC_Agent.SetDestination(machineSelected.transform.position);
    }

    // Detects when the collider on the character has hit something and sets a reference to it
    private void OnTriggerEnter(Collider other)
    {
        machineCollided = other.gameObject;

    }

    IEnumerator StartUsage()
    {
        // Removes the machine the NPC used
        MachinesChoice.Remove(machineChosen);

        // Calls the NPC's method in machine_fsm script
        machineScript.NPCUseMachine();

        // This function is here to force the whole script to wait for the MachineInUse() coroutine to run its entire course
        yield return StartCoroutine(MachineInUse());


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
                yield return new WaitForSeconds(2f);
            }
            else
            {
                // This runs when useTime > 8
                // Resets the counter and then break the loop, effectively ending this coroutine 
                doingSomething = false;
                useTime = 0;
                yield break;
            }
        }
    }

    IEnumerator StartIdle()
    {
        // This function is here to force the whole script to wait for the MachineInUse() coroutine to run its entire course
        yield return StartCoroutine(Idle());

        doingSomething = false;

    }

    IEnumerator Idle()
    {
        for (; ; )
        {
            // These code causes the character to move to random points on the stand by area every 3 seconds
            NPC_Agent.SetDestination(StandByArea.transform.position + new Vector3(Random.Range(-2, 3), Random.Range(-2, 3), Random.Range(-2, 3)));
            yield return new WaitForSeconds(3);
            yield break;
        }
    }

    // Testing purposes
    IEnumerator CurrentState()
    {
        for (; ; )
        {
            Debug.Log("NPC state: " + state);
            yield return new WaitForSeconds(1);
        }
    }
}
