using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ToFire : BaseWeapon
{
    public GameObject activeWeapon;
    private Transform weaponParent;
    private int weaponID;
    //public int tempBulletCount;
    private int _damage;
    public KeyCode Reload = KeyCode.R; //клавиша перезарядки

    protected override void Awake()
    {
        base.Awake();
    }


    // Start is called before the first frame update
    void Start()
    {
        weaponParent = this.transform.Find("Weapons");
        
        //firingfWeapon = this.GetComponentInChildren<Gun>().gameObject;
        //firingWeapon = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
    }


    // Update is called once per frame
    void Update()
    {
        if (Photon.IsMine)
        {
            //находим активный weapon
            weaponID = this.GetComponent<ChangeWeapon>().weaponID;
            activeWeapon = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(weaponID).gameObject;


            if (activeWeapon.tag == "Weapons") //2. смотрим, огнестрельный ли он
            {
                //1. Fire
                if (Input.GetButtonDown("Fire1"))//если с инпута кнопка "Стрелять" ось - Fire1 - левая кнопка мыши
                {
                    _damage = activeWeapon.GetComponent<Gun>()._damage;
                    Photon.RPC("Fire", RpcTarget.All, weaponID);
                }
                //2. Reload gun
                if (Input.GetKeyDown(Reload))
                {
                    Photon.RPC("ReloadBullet", RpcTarget.All, weaponID);
                }
            }

            //3.Flashlight
            if (activeWeapon.GetComponent<Flashlight>())
            {
                //Activate GUI of battery
                GameObject.FindGameObjectWithTag("CanvasManager").transform.GetChild(3).gameObject.SetActive(true);

                //battery charge> 0
                if (activeWeapon.GetComponent<Flashlight>().FlashLightWorkable) 
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (!activeWeapon.GetComponentInChildren<Light>().enabled)
                        {
                            Photon.RPC("OnOff", RpcTarget.All, weaponID, true);
                        }
                        else
                        {
                            Photon.RPC("OnOff", RpcTarget.All, weaponID, false);
                            
                        }
                    }
                    if (activeWeapon.GetComponent<Flashlight>().TimerWorkable == false)
                    {
                        Photon.RPC("ReloadTimer", RpcTarget.All, weaponID);
                    }
                }
                else // battery isover
                {
                    Photon.RPC("OnOff", RpcTarget.All, weaponID, false);
                    activeWeapon.GetComponent<Flashlight>().timeoutSlider.value = 0;
                }

                if (activeWeapon.GetComponentInChildren<Light>().enabled)
                {
                    activeWeapon.GetComponent<Flashlight>().currTime += Time.deltaTime;
                    activeWeapon.GetComponent<Flashlight>().sumCurrTime += Time.deltaTime;

                    if (activeWeapon.GetComponent<Flashlight>().currTime > activeWeapon.GetComponent<Flashlight>().timeout)
                    {
                        activeWeapon.GetComponent<Flashlight>().TimerWorkable = false;
                        activeWeapon.GetComponent<Flashlight>().currTime = 0;
                    }
                }
                if ((activeWeapon.GetComponent<Flashlight>().batteryCharge - activeWeapon.GetComponent<Flashlight>().sumCurrTime) <= 0)
                {
                    activeWeapon.GetComponent<Flashlight>().FlashLightWorkable = false;
                    activeWeapon.GetComponent<Flashlight>().timeoutSlider.gameObject.SetActive(false);
                    //OnOff(false);
                }

                activeWeapon.GetComponent<Flashlight>().timeoutSlider.value = activeWeapon.GetComponent<Flashlight>().timeout - activeWeapon.GetComponent<Flashlight>().currTime;
                activeWeapon.GetComponent<Flashlight>().butterySlider.value = activeWeapon.GetComponent<Flashlight>().batteryCharge - activeWeapon.GetComponent<Flashlight>().sumCurrTime;

            }
            else
            {
                GameObject.FindGameObjectWithTag("CanvasManager").transform.GetChild(3).gameObject.SetActive(false);
            }

        }
        else 
        { 
            return; 
        }
    }

    [PunRPC]
    IEnumerator WaitAndLight(float waitTime, int weaponID)
    {

        yield return new WaitForSeconds(waitTime);
        //1. Находим нужное оружие
        Transform weaponParent = this.gameObject.transform.GetChild(0).GetChild(0);
        GameObject activeWeapon = weaponParent.GetChild(weaponID).gameObject;

        activeWeapon.GetComponent<Flashlight>().currTime = 0;
        activeWeapon.GetComponent<Flashlight>().TimerWorkable = true;
    }




    [PunRPC]
    public void ReloadTimer(int weaponID)
    {
        //1. Находим нужное оружие
        Transform weaponParent = this.gameObject.transform.GetChild(0).GetChild(0);
        GameObject activeWeapon = weaponParent.GetChild(weaponID).gameObject;

        activeWeapon.GetComponent<Flashlight>().TimerWorkable = false;
        activeWeapon.GetComponent<Flashlight>().currTime = 0;
        activeWeapon.GetComponentInChildren<Light>().enabled = false;

        StartCoroutine(WaitAndLight(0.5f, weaponID));
        //_fire = false; //стрелять нельзя
        //Anim.SetTrigger("reload");
        //_audio.PlayOneShot(_gunSound[1]);
    }



    [PunRPC]
    private void OnOff(int weaponID, bool val)
    {
        //1. Находим нужное оружие
        Transform weaponParent = this.gameObject.transform.GetChild(0).GetChild(0);
        GameObject activeWeapon = weaponParent.GetChild(weaponID).gameObject;

        activeWeapon.GetComponentInChildren<Light>().enabled = val;
    }




    [PunRPC]
    IEnumerator WaitAndFire(float waitTime, int weaponID)
    {
        
        yield return new WaitForSeconds(waitTime);
        //Photon.RPC("ReadyGun", RpcTarget.All, weaponID);
        print("WaitAndPrint " + Time.time);
        //1. Находим нужное оружие
        Transform weaponParent = this.gameObject.transform.GetChild(0).GetChild(0);
        GameObject activeWeapon = weaponParent.GetChild(weaponID).gameObject;

        activeWeapon.GetComponent<Gun>()._bulletCount = 6;
        activeWeapon.GetComponent<Gun>()._fire = true;
    }


    [PunRPC]
    public void ReloadBullet(int weaponID)
    {
        //1. Находим нужное оружие
        Transform weaponParent = this.gameObject.transform.GetChild(0).GetChild(0);
        GameObject activeWeapon = weaponParent.GetChild(weaponID).gameObject;

        activeWeapon.GetComponent<Gun>()._fire = false;
        activeWeapon.GetComponent<Animator>().SetTrigger("reload");
        _audio.PlayOneShot(activeWeapon.GetComponent<Gun>()._gunSound[1]);
        print("Starting " + Time.time);

        StartCoroutine(WaitAndFire(2.5f, weaponID));
        //_fire = false; //стрелять нельзя
        //Anim.SetTrigger("reload");
        //_audio.PlayOneShot(_gunSound[1]);
    }
    //[PunRPC]
    //public void ReadyGun(int weaponID)
    //{
    //    //1. Находим нужное оружие
    //    Transform weaponParent = this.gameObject.transform.GetChild(0).GetChild(0);
    //    GameObject activeWeapon = weaponParent.GetChild(weaponID).gameObject;

    //    activeWeapon.GetComponent<Gun>()._bulletCount = 6;
    //    activeWeapon.GetComponent<Gun>()._fire = true;
    //    print("Done " + Time.time);
    //}


    [PunRPC]
    public void Fire(int weaponID)
    {
        //1. Находим нужное оружие
        Transform weaponParent = this.gameObject.transform.GetChild(0).GetChild(0);
        GameObject activeWeapon = weaponParent.GetChild(weaponID).gameObject;

        //1) Есть ли у нас патроны и разрешено ли нам стрелять
        if (activeWeapon.GetComponent<Gun>()._bulletCount > 0 && activeWeapon.GetComponent<Gun>()._fire)
        {
            activeWeapon.GetComponentInChildren<ParticleSystem>().Play();
            _audio.PlayOneShot(activeWeapon.GetComponent<Gun>()._gunSound[0]);
            activeWeapon.GetComponent<Animator>().SetTrigger("shoot");
            activeWeapon.GetComponent<Gun>()._bulletCount--;

            //_audio.PlayOneShot(audio);  //audio
            //Anim.SetTrigger("shoot"); //animation shoot  
            //_muzzleFlash.Play(); 
            //_bulletCount--;

        //2) Реализация стрельбы: если префаб и если нет
        if (activeWeapon.GetComponent<Gun>().prefab)
            {
                RaycastHit hit; //создадим другой RaycastHit, с др.настройками луча: лететь будет в прицел

                Ray ray = MainCamera.ScreenPointToRay(_crossHair.transform.position); //луч из камеры, из того места, где находится прицел. Из экранных координат мировые.
                GameObject temp = Instantiate(activeWeapon.GetComponent<Gun>()._bullet, activeWeapon.GetComponent<Gun>()._gunT.position, Quaternion.identity);

                Rigidbody BulletRB = temp.GetComponent<Rigidbody>(); //Нужно получить Rigidbody объекта, т.к. лететь будет по физике и толкать будем физикой
                activeWeapon.GetComponent<Gun>().line.SetPosition(0, activeWeapon.GetComponent<Gun>()._gunT.position); //1) индекс массива, 2) координаты. в которые массив запишем

                if (Physics.Raycast(ray, out hit, activeWeapon.GetComponent<Gun>()._shootDistance)) //смотрим, попали мы куда-то не попали //передаем лучик, результат в hit, дистанция луча
                {
                    //нам нужно расчитать вектор направления к цели, имея 2 точки - стартовую и конечную
                    //Для придания силы вектору, умножим на _shootDistance (один из возможных множителей)
                    BulletRB.AddForce(GetDirection(hit.point, temp.transform.position) * activeWeapon.GetComponent<Gun>()._shootDistance, ForceMode.Impulse);
                    activeWeapon.GetComponent<Gun>().line.SetPosition(1, hit.point); //трассировка линии полета пули от точки 1 до точки 2
                    SetDamage(hit.collider.GetComponent<ISetDamage>());
                }
                else
                {   //если стрельнули вникуда (меняестся в итоге точка назначения только)
                    BulletRB.AddForce(GetDirection(ray.GetPoint(10000f), temp.transform.position) * activeWeapon.GetComponent<Gun>()._shootDistance, ForceMode.Impulse); //привяжем конечную точку к лучу
                    activeWeapon.GetComponent<Gun>().line.SetPosition(1, ray.GetPoint(10000f)); //трассировка линии полета пули от точки 1 до точки 2
                }
            }
            else
            {
                RaycastHit hit;
                Ray ray = new Ray(activeWeapon.GetComponent<Gun>().TMcam.position, activeWeapon.GetComponent<Gun>().TMcam.forward);
                //занимаемся райкастом
                if (Physics.Raycast(ray, out hit, activeWeapon.GetComponent<Gun>()._shootDistance))
                {
                    //если куда-то попали. Проверяем, куда попали
                    if (hit.collider.tag == "Player")
                    {
                        return;
                    }
                    else
                    {
                        //путь по тегам очень плох: цикл долго по ним бегает - неправильно
                        //воспользуемся интерфейсом: метод SetDamage, кот.наносит урон.
                        //решение вопроса-проблемы - множественной проверки по тегам
                        SetDamage(hit.collider.GetComponent<ISetDamage>());
                        //надо создать частицы. сделаем это через метод (выше) - правильнее с т.з.реализации
                        CreateParticleHit(hit);
                    }
                }
            }
        }

        //4. Возвращаем данные по ссылкам
        //tempBulletCount = _bulletCount;
        //activeWeapon.GetComponent<Gun>()._bulletCount = tempBulletCount;

    }

    //метод, который создает частицы
    private void CreateParticleHit(RaycastHit hit)
    {
        GameObject tempHit = Instantiate(_hitParticle, hit.point, Quaternion.identity); //tempHit - тот объект, в кот.поместим частицы
        //Для того, чтобы частицы прилепились к объекту, на случай, если объект двигается и задвигались с ним
        tempHit.transform.parent = hit.transform;
        Destroy(tempHit, 0.5f); //не пуллинг, т.к. не всегда оптимален.
    }

    //Метод для  расчета вектора полета префаба, в кот.будем передавать стартовую и конечную точку, а возвращать вектор
    private Vector3 GetDirection(Vector3 HitPoint, Vector3 bulletPos)
    {
        Vector3 decr = HitPoint - bulletPos;
        float dist = decr.magnitude;
        return decr / dist;
    }

    //наносит урон и содержит интерфейс.
    private void SetDamage(ISetDamage obj)
    {
        if (obj != null)
        {
            obj.SetDamage(_damage);
        }
    }


}
