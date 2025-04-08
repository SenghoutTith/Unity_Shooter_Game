using UnityEngine;

public class EnemtAnimationEvents : MonoBehaviour
{
    private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    public void AnimationTrigger() => enemy.AnimationTrigger();
}
