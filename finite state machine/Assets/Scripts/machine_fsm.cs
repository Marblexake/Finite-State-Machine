using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class machine_fsm : MonoBehaviour
{
    private bool InUse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CheckState()
    {

    }

    void mStandBy()
    {

    }

    // Needs more work, but it returns if the machine is in use
    public bool mInUse()
    {
        // Counts down 8 seconds as a form of using the machine
        return InUse;
    }

    void mBroken()
    {

    }

    void mRepair()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
