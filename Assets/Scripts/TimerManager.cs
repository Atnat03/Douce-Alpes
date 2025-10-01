using System.Collections;

public interface ITimer
{
    public float elapsedTime {get;set;}
    
    public IEnumerator Timer(float time);
}
