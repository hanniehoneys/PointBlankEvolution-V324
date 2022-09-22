using System.Collections.Generic;

namespace PointBlank.Core.Models.Room
{
  public class VoteKick
  {
    public int kikar = 1;
    public int deixar = 1;
    public List<int> _votes = new List<int>();
    public bool[] TotalArray = new bool[16];
    public int creatorIdx;
    public int victimIdx;
    public int motive;
    public int allies;
    public int enemys;

    public VoteKick(int creator, int victim)
    {
      this.creatorIdx = creator;
      this.victimIdx = victim;
      this._votes.Add(creator);
      this._votes.Add(victim);
    }

    public int GetInGamePlayers()
    {
      int num = 0;
      for (int index = 0; index < 16; ++index)
      {
        if (this.TotalArray[index])
          ++num;
      }
      return num;
    }
  }
}
