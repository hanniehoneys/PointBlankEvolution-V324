namespace PointBlank.Core.Models.Account.Title
{
  public class TitleQ
  {
    public int _id;
    public int _classId;
    public int _medals;
    public int _brooch;
    public int _blueOrder;
    public int _insignia;
    public int _rank;
    public int _slot;
    public int _req1;
    public int _req2;
    public long _flag;

    public TitleQ()
    {
    }

    public TitleQ(int titleId)
    {
      this._id = titleId;
      this._flag = 1L << titleId;
    }
  }
}
