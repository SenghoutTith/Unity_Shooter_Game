using UnityEngine;

public class PlayAnimationEvents : MonoBehaviour
{
    private WeaponVisualController weaponVisualController;
    private PlayerWeaponController weaponController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponVisualController = GetComponentInParent<WeaponVisualController>();
        weaponController = GetComponentInParent<PlayerWeaponController>();
    }

    void ReloadIsOver() {
        weaponVisualController.ReturnRigWeightToOne();
        // Refill bullets for the current weapon
        weaponController.CurrentWeapon().RefillBullets();
        weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() => weaponVisualController.SwitchOnCurrentWeaponModel();
}
