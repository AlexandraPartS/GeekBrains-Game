using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor;
using Photon.Pun;
// запрашиваемый компонент NavMeshAgent: нельзя удалить NavMeshAgent, т.к.используется для Bot, и нельзя Bot настроить другой меш
[RequireComponent(typeof(NavMeshAgent))]


public class Bot : Unit
{
    //[ContextMenuItem("Значение по умолчанию", nameof(Default))]
    private NavMeshAgent _agent;    //ссылка на агента
    private Transform _playerPos;   //позиция игрока
    private Transform Target;       //

    private float _groundChkDistance = 0.1f; //для отслеживания прыжков - стоим или не стоим на земле. Длина луча здесь до земли
    private bool grounded;          //стоим или не на земле


    private float _stoppingDistance = 0.1f; //для "вставания" на точки (state patrul)
    private float _attackDistance = 6; //остановка Бота, чтобы не "толкал". 
    private float _seekDistance = 2;

    //структура данных для хранения точек для патрулирования - список или массив - используем лист - более динамически используемый
    [SerializeField] private List<Vector3> _wayPoints = new List<Vector3>();
    private int _wayCounter;        //итератор
    //отладочный скрипт, который будет переключаться, прежде чем все отладочные скрипты пойдут в билд. Оборачиваем в платформозависимые 
    //флаги компиляции - Unity Editor, тогда все считается, в остальных случаях компилятор вырезает все, что лежит между двумя флагами
    //разделение обязательно

    //сначала сохраняем данные из редактора, потом из игры в либо scriptableObject, либо просто файл данные с патрулируемой локации 
    //и при загрузке игра будет восстанавливать это файл -XML, либо просто список координат
    //корневой объект для координат. пустышки, кот.интерпретируют себя, как точки
    private GameObject _wayPointMain;

    private float _timeWait = 2f;       //таймер для стояния на точке некоторое время
    private float _timeOut;

    //Gun
    [Header("Настройка оружия")]
    [Tooltip("Объект должен находиться на дуле оружия, добавляется автоматически по тегу GunT")]
    [SerializeField] private Transform _gunT;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private GameObject _hitParticle;
    [SerializeField] private int _bulletCount = 30;             //кол-во патронов
    [SerializeField] private float _shootDistance = 1000f;      //дистанция, на кот.стреляем (для луча)
    [SerializeField] private int _damage = 20;                  //урон, который будет наносить оружие

    [Header("Переменные состояния")]
    [SerializeField] private bool patrol;   //тригеры, изменяющие наше состояние
    [SerializeField] private bool shooting;
    [SerializeField] private bool isTarget;
    [SerializeField] private Slider healthSlider;

    [Header("Поиск целей")]
    [SerializeField] private List<Transform> visibleTargets = new List<Transform>();
    [Header("Настройка зоны видимости")]
    //[ContextMenuItem("Рандомное значение угла обзора", nameof(Randomize))]
    [Range(30,220)][SerializeField] private float _maxAngle = 30;
    [Range(10,40)][SerializeField] private float _maxRadius = 20;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Текст")]
    public string Test;

    [Header("Текст")]
    [TextArea(3,5)]public string Test2;


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position + Vector3.up;
        Handles.color = new Color(1, 0, 1, 0.1f);
        Handles.DrawSolidArc(pos, Vector3.up, Vector3.forward, _maxAngle, _maxRadius);
        Handles.DrawSolidArc(pos, Vector3.up, Vector3.forward, -_maxAngle, _maxRadius);
    }
    [MenuItem("Tools/Рандомное значение угла обзора")]
    private void Randomize()
    {
       // _maxAngle = Random.Range(30,90);
    }
    [ContextMenu("Tools/Значение по умолчанию")]
    private void Default()
    {
        _maxAngle = 30;
        _maxRadius = 20;
        _bulletCount = 30;
        _shootDistance = 1000f;
        _damage = 20;  
    }


