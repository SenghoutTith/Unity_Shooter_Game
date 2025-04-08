using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Rigidbody rb;
    private BoxCollider cd;
    private MeshRenderer mesh;
    private TrailRenderer trail;
    private bool bulletDisabled;

    [SerializeField] private GameObject bulletImpactFX;

    private Vector3 startPosition;
    private float flyDistance;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        cd = GetComponent<BoxCollider>();
        mesh = GetComponent<MeshRenderer>();
        trail = GetComponent<TrailRenderer>();
    }

    public void BulletSetup(float flyDistance) {
        bulletDisabled = false;
        cd.enabled = true;
        mesh.enabled = true;
        trail.time = .25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + .5f;
    }

    private void Update() {

        FadeTrailIfNeeded();
        DisabledBulletIfNedded();
        ReturnToPoolIfNeeded();
    }

    private void FadeTrailIfNeeded() {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f) {
            trail.time -= 2 * Time.deltaTime;
        }
    }

    private void DisabledBulletIfNedded() {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled) {
            cd.enabled = false;
            mesh.enabled = false;
            bulletDisabled = true;
        }
    }

    private void ReturnToPoolIfNeeded() {
        if (trail.time < 0) {
            ReturnBulletToPool();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        CreateImpactFX(collision);
        ReturnBulletToPool();
    }

    private void ReturnBulletToPool() => ObjectPool.instance.ReturnObject(gameObject);

    private void CreateImpactFX(Collision collision) {
        if (collision.contacts.Length > 0) {
            ContactPoint contact = collision.contacts[0];
            GameObject newImpactFX = ObjectPool.instance.GetObject(bulletImpactFX);
            newImpactFX.transform.position = contact.point;
            ObjectPool.instance.ReturnObject(newImpactFX, 1);
        }
    }

}
