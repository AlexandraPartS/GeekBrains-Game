using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public struct PlayerData
{
    public string PLName;
    public int PLHealth;
    public bool PLDead;

    public override string ToString()
    {
        return $"Name:{PLName} Helth:{PLHealth} Dead:{PLDead}";
    }
}

public class SinglePlayer : Unit//, IPunObservable
{
        //Переменные для механизма выделения объекта
        [SerializeField] private Shader Outline;
        [SerializeField] private Shader Base;
        [SerializeField] private bool Select; //Выбран у нас объект или нет
                                              //[SerializeField] private bool isSelect;
        [SerializeField] private bool isRed;
        [SerializeField] private GameObject TempGO; //Переменная для временного GO. который мы будем выделять
        private RaycastHit Hit; //
        [SerializeField] private Transform MCam;
        private AudioListener _AudioListener;

        private ISaveData _data;

        private FirstPersonController _controller;
        public Slider sliderMyHealth;

        //public GameObject CameraDead;
        //public GameObject CameraDeadParent;
        //public GameObject PanelDead;

        // Через bool
        //public GameObject GOWeapon1;
        //public GameObject GOWeapon2;
        //public GameObject GOWeapon3;
        //public GameObject GOWeapon4;
        //        [SerializeField] private bool isWeapon1Act;
        //        [SerializeField] private bool isWeapon2Act;
        //        [SerializeField] private bool isWeapon3Act;
        //        [SerializeField] private bool isWeapon4Act;
        // [SerializeField] private int childNumber;



        //#region IPunObservable implementation
        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    if (stream.IsWriting)
        //    {   // We own this player: send the others our data
        //        stream.SendNext(isRed);

        //       // stream.SendNext(childNumber);
        //        //stream.SendNext(isWeapon1Act);
        //        //stream.SendNext(isWeapon2Act);
        //        //stream.SendNext(isWeapon3Act);
        //        //stream.SendNext(isWeapon4Act);
        //    }
        //    else
        //    {   // Network player, receive data
        //        isRed = (bool)stream.ReceiveNext();

        //        //childNumber = (int)stream.ReceiveNext();
        //        //isWeapon1Act = (bool)stream.ReceiveNext();
        //        //isWeapon2Act = (bool)stream.ReceiveNext();
        //        //isWeapon3Act = (bool)stream.ReceiveNext();
        //        //isWeapon4Act = (bool)stream.ReceiveNext();
        //    }
        //}
        //#endregion




    [PunRPC]
    void GoToRed()
    {
        GetComponent<Renderer>().material.color = Color.red;
        isRed = true;

    }
    [PunRPC]
    void GoToBackColor()
    {
        GetComponent<Renderer>().material.color = Color.white;
        isRed = false;
    }
    
    void Start()
    {
        //подставляет метод, который ходим: классы, кот.написаны дальше, будут реализовывать интерфейс и мы сюда будет подставлять создание нового класса: создается экземпляр класса и дальше через будем им пользоваться
        //_data = new XMLData();
        //_data = new JSONData();
        _data = new StreamData();

        _controller = GetComponent<FirstPersonController>();

        MCam = MainCamera.transform;
        _AudioListener = GetComponentInChildren<AudioListener>();
        sliderMyHealth = GameObject.FindGameObjectWithTag("CanvasManager").transform.GetChild(2).GetChild(1).GetComponent<Slider>();

        // Через bool
        //GOWeapon1 = transform.Find("Flashlight").gameObject;
        //GOWeapon2 = transform.Find("Knife").gameObject;
        //GOWeapon3 = transform.Find("Glock").gameObject;
        //GOWeapon4 = transform.Find("SCAR").gameObject;
        //childNumber = GetComponentInChildren<ChangeWeapon>().weaponID;

        if (!Photon.IsMine)
        {
            MCam.GetComponent<Camera>().enabled = false;
            _controller.enabled = false;
            _AudioListener.enabled = false;
        }
        else
        {
            _controller.enabled = true;
            MCam.GetComponent<Camera>().enabled = true;
            _AudioListener.enabled = true;

        }
        Health = 300;
        sliderMyHealth.maxValue = Health;

        //Забрать шейдеры
        Outline = Shader.Find("Nature/Terrain/Specular");
        Base = Shader.Find("Legacy Shaders/Bumped Specular");
        //Outline = Shader.Find("Toon/Basic Outline");
        //Outline = Shader.Find("Mobile/Particles/Additive");
        //Base = Shader.Find("Standard");

        PlayerData SinglePlayer = new PlayerData
        {
            PLName = name,
            PLHealth = Health,
            PLDead = Dead
        };

        _data.Save(SinglePlayer);
        PlayerData NEWPlayer = _data.Load();

        //Workong with PlayerPrefs
        //PlayerPrefs.SetString("Name", SinglePlayer.PLName);
        //PlayerPrefs.SetInt("Health", SinglePlayer.PLHealth);
        //PlayerPrefs.Save(); //концепция: чем меньше трогать, тем лучше
        //Debug.Log(PlayerPrefs.GetString("Name"));

        //PlayerPrefs.DeleteAll();
        //Debug.Log(PlayerPrefs.GetString("Name"));
        Debug.Log(NEWPlayer);

    }


    //photonView.Owner
    [PunRPC]
    public void DestroyPlayer()
    {
        GetComponent<FirstPersonController>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        this.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
        gameObject.layer = 0;
        gameObject.tag = "Untagged";
        gameObject.GetComponent<ToFire>().enabled = false;
        gameObject.GetComponentInChildren<AudioSource>().enabled = false;
        //gameObject.GetComponent<AudioListener>().enabled = false;
        //Destroy(GOInstance, 0f);
        //OnPlayerLeftRoom();
        //UnityEngine.SceneManagement.SceneManager.LoadScene("LoseScene");
    }



