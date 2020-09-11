/* Что будет в базовом классе оружия? 
 * - несколько перемнных, касающихся оружия/ Касаться реализации стрельбы: таймер, вспышка, эффекты, звуки...
 */

using UnityEngine;

public abstract class BaseWeapon : BaseObject
{
    public Transform _gunT;
    protected ParticleSystem _muzzleFlash; //Система частиц - вспышка

    //для того, чтобы заработала система частиц от попадания, необходимо создать объект в том месте, куда попали, когда нажали на кнопку мыши
    //создаем этот объект, переместить его туда каким-то образом и проиграть анимацию частиц, удалить этот объект
    //объект, который изображает из себя частицы
    protected GameObject _hitParticle;

    //переменная для прицела - для стрельбы префабами
    //прицел м.б. просто добавлен или мы можем его найти по тегу. Но делаем автоматически назначаемым, т.к. у нас все будет динамически переключаться.
    [SerializeField] protected GameObject _crossHair;

    //инициализация таймера
    protected Timer _rechargeTimer = new Timer();

    //флаг, который будет разрешать или запрещать это процесс, чтобы его м.б. контролировать
    public bool _fire = true;

    //переменные для аудиоклипов
    public AudioClip[] _gunSound;
    public AudioSource _audio;



    //Метод, который отвечает за стрельбу
   //public abstract void Fire();


    protected override void Awake()
    {
        base.Awake();

        _gunT = GOTransform.GetChild(2); //Вольность, т.к. слишком частный случай: мало оружия и у всех gunT 
        //занимает второе место в иерархии детей. По-хорошему надо искать по тегу.

        //Вспышки будем забирать из внутренних компонентов. Дочерний элементы gunT скорее всего будет содержать ParticleSystem (и он там будет один)
        _muzzleFlash = GetComponentInChildren<ParticleSystem>();
        //частицы имитации попадания: загружаем из ресурсов. Загружаем GameObject-префаб, передаем, где расположен.
        _hitParticle = Resources.Load<GameObject>("Prefabs/Flare");
        _crossHair = GameObject.FindGameObjectWithTag("Cross");
        _audio = GameObject.FindObjectOfType<AudioSource>(); 
    }

    void Update()
    {
        _rechargeTimer.Update();
        if(_rechargeTimer.IsEvent())
        {
            _fire = true;
        }
    }
}
