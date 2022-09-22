using System;

namespace PointBlank.Core.Managers.Events
{
  public class PlayTimeModel
  {
    public string _title = "";
    public int _goodReward1;
    public int _goodReward2;
    public long _goodCount1;
    public long _goodCount2;
    public uint _startDate;
    public uint _endDate;
    public long _time;

    public bool EventIsEnabled()
    {
      uint num = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
      return this._startDate <= num && num < this._endDate;
    }

    public long GetRewardCount(int goodId)
    {
      return goodId == this._goodReward1 ? this._goodCount1 : (goodId == this._goodReward2 ? this._goodCount2 : 0L);
    }
  }
}
