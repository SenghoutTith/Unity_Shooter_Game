using UnityEngine;

public enum HangType 
{
    Lowhang,
    Backhang,
    Sidehang
}

public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType weaponType;

    [SerializeField] private HangType hangType;

    public void Activate(bool activated) => gameObject.SetActive(activated);
    public bool HangTypeIs(HangType type) => this.hangType == type;
}
