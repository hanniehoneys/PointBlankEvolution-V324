using PointBlank.Core.Models.Account;
using System;
using System.Collections.Generic;

namespace PointBlank.Core.Managers.Events
{
  public class EventVisitModel
  {
    public int checks = 7;
    public string title = "";
    public List<VisitBox> box = new List<VisitBox>();
    public int id;
    public uint startDate;
    public uint endDate;

    public EventVisitModel()
    {
      for (int index = 0; index < 7; ++index)
        this.box.Add(new VisitBox());
    }

    public bool EventIsEnabled()
    {
      uint num = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
      return this.startDate <= num && num < this.endDate;
    }

    public VisitItem getReward(int idx, int rewardIdx)
    {
      try
      {
        return rewardIdx == 0 ? this.box[idx].reward1 : this.box[idx].reward2;
      }
      catch
      {
        return (VisitItem) null;
      }
    }

    public void SetBoxCounts()
    {
      for (int index = 0; index < 7; ++index)
        this.box[index].SetCount();
    }
  }
}