#endif
    //Coroutine
    IEnumerator FindTargets(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    //Shoot
   IEnumerator Shoot(RaycastHit playerHit)
    {
        yield return new WaitForSeconds(0.5f);
        _muzzleFlash.Play();
        playerHit.collider.GetComponent<ISetDamage>().SetDamage(_damage);
        GameObject temp = Instantiate(_hitParticle, playerHit.point, Quaternion.identity);
        temp.transform.parent = playerHit.transform;
        Destroy(temp, 0.8f);
        shooting = false;
        Anim.SetTrigger("shoot");
    }


    protected override void Awake()
    {
        base.Awake();
        Health = 100;
        healthSlider.maxValue = Health;
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = true;
        _agent.updateRotation = true;

        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;

        _wayPointMain = FindObjectOfType<PathWaypoint>().gameObject; //Debug

        foreach(Transform T in _wayPointMain.transform)
        {
            _wayPoints.Add(T.position);
        }
        patrol = true;
        _agent.stoppingDistance = _stoppingDistance;

        //GUN
        _gunT = GameObject.FindGameObjectWithTag("GunT").transform; 
        _muzzleFlash = GetComponentInChildren<ParticleSystem>();
        _hitParticle = Resources.Load<GameObject>("Prefabs/Flare");


        //Find Targets Coroutine;
        StartCoroutine("FindTargets", 0.1f);
    }

    private void FindVisibleTargets()
    {
        Collider[] targetInViewRadius = Physics.OverlapSphere(Position, _maxRadius, targetMask);
        for (int i=0; i< targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - Position).normalized;
            float targetAngle = Vector3.Angle(transform.forward, dirToTarget);

            if((-_maxAngle)<targetAngle && targetAngle < _maxAngle)
            {
                float distToTarget = Vector3.Distance(Position, target.position);

                if(!Physics.Raycast((transform.position + Vector3.up), dirToTarget, obstacleMask))
                {
                    if(!visibleTargets.Contains(target))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }

        }
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.5f))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    [PunRPC]
    public void Destroy()
    {
        _agent.ResetPath();
        RB.isKinematic = true;
        Destroy(GOInstance, 5f);
    }

    void Update()
    {
        if(visibleTargets.Count>0)
        {
            patrol = false;
            Target = visibleTargets[0];
            if(Vector3.Distance(Position, Target.position)>_maxRadius)
            {
                visibleTargets.Clear();
            }
        }
        else
        {
            patrol = true;
        }


        //Visible targets
        if(_agent)
        {
            if (Dead)
            {
               // _agent.ResetPath();
                //RB.isKinematic = true;
                //Destroy(GOInstance, 5f);
                Photon.RPC("Destroy", RpcTarget.All);
                return;
            }
            if(_agent.remainingDistance > _agent.stoppingDistance)
            {
                Anim.SetBool("move", true);
            }
            else
            {
                Anim.SetBool("move", false);
            }
            if(patrol)
            {
                if(_wayPoints.Count>1)
                {
                    _agent.stoppingDistance = _stoppingDistance;
                    _agent.SetDestination(_wayPoints[_wayCounter]);

                    if(!_agent.hasPath)
                    {
                        _timeOut += 0.1f;
                        if(_timeOut > _timeWait)
                        {
                            _timeOut = 0;
                            if(_wayCounter<_wayPoints.Count -1)
                            {
                                _wayCounter++;
                            }
                            else
                            {
                                _wayCounter = 0;
                            }
                        }
                    }
                }
                else
                {
                    _agent.stoppingDistance = _attackDistance;
                    _agent.SetDestination(_playerPos.position);
                }
            }
            else
            {
                _agent.stoppingDistance = _attackDistance;
                _agent.SetDestination(Target.position);
                Vector3 pos = transform.position + Vector3.up;
                Ray ray = new Ray(pos, transform.forward);
                RaycastHit hit;
                Debug.DrawRay(ray.origin, ray.direction, Color.green);
                transform.LookAt(new Vector3(Target.position.x, 0f, Target.position.z));
                if(Physics.Raycast(ray, out hit, 500f, targetMask))
                {
                    if(hit.collider.tag == "Player" && !shooting)
                    {
                        _agent.ResetPath();
                        Anim.SetTrigger("shoot");
                        StartCoroutine(Shoot(hit));
                        shooting = true;
                    }
                    else
                    {
                        _agent.stoppingDistance = _seekDistance;
                        _agent.SetDestination(Target.position);
                    }
                }
                else
                {
                    if(Target)
                    {
                        _agent.SetDestination(Target.position);
                    }
                }
            }
           


        }

        healthSlider.value = Health;
    }
}
