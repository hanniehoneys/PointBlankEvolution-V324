using System;

namespace PointBlank.Core.Managers
{
  public class BanHistory
  {
    public long object_id;
    public long provider_id;
    public string type;
    public string value;
    public string reason;
    public DateTime startDate;
    public DateTime endDate;

    public BanHistory()
    {
      this.startDate = DateTime.Now;
      this.type = "";
      this.value = "";
      this.reason = "";
    }
  }
}
