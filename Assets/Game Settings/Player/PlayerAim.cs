using UnityEngine;

public class PlayerAim : MonoBehaviour
{

    private PlayerController controls;
    private Player player;

    [Header("Aim Control")]
    [SerializeField] private Transform aim;
    [SerializeField] private LineRenderer aimLaser;

    [SerializeField] private bool isAimingPercisely;

    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget;
    [Range(.5f,1)]
    [SerializeField] private float minAimDistance = 1.5f;
    [Range(1,3f)]
    [SerializeField] private float maxAimDistance = 4;
    [Range(3f,5f)]
    [SerializeField] private float cameraSensitivity = 5f;

    [SerializeField] private LayerMask aimLayerMask;
    private Vector3 lookingDirection;
    private Vector2 aminInput;

    private RaycastHit lastKnownMouseHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) {
            isAimingPercisely = !isAimingPercisely;
        }
        UpdateAimVisual();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    public RaycastHit GetMouseHitInfo() {
        Ray ray = Camera.main.ScreenPointToRay(aminInput);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, aimLayerMask)) {
            lastKnownMouseHit = hit;
            return lastKnownMouseHit;
        }
        return lastKnownMouseHit;
    }

    void AssignInputEvents()
    {
        controls = player.controls;
        controls.Character.Aim.performed += ctx => aminInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => aminInput = Vector2.zero;
    }

    private Vector3 DesiredAimPosition() {
        float actualMaxCamDistance = player.movement.moveInput.y < -.5f ? minAimDistance : maxAimDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distance = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampDistance = Mathf.Clamp(distance, minAimDistance, actualMaxCamDistance);

        desiredCameraPosition = transform.position + aimDirection * clampDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    public bool CanAimPrecisely() {
        return isAimingPercisely;
    }

    private void UpdateAimVisual() {

        aimLaser.enabled = player.weapon.WeaponReady();

        if (aimLaser.enabled == false) {
            return;
        }

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aim.position);
        weaponModel.gunPoint.LookAt(aim.position);

        Transform gunPoint = player.weapon.GetGunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection();

        float lastTipLength = .5f;
        float gunDistance = player.weapon.CurrentWeapon().gunDistance;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance)) {
            endPoint = hit.point;
            lastTipLength = 0;
        }
        // Ensure the LineRenderer has enough positions
        aimLaser.positionCount = 3;
        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * lastTipLength);
    }

    private void UpdateAimPosition() {
        aim.position = GetMouseHitInfo().point;
        if (!isAimingPercisely) {
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
        }
    }
    private void UpdateCameraPosition() {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredAimPosition(), cameraSensitivity * Time.deltaTime);
    }
}
