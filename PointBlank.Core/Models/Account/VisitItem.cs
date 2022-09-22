namespace PointBlank.Core.Models.Account
{
  public class VisitItem
  {
    public int good_id;
    public long count;
    public bool IsReward;

    public void SetCount(string text)
    {
      this.count = long.Parse(text);
      if (this.count <= 0L)
        return;
      this.IsReward = true;
    }
  }
}
