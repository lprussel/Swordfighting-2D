using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private PlayerManager playerManager;
    private Rigidbody rig;
    private MultiplayerInput input;

    private bool initialized;

    public void Initialize (PlayerManager playerManager, Rigidbody rigidbody, MultiplayerInput input)
    {
        this.playerManager = playerManager;
        this.rig = rigidbody;
        this.input = input;

        initialized = true;
    }
    
    void Update()
    {
        if (!initialized)
            return;

        switch (playerManager.playerState)
        {
            case PlayerManager.PlayerState.IDLE:
                HandleMovement();
                break;
            case PlayerManager.PlayerState.BLOCKING:
                //if (reposteTimer < maxReposteTime)
                //{
                //    reposteTimer += Time.deltaTime;
                //    canReposte = true;
                //}
                //else
                //    canReposte = false;

                rig.velocity = new Vector3(Mathf.Lerp(rig.velocity.x, 0, Time.deltaTime * 10), rig.velocity.y, rig.velocity.z);
                break;
            case PlayerManager.PlayerState.TELEGRAPHING:
                break;
            case PlayerManager.PlayerState.ATTACKING:
                break;
            case PlayerManager.PlayerState.RECOILING:
                rig.velocity = new Vector3(Mathf.Lerp(rig.velocity.x, 0, Time.deltaTime * 5), rig.velocity.y, rig.velocity.z);
                break;
            case PlayerManager.PlayerState.DASHING:
                break;
            case PlayerManager.PlayerState.JUMPING:
                break;
            case PlayerManager.PlayerState.HIT:
                rig.velocity = new Vector3(Mathf.Lerp(rig.velocity.x, 0, Time.deltaTime * 5), rig.velocity.y, rig.velocity.z);
                break;
            case PlayerManager.PlayerState.CANT_MOVE:
                rig.velocity = new Vector3(Mathf.Lerp(rig.velocity.x, 0, Time.deltaTime * 5), rig.velocity.y, rig.velocity.z);
                break;
        }
    }

    void HandleMovement ()
    {

    }

    void CheckGrounded ()
    {

    }

    void Telegraph ()
    {

    }

    void Attack ()
    {

    }
}
