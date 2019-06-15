using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worker_fsm : MonoBehaviour
{

    public enum WorkerState
    {
        Standby,
        needRepair,
        Moving,
        Repair
    }

    WorkerState state;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        switch (state)
        {
            case WorkerState.Standby:
                wStandby();
                break;

            case WorkerState.needRepair:
                wNeedRepair();
                break;

            case WorkerState.Moving:
                wMoving();
                break;

            case WorkerState.Repair:
                wRepair();
                break;

            default:
                Debug.Log("Worker Error, no state selected!");
                break;
        }
    }

    public WorkerState CheckState()
    {
        return state;
    }

    public void wStandby()
    {

    }

    public void wNeedRepair()
    {
        // Charts a path from current position to the machine that needs repair
    }

    void wMoving()
    {
        // Moving
    }

    public void wRepair()
    {

    }


}
