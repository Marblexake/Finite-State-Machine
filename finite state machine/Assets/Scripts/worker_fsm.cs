using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worker_fsm : MonoBehaviour
{

    enum WorkerState
    {
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
        
    }

    public void CheckState()
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
