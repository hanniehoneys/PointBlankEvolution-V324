namespace PointBlank.Core.Models.Account.Clan
{
  public class RecordInfo
  {
    public long PlayerId;
    public int RecordValue;

    public RecordInfo(string[] split)
    {
      this.PlayerId = this.GetPlayerId(split);
      this.RecordValue = this.GetPlayerValue(split);
    }

    public long GetPlayerId(string[] split)
    {
      try
      {
        return long.Parse(split[0]);
      }
      catch
      {
        return 0;
      }
    }

    public int GetPlayerValue(string[] split)
    {
      try
      {
        return int.Parse(split[1]);
      }
      catch
      {
        return 0;
      }
    }

    public string GetSplit()
    {
      return this.PlayerId.ToString() + "-" + (object) this.RecordValue;
    }
  }
}
