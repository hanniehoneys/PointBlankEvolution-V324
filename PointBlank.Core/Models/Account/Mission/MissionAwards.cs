namespace PointBlank.Core.Models.Account.Mission
{
  public class MissionAwards
  {
    public int _id;
    public int _blueOrder;
    public int _exp;
    public int _gp;

    public MissionAwards(int id, int blueOrder, int exp, int gp)
    {
      this._id = id;
      this._blueOrder = blueOrder;
      this._exp = exp;
      this._gp = gp;
    }
  }
}
