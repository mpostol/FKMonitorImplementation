//TODO Remove all useless using directives
using MethodDecorator.Fody.Interfaces;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class HoareMonitorFody : Attribute, IMethodDecorator  //TODO where we are using this attribute?
{
  public void Init(object instance, MethodBase method, object[] args)
  { }

  public void OnEntry()
  {
    Monitor.Enter(this);
    Console.WriteLine($"[Intercepted] Entering Method"); //TODO using Console for debugging purposes is not allowed. Use UT instead !!!
  }

  public void OnExit()
  {
    Monitor.Exit(this);
    Console.WriteLine($"[Intercepted] Exiting Method"); //TODO using Console for debugging purposes is not allowed. Use UT instead !!!
  }

  public void OnException(Exception exception)
  {
    Monitor.Exit(this);
    Console.WriteLine($"[Intercepted] Exception: {exception.Message}");   //TODO using Console for debugging purposes is not allowed. Use UT instead !!!
  }
}