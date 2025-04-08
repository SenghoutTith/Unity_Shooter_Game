using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum AmmoBoxType 
{
    smallBox,
    BigBox,
}

[System.Serializable]
public class AmmoData
{
    public WeaponType weaponType;
    [Range(10, 100)] public int minAmount;
    [Range(10, 100)] public int maxAmount;
}

public class Pickup_Ammo : Interactables
{
    [SerializeField] private AmmoBoxType boxType;

    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    [SerializeField] private GameObject[] boxModel;

    private void Start() => SetupBoxModel();

    private void SetupBoxModel() {
        for (int i = 0; i < boxModel.Length; i++) {
            boxModel[i].SetActive(false);
            if (i == (int)boxType) {
                boxModel[i].SetActive(true);
                UpdateMeshAndMeshRenderer(boxModel[i].GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;
        if (boxType == AmmoBoxType.BigBox)
        {
            currentAmmoList = bigBoxAmmo;
        }
        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType);
            if (weapon != null) {
                AddBulletsToWeapon(weapon, GetBulletAmount(ammo));
            }
        }
        ObjectPool.instance.ReturnObject(gameObject);
    }

    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null)
        {
            return;
        }

        weapon.totalReserveAmmo += amount;
    }

    private int GetBulletAmount(AmmoData ammo)
    {
        float min = Mathf.Min(ammo.minAmount, ammo.maxAmount);
        float max = Mathf.Max(ammo.minAmount, ammo.maxAmount);
        
        float randomValue = Random.Range(min, max);

        return Mathf.RoundToInt(randomValue);
    }
}
