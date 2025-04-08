using UnityEngine;

public class Pickup_Weapon : Interactables
{
    [SerializeField] private Weapon_Data weaponData;
    [SerializeField] private BackupWeaponModel[] models;
    [SerializeField] private Weapon weapon;
    private bool oldWeapon;

    private void Start() {
        weaponController = FindFirstObjectByType<PlayerWeaponController>();
        if (oldWeapon == false) {
            weapon = new Weapon(weaponData);
        }
        SetupGameObject();
    }

    public void SetupPickupWeapon(Weapon weapon, Transform transform) {
        oldWeapon = true;
        this.weapon = weapon;
        weaponData = weapon.weaponData;
        this.transform.position = transform.position + new Vector3(0, .5f, 0);
    }

    [ContextMenu("Update Item Model")]
    public void SetupGameObject() {
        gameObject.name = "Pickup_Weapon - " + weaponData.weaponType.ToString();
        SetupWeaponModel();
    }
    
    private void SetupWeaponModel() {
        foreach (BackupWeaponModel model in models)
        {
            model.gameObject.gameObject.SetActive(true);
            if (model.weaponType == weaponData.weaponType) {
                model.gameObject.SetActive(true);
                UpdateMeshAndMeshRenderer(model.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        weaponController.PickupWeapon(weapon);
        ObjectPool.instance.ReturnObject(gameObject);       
    }
}
