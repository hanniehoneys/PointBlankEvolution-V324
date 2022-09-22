using System;
using System.Threading;

namespace PointBlank.Core.Network
{
  public class TimerState
  {
    public Timer Timer = (Timer) null;
    public DateTime EndDate = new DateTime();
    private object sync = new object();

    public void Start(int period, TimerCallback callback)
    {
      lock (this.sync)
      {
        this.Timer = new Timer(callback, (object) this, period, -1);
        this.EndDate = DateTime.Now.AddMilliseconds((double) period);
      }
    }

    public int getTimeLeft()
    {
      if (this.Timer == null)
        return 0;
      int totalSeconds = (int) (this.EndDate - DateTime.Now).TotalSeconds;
      if (totalSeconds < 0)
        return 0;
      return totalSeconds;
    }
  }
}
