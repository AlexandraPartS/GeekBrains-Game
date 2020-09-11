/*  1. переменные: время уничтожения, урон
 *  */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BaseObject
{
    private int _damage = 20;
    private float destructTime = 10; //хранение времени уничтожения от пули
    //ДеструктТаймер зависит от логики игры: как ведет себя пуля в конце своего "ЖЦ". Вариант, когда игроки просто "выбрасывают" 
    //объекты, то эти объекты должны уничтожаться, чтоюы не "съедать" память

    protected override void Awake()
    {
        base.Awake();
        Destroy(GOInstance, destructTime);
    }

    protected virtual void SetDamage(ISetDamage obj)
    {
        if(obj !=null)
        {
            obj.SetDamage(_damage);
        }
    }

    //Метод столкновения пули с коллайдером - в режиме коллайдера (не триггера)
    private void OnCollisionEnter(Collision collision)
    {
        SetDamage(collision.gameObject.GetComponent<ISetDamage>());
        Destroy(gameObject);
    }


}
