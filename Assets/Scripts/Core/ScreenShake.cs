using Cinemachine;
using Game.Actions;
using Game.Projectiles;
using Game.Units;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private float shootShake = 1f;
    [SerializeField] private float grenadeShake = 5f;
    [SerializeField] private float swordShake = 2f;

    private CinemachineImpulseSource cinemachineImpulseSource;

    void Awake()
    {
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExplode += GrenadeProjectile_OnAnyGrenadeExplode;
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;
    }

    private void GrenadeProjectile_OnAnyGrenadeExplode()
    {
        GenerateImpulse(grenadeShake);
    }

    private void ShootAction_OnAnyShoot(Unit unit, Unit target)
    {
        GenerateImpulse(shootShake);
    }

    private void SwordAction_OnAnySwordHit()
    {
        GenerateImpulse(swordShake);
    }

    public void GenerateImpulse(float intensity = 1.0f)
    {
        cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
