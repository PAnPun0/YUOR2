using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {get; set;}

    public AudioSource ShootingChannel;



    public AudioClip AK74Shot;
    public AudioClip P1911Shot;


    
    public AudioSource reloadingSoundAK74;
    public AudioSource reloadingSound1911;

     
    public AudioSource emptymagSound1911;

    public AudioClip zombieWalking; 
    public AudioClip zombieChase;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath;


    public AudioSource zombieChannel;

    private void Awake()
    {
        if(Instance !=null && Instance != this)
        {
            Destroy(gameObject);
        }
        else{
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch(weapon)
        {   
            case WeaponModel.Pistol1911:
                ShootingChannel.PlayOneShot(P1911Shot);
                break;
            case WeaponModel.AK74:
                ShootingChannel.PlayOneShot(AK74Shot);
                break;
        }
    }
    public void PlayReloadSound(WeaponModel weapon)
    {
        switch(weapon)
        {   
            case WeaponModel.Pistol1911:
                reloadingSound1911.Play();
                break;
            case WeaponModel.AK74:
                reloadingSoundAK74.Play();
                break;
        }
    }
}
