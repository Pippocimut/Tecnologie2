using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public float health;
    public float maxHealth = 100f;
    public int itemCount = 0;
    public MeshRenderer model;
    public int gold;
    public Transform weaponManager;
    public int role;

    public UI_Player uI;

    public void Initialize(int _id)
    {
        id = _id;
        health = maxHealth;
    }

    public void InitializeGhost(int _id){
        id = _id;
        model.enabled = false;
    }
    public void Destroy(){
        GameManager.instance.players.Remove(id);
        Destroy(gameObject);
    }
    public void RemoveWeapon(){

        if(weaponManager!=null && weaponManager.childCount>0){
            Destroy(weaponManager.GetChild(0).gameObject);
        }
    }
    public void PickUpKnife()
    {
        GameObject estetico = Instantiate(Resources.Load<GameObject>(@"Items/Estetico"),weaponManager.position,weaponManager.rotation,weaponManager);
        estetico.layer = LayerMask.NameToLayer("Weapon");
    }
    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0f)
        {
            Die();
        }
    }
    public void Gold(int _gold){
        gold = _gold;
    }
    public void Die()
    {
        if(weaponManager!=null)
            weaponManager.gameObject.SetActive(false);
        model.enabled = false;
    }
    public void ShootEffect(){
        weaponManager.GetComponent<PistolEffect>().Effect();
    }

    public void DestroyGun(){
        weaponManager.gameObject.SetActive(false);
    }
    
    public void SpawnPistol(){
        weaponManager.gameObject.SetActive(true);
    }
}
