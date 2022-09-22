using PointBlank.Core.Models.Enums;
using System;
using System.Globalization;

namespace PointBlank.Core.Models.Account
{
  public class Message
  {
    public string sender_name = "";
    public string text = "";
    public int object_id;
    public int clanId;
    public int type;
    public int state;
    public long sender_id;
    public long expireDate;
    public NoteMessageClan cB;
    public int DaysRemaining;

    public Message()
    {
    }

    public Message(long expire, DateTime start)
    {
      this.expireDate = expire;
      this.SetDaysRemaining(start);
    }

    public Message(double days)
    {
      DateTime end = DateTime.Now.AddDays(days);
      this.expireDate = long.Parse(end.ToString("yyMMddHHmm"));
      this.SetDaysRemaining(end, DateTime.Now);
    }

    private void SetDaysRemaining(DateTime now)
    {
      this.SetDaysRemaining(DateTime.ParseExact(this.expireDate.ToString(), "yyMMddHHmm", (IFormatProvider) CultureInfo.InvariantCulture), now);
    }

    private void SetDaysRemaining(DateTime end, DateTime now)
    {
      int num = (int) Math.Ceiling((end - now).TotalDays);
      this.DaysRemaining = num < 0 ? 0 : num;
    }
  }
}
