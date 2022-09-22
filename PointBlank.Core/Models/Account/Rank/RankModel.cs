namespace PointBlank.Core.Models.Account.Rank
{
  public class RankModel
  {
    public int _onNextLevel;
    public int _id;
    public int _onGPUp;
    public int _onAllExp;

    public RankModel(int rank, int onNextLevel, int onGPUp, int onAllExp)
    {
      this._id = rank;
      this._onNextLevel = onNextLevel;
      this._onGPUp = onGPUp;
      this._onAllExp = onAllExp;
    }
  }
}