    void Update()
    {
            if (Photon.IsMine && PhotonNetwork.IsConnected == true)
            {
                //1. Функция работы с лежищими объектами PickUp
                if (Physics.Raycast(MCam.position, MCam.forward, out Hit, 5f))
                {
                    Collider target = Hit.collider;

                    if (target.tag == "Pickup")
                    {
                        if (!TempGO) //механизм снятия выделения
                        {
                            TempGO = target.gameObject;
                        }
                        Select = true;
                        if (TempGO.GetInstanceID() != target.gameObject.GetInstanceID())
                        {
                            Select = false;
                            TempGO = null;
                        }
                    }
                    else { Select = false; }
                }
                else { Select = false; }

                //1.2.
                if (Select)
                {
                foreach (Transform item in TempGO.transform) //обходим все объекты корневого
                {
                    if (item.GetComponent<Renderer>()) //если дочерний имеет Renderee - это значит, что мы можем с ним работать
                    {
                        item.GetComponent<Renderer>().material.shader = Outline;

                    }
                }
            }
            else
                {
                     if (TempGO)
                     {
                        foreach (Transform item in TempGO.transform)
                        {
                            if (item.GetComponent<Renderer>())
                            {
                                item.GetComponent<Renderer>().material.shader = Base;
                            }
                        }
                     }
                }


                //2. Функция переключения цвета капсулы игрока для проверки PhotonObserved
                //if(Input.GetKey(KeyCode.Space))
                //{
                //    GoToRed();
                //}
                //else
                //{
                //    GetComponent<Renderer>().material.color = Color.green;
                //    isRed = false;
                //}
                //GoToRed();
                if (Input.GetKey(KeyCode.Space))
                {
                    Photon.RPC("GoToRed", RpcTarget.All);
                }
                else
                {
                    Photon.RPC("GoToBackColor", RpcTarget.All);
                }


            //3. Функция-посредник для извлечения номера активного оружия
            // Через bool
            //if (GOWeapon1.activeSelf) {Debug.Log("IsWeapon1 = true");isWeapon1Act = true;} else { isWeapon1Act = false;}
            //if (GOWeapon2.activeSelf) { Debug.Log("IsWeapon2 = true"); isWeapon2Act = true; }else  {isWeapon2Act = false; }
            //if (GOWeapon3.activeSelf) { Debug.Log("IsWeapon3 = true"); isWeapon3Act = true; } else { isWeapon3Act = false; }
            //if (GOWeapon4.activeSelf) {Debug.Log("IsWeapon4 = true"); isWeapon4Act = true;  } else{ isWeapon4Act = false; }
            //UpdatechildNumber();

            //childNumber = GetComponentInChildren<ChangeWeapon>().weaponID;

            sliderMyHealth.value = Health;

                //4. Смерть
                if (Dead)
                {
                    GameObject.FindGameObjectWithTag("CanvasManager").transform.GetChild(4).gameObject.SetActive(true);
                    //PanelDead.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Photon.RPC("DestroyPlayer", RpcTarget.All);//photonView.Owner
                }
            }


            ////2.2.
            //if (isRed)
            //{
            //    GetComponent<Renderer>().material.color = Color.red;
            //}
            //else
            //{
            //    GetComponent<Renderer>().material.color = Color.green;
            //}

            // 3.1.
            //childNumber = 
            //GetComponentInChildren<ChangeWeapon>().weaponID = childNumber;
            //Через bool
            //if (isWeapon1Act) {GOWeapon1.SetActive(true);} else { GOWeapon1.SetActive(false); }
            //if (isWeapon2Act) { GOWeapon2.SetActive(true); } else { GOWeapon2.SetActive(false); }
            //if (isWeapon3Act) { GOWeapon3.SetActive(true); } else { GOWeapon3.SetActive(false); }
            //if (isWeapon4Act){ GOWeapon4.SetActive(true); }else { GOWeapon4.SetActive(false); }

    }

        //private IEnumerator UpdatechildNumber()
        //{
        //    while (EnemyCounter < count)
        //    {
        //        yield return new WaitForSeconds(Random.Range(4, 50));
        //        Create("Prefabs/Enemy");
        //        EnemyCounter++;
        //    }
        //}

}


//void Update()
//{
//    if (!Photon.IsMine && PhotonNetwork.IsConnected == true)
//    { return; }

//    if (Physics.Raycast(MCam.position, MCam.forward, out Hit, 5f))
//    {
//        Collider target = Hit.collider;

//        if (target.tag == "Pickup")
//        {
//            if (!TempGO) //механизм снятия выделения
//            {
//                TempGO = target.gameObject;
//            }
//            Select = true;
//            if (TempGO.GetInstanceID() != target.gameObject.GetInstanceID())
//            {
//                Select = false;
//                TempGO = null;
//            }
//        }
//        else
//        {
//            Select = false;
//        }
//    }
//    else
//    {
//        Select = false;
//    }

//    if (Select)
//    {
//        foreach (Transform item in TempGO.transform) //обходим все объекты корневого
//        {
//            if (item.GetComponent<Renderer>()) //если дочерний имеет Renderee - это значит, что мы можем с ним работать
//            {
//                item.GetComponent<Renderer>().material.shader = Outline;
//            }
//        }
//    }
//    else
//    {
//        if (TempGO)
//        {
//            foreach (Transform item in TempGO.transform)
//            {
//                if (item.GetComponent<Renderer>())
//                {
//                    item.GetComponent<Renderer>().material.shader = Base;
//                }
//            }
//        }
//    }

//    //isChangeWeapon = transform.GetComponentInChildren<ChangeWeapon>().Equals(isChangeWeapon);
//}
//}
