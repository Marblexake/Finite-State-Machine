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

    // Start is called before the first frame update
    void Start()
    {
        
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

    }

    void nChooseMachine()
    {

    }

    void nInUse()
    {

    }

    void nUseMachine()
    {

    }

    void IsDone()
    {

    }

    void Exit()
    {

    }
}
