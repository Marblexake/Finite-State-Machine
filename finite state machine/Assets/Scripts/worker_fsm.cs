using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class worker_fsm : MonoBehaviour
{

    public enum WorkerState
    {
        Standby,
        needRepair,
        Moving,
        Repair
    }

    WorkerState state;                              // The var containing the enum, and the "State" this FSM is in right now

    public NavMeshAgent WorkerAgent;                // Reference to the worker's NavMeshAgent
    public GameObject WorkerPos;                    // Reference to the position the worker has to go back to every time

    private int repairTime;                         // Variable that contains how long is spent repairing the machine

    private GameObject machineCalled;               // Reference to the machine that called a method in here
    private machine_fsm machineCalledScript;        // Reference to the machine_fsm script on the machine ^
    private GameObject machineCollided;             // Reference to the object collided with by this gameObject
    private bool doingSomething;                    // Boolean that determines if the script is doing anything at all
        
    private int once = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Testing purposes
        // StartCoroutine(CurrentState());

        // Enter state
        state = WorkerState.Standby;
    }

    void Update()
    {
        // Switch-Functions FSM style
        switch (state)
        {
            case WorkerState.Standby:
                wStandby();
                break;

            case WorkerState.needRepair:
                wNeedRepair();
                break;

            case WorkerState.Repair:
                wRepair();
                break;

            default:
                Debug.Log("Worker Error, no state selected!");
                break;
        }
    }


    // Returns the state of the worker
    public WorkerState CheckState()
    {
        return state;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // States:

    void wStandby()
    {
        // Worker is waiting for machine to call it
        if (transform.position != WorkerPos.transform.position)
        {
            WorkerAgent.SetDestination(WorkerPos.transform.position);
        }
    }

    void wNeedRepair()
    {

        state = WorkerState.Repair;

        // Charts a path from current position to the machine that needs repair
        WorkerAgent.SetDestination(machineCalled.transform.position);

    }

    void wRepair()
    {
        
        // Checks if the machine collided with is the one who called the worker over, and also if
        // the script is "doing something" right now
        if(machineCollided == machineCalled && doingSomething == false)
        {
            StartCoroutine(StartRepair());

            // Sets the script to be now "doing something"
            doingSomething = true;
        }

    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


    //**************************************************************
    // Methods used by script:

    // ACtually to be called by other scripts
    public void RepairMachine(GameObject machine)
    {
        // Reference to machine that called this method
        machineCalled = machine;
        // Reference to the script on the said machine
        machineCalledScript = machine.gameObject.GetComponent<machine_fsm>();
        state = WorkerState.needRepair;
    }

    // On collision, sets a reference to that collided object
    private void OnTriggerEnter(Collider other)
    {
        machineCollided = other.gameObject;

    }

    IEnumerator StartRepair()
    {
        // Gets the machine to begin repairing
        machineCalledScript.GetRepaired();

        Debug.Log("Worker is now repairing the machine");
        yield return StartCoroutine(RepairMachine());
        // ^^ This coroutine waits for the RepairMachine coroutine to finish before doing its tasks.

        doingSomething = false;
        state = WorkerState.Standby;
    }

    IEnumerator RepairMachine()
    {
        // Infinite loop
        for(; ; )
        {
            // Counts the length of time spent on repairing, if more than 3, the machine is now repaired
            if(repairTime < 3)
            {
                repairTime += 1;
                yield return new WaitForSeconds(1);
            }

            else
            {
                repairTime = 0;
                // Breaks the loop
                yield break;
            }
        }
    }

    // Testing purposes
    IEnumerator CurrentState()
    {
        for (; ; )
        {
            Debug.Log(state);
            yield return new WaitForSeconds(1);
        }
    }
}
