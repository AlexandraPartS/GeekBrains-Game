/* 1. Запросим все нужные нам ссылки
 * 2. Предоставить к ним каким-то образом доступ
 *  классика: использование свойств(полей) - спец.методов, которые дают доступ к переменным
 *  -> инкапсулируем - создаем свойства
 *  
 *  в начале получаем instance: возвращает значение, содержит метод get
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Базовый класс для кэширования ссылок, базовый класс для всеъ объектов.
/// </summary>
public abstract class BaseObject : MonoBehaviourPunCallbacks
{
    private PhotonView _photon;

    private Transform gOTransform;
    private GameObject gOInstance;
    private string _name;
    private bool isVisible;
    //private bool isWeapon1;
    //private bool isWeapon2;
    //private bool isWeapon3;
    //private bool isWeapon4;
    //private string _Flashlight, _Knife, _Glock, _SCAR;


    private Vector3 position;
    private Vector3 scale;
    private Quaternion rotation;

    private Material material;
    protected Color _color;

    private Rigidbody _rigidbody;

    private Camera mainCamera;
    private Animator anim; //Важный компонент, который часто будет использоваться.
    

    protected Transform GOTransform { get => gOTransform; } //Было бы неплохо сделать проверку 
    //существования трансформа, но вряд ли при авейк объект не будет существовать, когда будет проинициализирован класс.
    protected GameObject GOInstance { get => gOInstance;}
    protected Camera MainCamera { get => mainCamera;}
    protected Material Material { get => material;}
    /// <summary>
    /// Аниматор
    /// </summary>
    protected Animator Anim { get => anim;}
    /// <summary>
    /// Количество дочерних объектов
    /// </summary>
    protected int ChCount { get => GOTransform.childCount; } //Свойство для возвращения кол-ва дочерних элементов
    protected Rigidbody RB { get => _rigidbody; }
    protected string Name { get => _name; set => _name = value; }
    protected Vector3 Position { get => position; set => position = value; }
    protected Vector3 Scale { get => scale; set => scale = value; }
    protected Quaternion Rotation { get => rotation; set => rotation = value; }
    //Проверяем, есть ли у нас Renderer, если есть, то получаем поле enable - включен или отключен компонент
    //поменяем реализацию set
    protected bool IsVisible 
    { 
        get => isVisible;
        set
        {
            isVisible = value;
            if (gOInstance.GetComponent<Renderer>())
            {
                gOInstance.GetComponent<Renderer>().enabled = isVisible;
            }
            
        }
    }

    public PhotonView Photon { get =>  _photon; }

    //public bool IsWeapon1 { get => isWeapon1; set => isWeapon1 = value; }
    //public bool IsWeapon2 { get => isWeapon2; set => isWeapon2 = value; }
    //public bool IsWeapon3 { get => isWeapon3; set => isWeapon3 = value; }
    //public bool IsWeapon4 { get => isWeapon4; set => isWeapon4 = value; }
    //public Transform GOTransformWeapon { get => goTransformWeapon; set => goTransformWeapon = value; }



    //Инициализация: положим в поля необходимые ссылки:
    #region 
    protected virtual void Awake() //метод, который нам ничего не возвращает.
    {
        gOInstance = gameObject;
        gOTransform = gameObject.transform; // = GetComponent
        Name = gameObject.name;
        //mainCamera = Camera.main;
        //_Flashlight = "Flashlight"; _Knife = "Knife"; _Glock = "Glock"; _SCAR = "SCAR";
        //Далее компоненты, которые могут быть, и которые могут и не быть.
        if (GetComponent<PhotonView>())
        {
            _photon = GetComponent<PhotonView>();
            //_photon = PhotonView.Get(this);
            if(_photon.IsMine)
            {
                mainCamera = Camera.main;
            }
            else
            {
                mainCamera = transform.GetComponentInChildren<Camera>();
            }
        }
        else
        {
            if(gameObject.tag == "Weapons")
            {
                //GameObject GO = transform.GetComponentInParent<SinglePlayer>().gameObject;
                mainCamera = GetComponentInParent<SinglePlayer>().mainCamera;
                //if (gameObject.name == "Flashlight") { isWeapon1 = gameObject.activeInHierarchy; }
                //if (gameObject.name == "Knife") { isWeapon2 = gameObject.activeInHierarchy; }
                //if (gameObject.name == "Glock") { isWeapon3 = gameObject.activeInHierarchy; }
                //if (gameObject.name == "SCAR") { isWeapon4 = gameObject.activeInHierarchy; }
            }
        }


        if(GetComponent<Rigidbody>())  //Тестовый запрос на предмет, есть ли у объекта Rigidbody.
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        if (GetComponent<Animator>())  // - //- 
        {
            anim = GetComponent<Animator>();
        }
        if (GetComponent<Renderer>())  // - //- 
        {
            material = GetComponent<Renderer>().material ;
        }
    }
    #endregion
}
