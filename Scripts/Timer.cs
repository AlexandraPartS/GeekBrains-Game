/*таймер, кот.прерывает стрельбу: при нажатии клавиши происходил только один выстер. 
/мы можем сделать при отпускании и отпускании клавиши мыши, но с таймером интереснее.

! так же он не наследуется от MonoBehaviour и от него нельзя унаследоваться
*/

using System;

public class Timer
{
    DateTime _start; //один из способов организации таймера. самый простой
    float _elapsed = -1;
    TimeSpan _duration;

    public void Start(float elapsed) //в старт передаем время, которое осталось до истечения
    {
        _elapsed = elapsed;
        _start = DateTime.Now;
        _duration = TimeSpan.Zero;
    }

    public void Update() //название метода можно поменять, т.к. это уже не те стандартные start и update методы
    {
        if(_elapsed > 0)
        {
            _duration = DateTime.Now - _start;
            if(_duration.TotalSeconds > _elapsed)
            {
                _elapsed = 0;
            }
        }
    }
    //роль флага: если 0, возвращает false/true в зависимости от того, что будет
    public bool IsEvent()
    {
        return _elapsed == 0;
    }
}
