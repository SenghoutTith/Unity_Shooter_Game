using UnityEngine;

public class Player : MonoBehaviour
{

    public PlayerController controls { get; private set; }

    public PlayerAim aim { get; private set; } // read only
    public PlayerMovement movement { get; private set; } // read only
    public PlayerWeaponController weapon { get; private set; } // read only
    public WeaponVisualController weaponVisuals { get; private set; } // read only
    public PlayerInteraction interaction { get; private set; } // read only

    
    public void Awake() {
        controls = new PlayerController();

        aim = GetComponent<PlayerAim>();

        movement = GetComponent<PlayerMovement>();

        weapon = GetComponent<PlayerWeaponController>();

        weaponVisuals = GetComponentInChildren<WeaponVisualController>();

        interaction = GetComponent<PlayerInteraction>();
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }
}
