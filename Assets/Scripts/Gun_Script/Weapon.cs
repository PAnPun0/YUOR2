using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Weapon : MonoBehaviour
{

    public bool isActiveWeapon;

    public int weaponDamage;

    //стрельба
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    //бёрст
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    //Спрей
    public float spreadIntensity;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    internal Animator animator;

    //Перезарядка
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;


    public enum WeaponModel
    {
        Pistol1911,
        AK74
    }

    public WeaponModel thisWeaponModel;


    public enum ShootingMode
    {
        Single,
        Burst,
        Auto

    }

    public ShootingMode currentShootingMode;

    public void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;
    }


    void Update()
    {

        if(isActiveWeapon)
        {
            

            if(bulletsLeft==0 && isShooting)
            {
                SoundManager.Instance.emptymagSound1911.Play();
            }


            if(currentShootingMode == ShootingMode.Auto)
            {
            //Постоянная стрельба
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single ||
            currentShootingMode == ShootingMode.Burst)
            {
            //Одиночная стрельба
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading==false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

            if(readyToShoot && isShooting == false && isReloading == false && bulletsLeft <=0)
            {
                //Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0 )
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            
        }

    }

    

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        //SoundManager.Instance.shootingSound1911.Play();

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot= false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        
        //instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        bullet.transform.forward = shootingDirection;
        
        //shoot
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        
        //destroy bullet
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        //burst mode
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft >1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload()
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        if(WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > magazineSize)
        {
            bulletsLeft = magazineSize;
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }
        else
        {
            bulletsLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);

        }


        isReloading=false;
    }


    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;
        Vector3 targetPoint;
        if(Physics.Raycast(ray,out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }
        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x,y,0);

    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
