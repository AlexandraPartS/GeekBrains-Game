using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;



public class Gun : BaseWeapon//, IPunObservable
{   //что будет в оружии?
    public int _bulletCount = 10; //кол-во патронов
    //дистанция, на кот.стреляем (для луча)
    public float _shootDistance = 1000f;
    public int _damage = 20; //урон, который будет наносить оружие

    public KeyCode Reload = KeyCode.R; //клавиша перезарядки

    //заложим скрипт стрельбы префабами:
    public GameObject _bullet; //сам префаб
    public LineRenderer line;  //отладка механизма стрельбы
    //Про LineRenderer: 1) мы можем его добавить и использовать, 2) не прикреплять к объекту, а создавать динамически - 
    //но это плохо с т.з.производительности - постоянно создавать объект в рантайме

    public bool prefab; //стреляем префабами или нет, галочка для оружия

    public Transform TMcam;
    //[SerializeField] private bool isFiring = false;


    //#region IPunObservable implementation
    //    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //    {
    //        if (stream.IsWriting)
    //        {
    //            // We own this player: send the others our data
    //            stream.SendNext(isFiring);
    //            Debug.Log("we are isFiring_ _ _ _ _ _ __ _ _ _ _ _ __ _ _ _ _ _ __ _ _ _ _ _ __ _ _ _ _ _ _");
    //        }
    //        else
    //        {
    //            // Network player, receive data
    //            this.isFiring = (bool)stream.ReceiveNext();
    //            Debug.Log("isFiring NOW!!!");
    //        }
    //}
    //#endregion


    protected override void Awake()
    {
        base.Awake();
    }


    void Start()
    {
        TMcam = MainCamera.transform;
        //инициализация префабов
        if (prefab)
        {
            //загружаем пулю из ресурсов
            _bullet = Resources.Load<GameObject>("Prefabs/Bullet");
            //параметры для компонента LineRenderer, прикрепленного к выбранному оружию
            line = GetComponent<LineRenderer>();
            line.startWidth = 0.02f;
            line.endWidth = 0.02f;
        }
    }


    ////наносит урон и содержит интерфейс.
    //private void SetDamage(ISetDamage obj)
    //{
    //    if (obj != null)
    //    {
    //        obj.SetDamage(_damage);
    //    }
    //}

    ////метод, который создает частицы
    //private void CreateParticleHit(RaycastHit hit)
    //{
    //    GameObject tempHit = Instantiate(_hitParticle, hit.point, Quaternion.identity); //tempHit - тот объект, в кот.поместим частицы
    //    //Для того, чтобы частицы прилепились к объекту, на случай, если объект двигается и задвигались с ним
    //    tempHit.transform.parent = hit.transform;
    //    Destroy(tempHit, 0.5f); //не пуллинг, т.к. не всегда оптимален.
    //}

    ////Метод для  расчета вектора полета префаба, в кот.будем передавать стартовую и конечную точку, а возвращать вектор
    //private Vector3 GetDirection(Vector3 HitPoint, Vector3 bulletPos)
    //{
    //    Vector3 decr = HitPoint - bulletPos;
    //    float dist = decr.magnitude;
    //    return decr / dist;
    //}


    //public override void Fire()
    //{
    //    //1. Есть ли у нас патроны и разрешено ли нам стрелять
    //    if (_bulletCount > 0 && _fire)
    //    {
    //        Anim.SetTrigger("shoot"); //animation shoot
    //        _audio.PlayOneShot(_gunSound[0]);  //audio
    //        _muzzleFlash.Play();
    //        _bulletCount--;

    //        //2. Реализация стрельбы: если префаб и если нет
    //        if (prefab)
    //        {

    //            RaycastHit hit; //создадим другой RaycastHit, с др.настройками луча: лететь будет в прицел

    //            Ray ray = MainCamera.ScreenPointToRay(_crossHair.transform.position); //луч из камеры, из того места, где находится прицел. Из экранных координат мировые. Луч из того места, где находится принцел
    //            GameObject temp = Instantiate(_bullet, _gunT.position, Quaternion.identity);

    //            Rigidbody BulletRB = temp.GetComponent<Rigidbody>(); //Нужно получить Rigidbody объекта, т.к. лететь будет по физике и толкать будем физикой
    //            line.SetPosition(0, _gunT.position); //1) индекс массива, 2) координаты. в которые массив запишем

    //            if (Physics.Raycast(ray, out hit, _shootDistance)) //смотрим, попали мы куда-то не попали //передаем лучик, результат в hit, дистанция луча
    //            {
    //                //нам нужно расчитать вектор направления к цели, имея 2 точки - стартовую и конечную
    //                //Для придания силы вектору, умножим на _shootDistance (один из возможных множителей)
    //                BulletRB.AddForce(GetDirection(hit.point, temp.transform.position) * _shootDistance, ForceMode.Impulse);
    //                line.SetPosition(1, hit.point); //трассировка линии полета пули от точки 1 до точки 2
    //                SetDamage(hit.collider.GetComponent<ISetDamage>());
    //            }
    //            else
    //            {   //если стрельнули вникуда (меняестся в итоге точка назначения только)
    //                BulletRB.AddForce(GetDirection(ray.GetPoint(10000f), temp.transform.position) * _shootDistance, ForceMode.Impulse); //привяжем конечную точку к лучу
    //                line.SetPosition(1, ray.GetPoint(10000f)); //трассировка линии полета пули от точки 1 до точки 2
    //            }
    //        }
    //        else
    //        {
    //            RaycastHit hit;
    //            Ray ray = new Ray(TMcam.position, TMcam.forward);
    //            //занимаемся райкастом
    //            if (Physics.Raycast(ray, out hit, _shootDistance))
    //            {
    //                //если куда-то попали. Проверяем, куда попали
    //                if (hit.collider.tag == "Player")
    //                {
    //                    return;
    //                }
    //                else
    //                {
    //                    //путь по тегам очень плох: цикл долго по ним бегает - неправильно
    //                    //воспользуемся интерфейсом: метод SetDamage, кот.наносит урон.
    //                    //решение вопроса-проблемы - множественной проверки по тегам
    //                    SetDamage(hit.collider.GetComponent<ISetDamage>());
    //                    //надо создать частицы. сделаем это через метод (выше) - правильнее с т.з.реализации
    //                    CreateParticleHit(hit);
    //                }
    //            }
    //        }
    //    }
    //}




    //private void ReloadBullet() //метод обновляет кол-во патронов
    //{
    //    _bulletCount = 30;
    //    _fire = true;
    //}




    // отслеживаем перезарядку и стрельбу
    void Update()
    {
        //if (!Photon.IsMine){ return; }

        //if (Input.GetButtonDown("Fire1"))//если с инпута кнопка "Стрелять" ось - Fire1 - левая кнопка мыши
        //{
        //    Fire();
        //}
        //if(Input.GetKeyDown(Reload))
        //{
        //    _fire = false; //стрелять нельзя
        //    Anim.SetTrigger("reload");
        //   _audio.PlayOneShot(_gunSound[1]);
        //    ReloadBullet();
        //}
    }
}
