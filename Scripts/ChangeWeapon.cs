using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;

public class ChangeWeapon : BaseObject
{
    public int weaponID = 0;
    // private PhotonView _photonView;
    //флаг для IPunObservable. True, when the user is Change
    //[SerializeField] private bool isChangeWeapon;
    [SerializeField] private Transform GOTransformWeapon;

    [SerializeField] private int ChCountWeapon;
    public int iCurActWeapon;
    //public Transform activeWeapon;

    protected override void Awake()
    {
        base.Awake();
        GOTransformWeapon = this.gameObject.transform.GetChild(0).GetChild(0);
        ChCountWeapon = GOTransformWeapon.childCount; 
        SelectWeapon();
        
    }

    void Start()
    {
    }

    //#region IPunObservable implementation
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {   // We own this player: send the others our data
    //        stream.SendNext(weaponID);
    //    }
    //    else
    //    {   // Network player, receive data
    //        weaponID = (int)stream.ReceiveNext();
    //    }
    //}
    //#endregion

    [PunRPC]
    private void ActivateWeapon(int i)
    {
        GOTransformWeapon.GetChild(i).gameObject.SetActive(true);
    }
    [PunRPC]
    private void unActivateWeapon(int i)
    {
        GOTransformWeapon.GetChild(i).gameObject.SetActive(false);
    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in GOTransformWeapon)
        {
            if (i == weaponID)
            {
                Photon.RPC("ActivateWeapon", RpcTarget.All, i);
                //weapon.gameObject.SetActive(true);
                //if (i == 0) { IsWeapon1 = true;  }
                //if (i == 1) { IsWeapon2 = true; }
                //if (i == 2) { IsWeapon3 = true; }
                //if (i == 3) { IsWeapon4 = true; }

            }
            else
            {
                Photon.RPC("unActivateWeapon", RpcTarget.All, i);

                //weapon.gameObject.SetActive(false);
                //if (i == 0) {  IsWeapon2 = false; IsWeapon3 = false; IsWeapon4 = false; }
                //if (i == 1) { IsWeapon1 = false; IsWeapon3 = false; IsWeapon4 = false; }
                //if (i == 2) { IsWeapon1 = false; IsWeapon2 = false; IsWeapon4 = false; }
                //if (i == 3) { IsWeapon1 = false; IsWeapon2 = false; IsWeapon3 = false;}
            }
            i++;
        }
    }

    void Update()
    {
        //if (!_photonView.IsMine) { return; }

            //метод для переключения оружия: переменная предыдущего оружия. 
            //Проверка предыдущего и выбранного: если не равны, то вызываем метод SelectWeapon()

            //int previousSelectWeaponID = weaponID;

        //Контролирование нажатие клавиш
        //Отследить нажатие нужной кнопки:
        //классическое для игр решение: отслеживаем по колесу или 1-5 верхних клавиш
        //Обращаемся к Input: спрашиваем, нажата ли клавиша, здесь - поворот колеса мышки

        if (Photon.IsMine)
        {
            int previousSelectWeaponID = weaponID;

            if (Input.GetAxis("Mouse ScrollWheel") < 0) //вниз
            {

                if (weaponID <= 0)
                {
                    weaponID = ChCountWeapon - 1;
                }
                else
                {
                    weaponID--;
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0) //вверх
            {

                if (weaponID >= ChCountWeapon - 1 )
                {
                    weaponID = 0;
                }
                else
                {
                    weaponID++;
                }
            }

            //если что-то поменялось, то только тогда вызываем наш метод:
            if (previousSelectWeaponID != weaponID)
            {
                SelectWeapon();
            }

        }
        else
        {
            return;
        }

        //else //Not My Photon
        //{
            

        //    //если что-то поменялось, то только тогда вызываем наш метод:
        //   // if (previousSelectWeaponID != weaponID)
        //    {
        //        Photon.RPC("SelectWeapon", RpcTarget.All);
        //    }
        //    //int previousSelectWeaponID = weaponID;
        //}
    }

}
