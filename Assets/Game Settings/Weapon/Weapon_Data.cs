using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_Data", menuName = "Weapon System/Weapon Data", order = 0)]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;

    [Header("Megazine Details")]
    public int bulletInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Header("Regular Shot")]
    public WeaponType weaponType;
    public int bulletsPerShot = 1;
    public float fireRate;

    [Header("Burst Shot")]
    public bool burstAvailable;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay = .1f;

    [Header("Weapon Spread")]
    public float baseSpread = 1;
    public float maxSpread = 2;
    public float spreadIncreaseRate = .15f;

    [Header("Weapon Specifics")]
    public ShootType shootType;
    [Range(2, 12)]
    public float gunDistance = 4;
    [Range(4, 12)]
    public float cameraDistance = 7;
}
