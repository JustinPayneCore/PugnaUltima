using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// Class that handles the dodge of a fighter/player.
/// Author(s): Richard Mac
/// Date: Nov 18 2022
/// Change History: Nov 22 2022 - Lukasz Bednarek
/// - integrated Jaspers' animations using Animator controller and set triggers
/// - Add logic for RPC call for sound effect method.
/// Change History: November 24 2022 - Richard Mac
/// - implemented on dodge colour change
/// - implemented a cooldown for the dodge mechanic 
/// </summary>
public class Dodge : NetworkBehaviour
{
    // fighter prefab components
    protected Rigidbody2D _body; // affects jump velocity
    protected Collider2D _playerHitbox; // player's box collider (hitbox)
    protected Animator _animator;
    protected GameObject _audioManager;
    protected NetworkPlayer _networkPlayer; //player's local instance

    private float cooldown = 1f;
    private float nextDodgeTime = 0f; 
    private bool isDodgePressed;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        CacheComponents();
    }

    // Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    private void Start()
    {
        this._audioManager = GameObject.Find("SceneAudioManager");
    }

    // Helper method to initialize fighter prefab components
    private void CacheComponents()
    {
        if (!_body) _body = GetComponent<Rigidbody2D>();
        if (!_playerHitbox) _playerHitbox = GetComponent<Collider2D>();
        if (!_animator) _animator = GetComponent<Animator>();
        if (!_networkPlayer) _networkPlayer = GetComponent<NetworkPlayer>();
    }

    public override void FixedUpdateNetwork()
    {
        // checking for input presses
        if (GetInput(out NetworkInputData data))
        {
            isDodgePressed |= data.dodge;
           
        }

        if (isDodgePressed)
        {
            isDodgePressed = false;
            if (Time.time > nextDodgeTime)
            {
                StartCoroutine(DodgeAction());
                nextDodgeTime = Time.time + cooldown;
            }
           
        }
    }

    private IEnumerator DodgeAction()
    {
        // beginning section - stop inputs and play animation
        _networkPlayer.DisableInputsTemporarily(0.7f);
        yield return new WaitForSeconds(0.1f);  // time till next section

        // middle section - disable hitbox (invincible), show dodge color effect, and play sound
        Debug.Log("hitbox down");
        _playerHitbox.enabled = false;
        _networkPlayer.ColorSpriteTemporarily(0.5f, Color.gray);
        if (Object.HasStateAuthority) _audioManager.GetComponent<GameplayAudioManager>().RPC_PlayUniversalCharatcerSFXAudio(PlayerActions.Dodge.ToString());
        yield return new WaitForSeconds(0.5f);  // time till next section


        // ending section - reenable hitbox (not invincible)
        Debug.Log("hitbox back");
        _playerHitbox.enabled = true;

    }

}
