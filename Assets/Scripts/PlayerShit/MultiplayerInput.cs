using UnityEngine;
using System.Collections;
using System;

public class MultiplayerInput : MonoBehaviour
{
    private InputEmulator inputEmulator;

    private int playerNumber;
	[HideInInspector]
	public Vector2 controllerInput;

	private Vector3 mouseVector;
    [HideInInspector]
	public Vector3 mousePosition;

	public Action OnReceiveAttackInput = delegate { };
	public Action OnReceiveDodgeInput = delegate { };
	public Action OnBlockInputEnter = delegate { };
	public Action OnBlockInputExit = delegate { };
	public Action OnReceiveJumpInput = delegate { };

	private Plane gamePlane;

    private bool initialized;

    public void Initialize (PlayerManager playerManager, int playerNumber, bool isAI)
    {
        this.playerNumber = playerNumber;

        gamePlane.SetNormalAndPosition(-Vector3.forward, Vector3.zero);

        if (isAI)
        {
            inputEmulator = gameObject.AddComponent<InputEmulator>();
            inputEmulator.Initialize(playerManager, this, GameManager.instance.GetOtherPlayer(playerNumber));
        }

        initialized = true;
    }

	void Update ()
	{
        if (!initialized)
            return;

        controllerInput.x = Input.GetAxisRaw("Horizontal" + playerNumber.ToString());
        controllerInput.y = Input.GetAxisRaw("Vertical" + playerNumber.ToString());

        bool dodge = Input.GetButtonDown("Dodge" + playerNumber.ToString());
        bool attack = Input.GetButtonDown("Attack" + playerNumber.ToString());
        bool blockStart = Input.GetButtonDown("Block" + playerNumber.ToString());
        bool blockStop = Input.GetButtonUp("Block" + playerNumber.ToString());
        bool jump = Input.GetButtonUp("Jump" + playerNumber.ToString());

        if (dodge)
        {
            OnReceiveDodgeInput();
        }
        if (attack)
        {
            OnReceiveAttackInput();
        }
        if (blockStart)
        {
            OnBlockInputEnter();
        }
        if (blockStop)
        {
            OnBlockInputExit();
        }
        if (jump)
        {
            OnReceiveJumpInput();
        }
    }
}
