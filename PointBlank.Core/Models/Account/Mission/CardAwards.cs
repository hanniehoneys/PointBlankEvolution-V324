namespace PointBlank.Core.Models.Account.Mission
{
  public class CardAwards
  {
    public int _id;
    public int _card;
    public int _insignia;
    public int _medal;
    public int _brooch;
    public int _exp;
    public int _gp;

    public bool Unusable()
    {
      return this._insignia == 0 && this._medal == 0 && (this._brooch == 0 && this._exp == 0) && this._gp == 0;
    }
  }
}
