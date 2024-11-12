using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class player_fsm : MonoBehaviour
{
    public enum PlayerState
    {
        Standby,
        InUse,
        UseMachine,
        Tired,
        GetFood,
        Eat,
        Injury,
        FirstAid,
        Hospital,
        Moving,
        Exit
    }

    static PlayerState state;                                   // The var containing the enum, and the "State" this FSM is in right now

    public NavMeshAgent PlayerAgent;                            // Reference to the player's NavMeshAgent


    // UI elements
    public GameObject GetFood;
    public GameObject fullyEnergised;
    public GameObject painSpray;
    public GameObject hospitalUI;
    public GameObject IsInUse;
    public GameObject AlreadyUsed;
    public GameObject ExitUI;

    public Image image;                                         // Reference to hospitalUI panel image
    public TextMeshProUGUI timeLeft;                            // Reference to hospitalUI text
    private int countdownTime = 10;                             // Countdown timer on the hospitalUI

    // Booleans that represents the pressing of buttons
    private bool GetFoodOKButtonPress = false;
    private bool GetEnergisedButtonPress = false;
    private bool AlrightButton = false;

    private int Done;                                           // This contains the nunber of "machines" the player has done
    private machine_fsm machine;                                // Reference to the machine class for method usage

    private int useTime;                                        // Variable integer that holds the usage time of the machine
    private int rand;                                           // Contains a pseudo-random number to determine the next state
    private GameObject machineHitByRay;                         // Reference to the machine that got hit by the raycast
    private GameObject machineCollided;                         // Reference to object "this" collided with
    private GameObject vendingMachine;                          // Reference to the vending machine in the scene

    public GameObject StandByArea;                              // Reference to the yellow mat in the middle of the game level

    private Camera mainCam;                                     // Reference to main cam
    private bool doingSomething;                                // This is a boolean that determines if the script is "doing something or not"

    private GameObject textObject;
    public GameObject textPrefab;


    // Start is called before the first frame update
    void Start()
    {
        // Sets the machines done to 0
        Done = 0;
        // Testing pirposes
        // StartCoroutine(CurrentState());

        textObject = Instantiate(textPrefab, transform.position, transform.rotation);

        // Reference to Image for hospital background
        image = hospitalUI.GetComponent<Image>();

        // References the camera
        mainCam = Camera.main;

        // Enter state
        state = PlayerState.Standby;

    }

    private void Update()
    {
        Transform textTrans = textObject.GetComponent<Transform>();
        textTrans.position = transform.position;
        textTrans.Translate(Vector3.up * 0.7f);
        textTrans.Translate(Vector3.left * 1.5f);

        StateTextScript textScript = textObject.GetComponent<StateTextScript>();

        // Runs every frame, checks if the player has clicked, and if he did, return what he clicked.
        machineHitByRay = machineHit();

        // Switch-Functions FSM style
        switch (state)
        {
            case PlayerState.Standby:
                pStandBy();
                                textScript.UpdateStateText("Current State: Standby");

                break;
            case PlayerState.Moving:
                pUseMachine();
                                textScript.UpdateStateText("Current State: Moving");

                break;
            case PlayerState.InUse:
                pInUse(machineHitByRay);
                textScript.UpdateStateText("Current State: InUse");
                break;

            case PlayerState.UseMachine:
                            textScript.UpdateStateText("Current State: UseMachine");

                break;

            case PlayerState.Tired:
                pTired();
                                textScript.UpdateStateText("Current State: Tired");

                break;

            case PlayerState.GetFood:
                pGetFood();
                                textScript.UpdateStateText("Current State: GetFood");

                break;

            case PlayerState.Eat:
                pEat();
                                textScript.UpdateStateText("Current State: Eat");

                break;

            case PlayerState.Injury:
                pInjury();
                                textScript.UpdateStateText("Current State: Injury");

                break;

            case PlayerState.FirstAid:
                pFirstAid();
                                textScript.UpdateStateText("Current State: FirstAid");

                break;

            case PlayerState.Exit:
                pExit();
                break;

            case PlayerState.Hospital:
                pHospital();
                                textScript.UpdateStateText("Current State: Hospital");

                break;

            default:
                pStandBy();
                Debug.Log("Player Error, No state");
                break;
        }
    }

    // To be called by other classes - returns the current state of the player
    public PlayerState CheckState()
    {
        return state;
    }

    void pExit()
    {
        ExitUI.SetActive(true);
    }

    // Default idle state
    void pStandBy()
    {
        // Checks if the script is doing something at the moment
        if (doingSomething == false)
        {
            // This triggers idle functions, where the character just moves around a set area randomly
            StartCoroutine(StartIdle());
            doingSomething = true;
        }

        // Checks if the player has reached the StandByArea, with some leeway
        if (PlayerAgent.remainingDistance <= 2)
        {
            // Only returns true when the player both clicks and hits something
            if (Input.GetMouseButtonDown(0) && machineHitByRay)
            {
                state = PlayerState.InUse;
            }

            // Checks if the player has done all 8 machines
            if (Done == 8)
            {
                state = PlayerState.Exit;
            }
        }

    }

    void pInUse(GameObject machineSelected)
    {
        // References the script component on the machine object
        machine = machineSelected.GetComponent<machine_fsm>();

        // Checks if there is a machine script reference
        if (machine)
        {
            // Checks if the machine is in use.
            if (machine.CheckState() == machine_fsm.MachineState.InUse)
            {
                // Activates the UI
                IsInUse.SetActive(true);

                // The state transition
                state = PlayerState.Standby;

            }
            // Checks if the machine is under repair or broken
            else if (machine.CheckState() == machine_fsm.MachineState.Repair || machine.CheckState() == machine_fsm.MachineState.Broken)
            {
                state = PlayerState.Standby;

            }
            else
            {
                // Checks if the machine has been used by the player
                if (machine.player_used)
                {
                    // Activates the UI
                    AlreadyUsed.SetActive(true);
                    state = PlayerState.Standby;
                }
                else
                {
                    // Moves the character to the machine
                    Moving(machineSelected);
                    state = PlayerState.Moving;
                }
            }

        }
    }

    public void pUseMachine()
    {
        // Checks if the collided object is the same as the object hit by the ray, and also if script is doing anything
        if (machineCollided == machineHitByRay && doingSomething == false)
        {
            // Starts the player usage of the machine
            StartCoroutine(PlayerStartUsage());
            doingSomething = true;

            state = PlayerState.UseMachine;
        }

    }

    // Tired
    void pTired()
    {
        // Checks if the script is doing something at the moment
        if (doingSomething == false)
        {
            StartCoroutine(StartIdle());
            doingSomething = true;
        }
        
        // If player is near the mat
        if (PlayerAgent.remainingDistance <= 2)
        {
            // Activates the UI
            GetFood.SetActive(true);
            // Checks if the player has pressed the button on the UI
            if (GetFoodOKButtonPress)
            {
                // Resets the button back to unpressed and deactivates the UI
                GetFoodOKButtonPress = false;
                GetFood.SetActive(false);

                state = PlayerState.GetFood;
            }
        }
    }

    void pGetFood()
    {
        // Code that detects the player clicking the vending machine
        vendingMachine = machineHit();
        if (vendingMachine)
        {
            PlayerAgent.SetDestination(vendingMachine.transform.position);
        }

        // If the machine collided with is the same as what was hit by the player
        if (machineCollided == vendingMachine)
        {
            Debug.Log("pEat is called");
            state = PlayerState.Eat;
        }

    }

    void pEat()
    {
        // Character eats the food
        // pPlayer is now fully energised!

        //Activates the UI
        fullyEnergised.SetActive(true);

        //Moves the character to the stand by area again
        PlayerAgent.SetDestination(StandByArea.transform.position);

        // Checks if the player has pressed the button on the UI
        if (GetEnergisedButtonPress)
        {
            // Resets the button back to unpressed and deactivates the UI
            GetEnergisedButtonPress = false;
            fullyEnergised.SetActive(false);
            state = PlayerState.Standby;
        }

    }


    // Injured
    void pInjury()
    {
        rand = Random.Range(1, 11);

        // If the rand no. is less than or equals to 2(so technically 20 %), transition to hospital state, else go first aid
        if (rand <= 2)
        {
            state = PlayerState.Hospital;
        }
        else
        {
            state = PlayerState.FirstAid;
        }
    }

    void pFirstAid()
    {
        // Activates the UI
        painSpray.SetActive(true);
        // Checks if the player has pressed the button on the UI
        if (AlrightButton)
        {
            // Resets the button back to unpressed and deactivates the UI
            AlrightButton = false;
            painSpray.SetActive(false);
            state = PlayerState.Standby;
        }

    }

    void pHospital()
    {
        // Activates the UI
        hospitalUI.SetActive(true);
        // Checks if the player has pressed the button on the UI
        if (doingSomething == false)
        {
            // UI fades in
            StartCoroutine(HospitalUI());
            // Starts the countdown
            StartCoroutine(Countdown());
            doingSomething = true;
        }

    }
    // Button presses: **************************************************
    public void IsInUseButton()
    {
        // Checks if the player has pressed the button on the UI
        IsInUse.SetActive(false);
    }

    public void AlreadyUsedButton()
    {
        // Checks if the player has pressed the button on the UI
        AlreadyUsed.SetActive(false);
    }

    public void SprayPainSpray()
    {
        // Checks if the player has pressed the button on the UI
        AlrightButton = true;
    }

    public void GetEnergisedButton()
    {
        // Checks if the player has pressed the button on the UI
        GetEnergisedButtonPress = true;
    }

    public void GetFoodOKButton()
    {
        // Checks if the player has pressed the button on the UI
        GetFoodOKButtonPress = true;
    }
    // ******************************************************************

    // Moving function to move the character to the machine
    void Moving(GameObject machineSelected)
    {

        PlayerAgent.SetDestination(machineSelected.transform.position);

    }

    // Detects when the collider on the character has hit something and sets a reference to it
    private void OnTriggerEnter(Collider other)
    {
        machineCollided = other.gameObject;
    }

    private GameObject machineHit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // declare a ray from screen point to a point in the world
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // If the ray hit something
                if (hit.collider != null)
                {
                    // Set a reference to the hit gameObject
                    machineHitByRay = hit.collider.gameObject;

                }
            }
        }
        return machineHitByRay;
    }

    IEnumerator Countdown()
    {
        // Runs forever
        for (; ; )
        {
            if (countdownTime >= 1)
            {
                // This tracks the countdown time for when the player goes to the hospital
                countdownTime -= 1;
                timeLeft.text = "Time left: " + countdownTime.ToString();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                // Deactivates the UI 
                hospitalUI.SetActive(false);
                state = PlayerState.Standby;
                doingSomething = false;
                countdownTime = 10;
                yield break;
            }
        }
    }

    IEnumerator HospitalUI()
    {
        // Starting from zero and moving up slowly
        for (float i = 0; i <= 1f; i += 0.1f)
        {
            // Causes the panel to fade to black
            var tempColor = image.color;
            tempColor.a = i;
            image.color = tempColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator PlayerStartUsage()
    {
        // TO call the player method in the machine class to begin the coroutine there as well
        machine.PlayerUseMachine();

        // This function is here to force the whole script to wait for the MachineInUse() coroutine to run its entire course
        yield return StartCoroutine(MachineInUse());

        rand = Random.Range(1, 11);

        if (rand <= 1) // Chance ~ 10% to get injured
        {
            state = PlayerState.Injury;
        }
        else if (rand <= 4) // Chance ~ 30% to get tired
        {
            state = PlayerState.Tired;
        }
        else
        {
            machineHitByRay = null;
            state = PlayerState.Standby;
        }

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
                Done += 1;
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
            PlayerAgent.SetDestination(StandByArea.transform.position + new Vector3(Random.Range(-2, 3), Random.Range(-2, 3), Random.Range(-2, 3)));
            yield return new WaitForSeconds(3);
            yield break;
        }
    }

    // Testing purposes
    IEnumerator CurrentState()
    {
        for (; ; )
        {
            Debug.Log(Done);
            Debug.Log(state);
            yield return new WaitForSeconds(1);
        }
    }
}
