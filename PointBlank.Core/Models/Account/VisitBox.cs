namespace PointBlank.Core.Models.Account
{
  public class VisitBox
  {
    public VisitItem reward1;
    public VisitItem reward2;
    public int RewardCount;

    public VisitBox()
    {
      this.reward1 = new VisitItem();
      this.reward2 = new VisitItem();
    }

    public void SetCount()
    {
      if (this.reward1 != null && this.reward1.good_id > 0)
        ++this.RewardCount;
      if (this.reward2 == null || this.reward2.good_id <= 0)
        return;
      ++this.RewardCount;
    }
  }
}
