using UnityEngine;
using System.Collections;

public class WaterScript : MonoBehaviour 
{
    public AudioSource BulletSplashSound;
    public ParticleSystem BulletSplashParticle;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == (int)LayerEnum.Bullet)
        {
            BulletEnteredWater();
        }
        else if(other.gameObject.layer == (int)LayerEnum.Car)
        {
            PlaneEnteredWater();
        }
    }

    #region Bullet Methods
    void BulletEnteredWater()
    {
        PlaySplashSound();
        BulletSplashParticle.Play();
    }

    void PlaySplashSound()
    {
        BulletSplashSound.Play();
    }

    void PlaySplashParticle()
    {
        BulletSplashParticle.Play();
    }
    #endregion

    #region Plane Methods
    void PlaneEnteredWater()
    {
        
    }
    #endregion
}
