using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    Rifle,
    Shotgun,
    Sniper
}

public enum ShootType
{
    Single,
    Auto
}


[System.Serializable]

public class Weapon
{
    [Header("Weapon Specifics")]
    public WeaponType weaponType;
    public ShootType shootType;
    public float gunDistance { get; private set; }
    public float cameraDistance { get; private set; }
    public float currentSpread = 1;

    [Header("Megazine Details")]
    public int bulletInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;
    
    [Header("Fire Shot")]
    public int bulletsPerShot { get; private set; }
    public float fireRate = 1;
    private float defaultFireRate;
    private float lastShootTime = 0;

    [Header("Burst Shot")]
    public bool burstActive;
    private bool burstAvailable;
    private int burstBulletsPerShot;
    private float burstFireRate;
    public float burstFireDelay { get; private set;}

    [Header("Weapon Spread")]
    private float baseSpread = 1;
    private float maximumSpread = 2;
    private float spreadIncreaseRate = .15f;
    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;

    public Weapon_Data weaponData { get; private set; }

    public Weapon(Weapon_Data weaponData) {

        bulletInMagazine = weaponData.bulletInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        fireRate = weaponData.fireRate;
        shootType = weaponData.shootType;
        bulletsPerShot = weaponData.bulletsPerShot;
        weaponType = weaponData.weaponType;
        defaultFireRate = fireRate;

        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;

        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        burstAvailable = weaponData.burstAvailable;
        burstActive = weaponData.burstActive;
        burstBulletsPerShot = weaponData.burstBulletsPerShot;
        burstFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;

        this.weaponData = weaponData;
    }

    #region BURST METHODS
    public bool BurstActivated() {
        if (weaponType == WeaponType.Shotgun) {
            // burstFireDelay = .02f;
            // burstFireDelay = 0;
            return true;
        }
        return burstActive;
    }

    public void ToggleBurst() {
        if (burstAvailable == false) {
            return;
        }
        burstActive = !burstActive;
        if (burstActive) {
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        } else {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }
    #endregion

    #region SPREAD METHODS
    public Vector3 ApplySpread(Vector3 direction)
    {
        UpdateSpread();
        float randomizedValue = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
        return spreadRotation * direction;
    }

    public void IncreaseSpread() {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    public void UpdateSpread() {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown) {
            currentSpread = baseSpread;
        } else {
            IncreaseSpread();
        }
        lastSpreadUpdateTime = Time.time;
    }
    #endregion

    public bool CanShoot() => HaveEnoughAmmo() && ReadyToFire(); 

    private bool ReadyToFire() {
        if (Time.time > lastShootTime + 1 / fireRate) {
            lastShootTime = Time.time;
            return true;
        }
        return false;
    }

    #region RELOAD METHODS
    private bool HaveEnoughAmmo() => bulletInMagazine > 0;

    public bool CanReload() {

        if(bulletInMagazine == magazineCapacity) {
            return false;
        }

        if (totalReserveAmmo > 0) {
            return true;
        }
        return false;
    }

    public void RefillBullets() {

        // totalReserveAmmo += bulletInMagazine;
        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo) {
            bulletsToReload = totalReserveAmmo;
        }

        totalReserveAmmo -= bulletsToReload;
        bulletInMagazine += bulletsToReload;

        if (totalReserveAmmo < 0) {
            totalReserveAmmo = 0;
        }
    }
    #endregion
}
