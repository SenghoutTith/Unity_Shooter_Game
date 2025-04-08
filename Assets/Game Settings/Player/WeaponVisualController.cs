using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualController : MonoBehaviour
{
    private Player player;
    private Animator animator;
    private bool isGrabbingWeapon;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;

    [Header("Rig Weight")]
    private Rig rig;
    [SerializeField] private float rigIncreaseStep;
    private bool rigShouldBeIncreased;

    [Header("Left Hand IK")]
    [SerializeField] private Transform leftHand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        player = GetComponentInParent<Player>();
        animator = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    // Update is called once per frame
    void Update()
    {
        PlayReloadAnimation();

        if (Input.GetKey(KeyCode.K)) {
            rigShouldBeIncreased = true;
        }

        if (rigShouldBeIncreased) {
            rig.weight += rigIncreaseStep * Time.deltaTime;

            if (rig.weight >= 1) {
                rigShouldBeIncreased = false;
            }
        }
    }

    public WeaponModel CurrentWeaponModel() {
        WeaponModel weaponModel = null;
        WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;
        for (int i = 0; i < weaponModels.Length; i++) {
            if (weaponModels[i].weaponType == weaponType) {
                weaponModel = weaponModels[i];
            }
        }
        return weaponModel;
    }

    public void PlayFireAnimation() => animator.SetTrigger("Reload");

    public void PlayReloadAnimation() {
        if (Input.GetKeyDown(KeyCode.R)) {
            PlayFireAnimation();
            rig.weight = .15f;
        }
    }

    public void ReturnRigWeightToOne() => rigShouldBeIncreased = true;

    public void SwitchOnCurrentWeaponModel() {
        SwitchOffWeaponModel();
        SwitchOffWeaponBackupModel();
        if (player.weapon.HasOnlyOneWeapon() == false) {
            SwitchOnWeaponBackupModel();
        }
        SwitchAnimationLayer((int)CurrentWeaponModel().holdType);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttactLeftHandToGun();
    }


    public void SwitchOffWeaponModel() {
        for (int i = 0; i < weaponModels.Length; i++) {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void SwitchOffWeaponBackupModel() {
        foreach (BackupWeaponModel backupModel in backupWeaponModels) {
            backupModel.Activate(false);
        }
    }

    public void SwitchOnWeaponBackupModel() {
        SwitchOffWeaponModel();
        
        BackupWeaponModel lowHang = null;
        BackupWeaponModel backHang = null;
        BackupWeaponModel sideHang = null;

        foreach (BackupWeaponModel backupModel in backupWeaponModels) {

            if (backupModel.weaponType == player.weapon.CurrentWeapon().weaponType) {
                continue;
            }

            if (player.weapon.WeaponInSlots(backupModel.weaponType) != null) {
                if (backupModel.HangTypeIs(HangType.Lowhang)) {
                    lowHang = backupModel;
                } else if (backupModel.HangTypeIs(HangType.Backhang) && backHang != null) {
                    backHang = backupModel;
                } else if (backupModel.HangTypeIs(HangType.Sidehang)) {
                    sideHang = backupModel;
                }
            }
        }

        lowHang?.Activate(true);
        backHang?.Activate(true);
        sideHang?.Activate(true);
    }

    void AttactLeftHandToGun() {
        // Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;
        Transform targetTransform = CurrentWeaponModel().holdPoint;
        leftHand.localPosition = targetTransform.localPosition;
        leftHand.localRotation = targetTransform.localRotation;
    }

    void SwitchAnimationLayer(int layerIndex) {
        for (int i = 1; i < animator.layerCount; i++) {
            animator.SetLayerWeight(i, 0);
        }
        animator.SetLayerWeight(layerIndex, 1);
    }

    private void PlayWeaponGrabAnimation() {
        GrabType grabType = CurrentWeaponModel().grabType;
        // leftHand.weight = 0;
        animator.SetFloat("WeaponGrabType", (float)grabType);
        animator.SetTrigger("GrabWeapon");
        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool isBusy) {
        isGrabbingWeapon = isBusy;
        animator.SetBool("BusyGrabbingWeapon", isGrabbingWeapon);
    }
}