using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20;

    [SerializeField] private Weapon_Data defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet Details")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform aim;
    
    [Header("Inventory")]
    [SerializeField] private int maxSlots = 3;
    [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private GameObject weaponPickupPrefab;


    private void Start() {
        player = GetComponent<Player>();
        AssignInputEvents();
        Invoke("EquipStartingWeapon", 0.1f);
    }

    private void Update() {
        if (isShooting) {
            Shoot();
        }
    }

    private IEnumerator BurstFire() {
        for (int i = 0; i < currentWeapon.bulletsPerShot; i++) {
            ShootSingleBullet();
            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

        }
    }

    private void Shoot() {

        if (WeaponReady() == false) {
            return;
        }
        if (currentWeapon.CanShoot() == false) {
            return;
        }

        player.weaponVisuals.PlayFireAnimation();

        if (currentWeapon.shootType == ShootType.Single) {
            isShooting = false;
        }
        if (currentWeapon.BurstActivated()) {
            StartCoroutine(BurstFire());
            return;
        }
        ShootSingleBullet();   
    }

    private void ShootSingleBullet() {
        currentWeapon.bulletInMagazine--;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = GetGunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GetGunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(currentWeapon.gunDistance);

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.linearVelocity = bulletsDirection * bulletSpeed;
    }

    private void Reload() {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    public Weapon CurrentWeapon() => currentWeapon; 

    // public Transform GetGunPoint() => gunPoint;
    public Transform GetGunPoint() {
        var currentWeaponModel = player.weaponVisuals.CurrentWeaponModel();
        if (currentWeaponModel == null) {
            return null;
        }
        return currentWeaponModel.gunPoint;
    }

    public Vector3 BulletDirection() {

        Vector3 direction = (aim.position - GetGunPoint().position).normalized;

        if (player.aim.CanAimPrecisely() == false) {
            direction.y = 0;
        }

        weaponHolder.LookAt(aim);
        GetGunPoint().LookAt(aim);

        return direction;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + BulletDirection() * 25);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(GetGunPoint().position, GetGunPoint().position + GetGunPoint().forward * 25);
    }

    #region SLOT MANAGEMENT
    private void EquipWeapon(int i) {
        if (i >= weaponSlots.Count) {
            return;
        }
        
        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];
        player.weaponVisuals.SwitchOnCurrentWeaponModel();
        CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
    }

    private void EquipStartingWeapon() {
        if (weaponSlots.Count == 0) {
            weaponSlots.Add(null);
        }
        weaponSlots[0] = new Weapon(defaultWeaponData);
        EquipWeapon(0);
    }

    public void PickupWeapon(Weapon newWeapon) {     

        if (WeaponInSlots(newWeapon.weaponType) != null) {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletInMagazine;
            return;
        }

        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType) {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);
            player.weaponVisuals.SwitchOffWeaponModel();
            weaponSlots[weaponIndex] = newWeapon;
            CreateNewWeaponOnTheGround();
            EquipWeapon(weaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnWeaponBackupModel();
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;
    public Weapon WeaponInSlots(WeaponType weapon) {

        foreach (Weapon w in weaponSlots) {
            if (w.weaponType == weapon) {
                return w;
            }
        }
    return null;
}

    public bool SetWeaponReady(bool isReady) => weaponReady = isReady;
    public bool WeaponReady() => weaponReady;

    private void DropWeapon() {
        if (HasOnlyOneWeapon()) return;
        CreateNewWeaponOnTheGround();
        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateNewWeaponOnTheGround() {
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab);
        droppedWeapon.GetComponent<Pickup_Weapon>().SetupPickupWeapon(currentWeapon, transform);
    }
    #endregion

    #region INPUT EVENTS
    private void AssignInputEvents() {
        PlayerController controls = player.controls;
        controls.Character.Fire.performed += ctx => isShooting = true;
        controls.Character.Fire.canceled += ctx => isShooting = false;
        controls.Character.EquipSlot1.performed += ctx => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += ctx => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += ctx => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += ctx => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += ctx => EquipWeapon(4);
        controls.Character.DropCurrentWeapon.performed += ctx => DropWeapon();
        controls.Character.Reload.performed += ctx => {
            if (currentWeapon.CanReload() && WeaponReady()) {
                Reload();
            }
        };
        controls.Character.ToggleWeaponMode.performed += ctx => currentWeapon.ToggleBurst();
    }
    #endregion
}
