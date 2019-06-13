using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_fsm : MonoBehaviour
{

    //private static bool isDone;
    private int Done;
    private List<GameObject> machineDone;
    public machine_fsm machine;

   public enum PlayerState
    {
        pStandby,
        pInUse,
        pUseMachine,
        pTired,
        pGetFood,
        pEat,
        pInjury,
        pFirstAid,
        pHospital,
        pExit
    }

    static PlayerState state;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        // Checks if the player is done
        isDone();

        switch (state)
        {
            case PlayerState.pStandby:
                pStandBy();
                break;

            //case PlayerState.pInUse:
            //    pInUse();
            //    break;

            case PlayerState.pUseMachine:
                pUseMachine();
                break;

            case PlayerState.pTired:
                pTired();
                break;

            case PlayerState.pGetFood:
                pGetFood();
                break;

            case PlayerState.pEat:
                pEat();
                break;

            case PlayerState.pInjury:
                pInjury();
                break;

            case PlayerState.pFirstAid:
                pFirstAid();
                break;

            case PlayerState.pHospital:
                pHospital();
                break;

            case PlayerState.pExit:
                break;

            default:
                pStandBy();
                Debug.Log("No state");
                break;
        }
    }

    public PlayerState CheckState()
    {
        return state;
    }

    void pStandBy()
    {
        // This can be a coroutine where the player can stay afk until he decides to click on something then it exits the coroutine.
        // Movement code to move to standby area
        //pInUse();

        // Code that makes the player move around the area randomly
    }

    void isDone()
    {
        if(Done == 8)
        {
            state = PlayerState.pExit;
        }
        
    }

    void pInUse(machine_fsm machineSelected)
    {
        //Player clicks on machine, return the type of machine
        if (machineSelected)
        {
            // need to check if the machine has been used by the player before
            // Checks if the machine is in use.
            if (machineSelected.mInUse())
            {
                // Player cant use and game tells the player so
                // Add the selected machine to the list of machines the player has done
                state = PlayerState.pStandby;
                //break;
            }
            else
            {
                //Player can use the machine
                state = PlayerState.pUseMachine;
                //break;
            }
        }
    }

    void pUseMachine()
    {
        //Counts down from 8 seconds, maybe we can use a coroutine here to force this game object to wait 8 seconds before doing the next thing

        //if(useTime == 8)
        //{
        //    if(chance = 30 %)
        //    {
        //        state = PlayerState.pTired
        //    }
        //    else if(chance = 10 %)
        //    {
        //        state = PlayerState.pInjury
        //    }
        //    else
        //    {
        //        state = PlayerState.pStandby
        //    }
        //}
    }

    // Tired
    void pTired()
    {
        //Player is now tired it tells the player it is now tired
        // Movement code to move to the standby area
        state = PlayerState.pGetFood;
    }

    void pGetFood()
    {
        // Player has to click on the vending machine to get the food.
        // Movement code
        state = PlayerState.pEat;
    }

    void pEat()
    {
        // Character eats the food
        // Nom nom words, and UI 
        state = PlayerState.pStandby;
    }

    // Injured
    void pInjury()
    {
        // Use the pain spray
        //if(serious < 20% )
        //{
        //    state = PlayerState.pHospital;
        //}
        //else
        //{
        //    state = PlayerState.pFirstAid
        //}
    }

    void pFirstAid()
    {
        // Use the pain spray
        state = PlayerState.pStandby;
    }

    void pHospital()
    {
        // Game fades to black, and reappears after 10 seconds (coroutine?)
        state = PlayerState.pStandby;
    }

    void pExit()
    {
        //Player is done and moves to the exit.
    }
}
