using System;

namespace PointBlank.Core.Models.Account.Players
{
  public class PlayerStats
  {
    public int fights;
    public int fights_win;
    public int fights_lost;
    public int fights_draw;
    public int kills_count;
    public int totalkills_count;
    public int totalfights_count;
    public int deaths_count;
    public int escapes;
    public int headshots_count;
    public int assist;
    public int ClanGames;
    public int ClanWins;
    public int ClanLoses;

    public int GetKDRatio()
    {
      if (this.headshots_count <= 0 && this.kills_count <= 0)
        return 0;
      return (int) Math.Floor(((double) (this.kills_count * 100) + 0.5) / (double) (this.kills_count + this.deaths_count));
    }

    public int GetHSRatio()
    {
      if (this.kills_count <= 0)
        return 0;
      return (int) Math.Floor((double) (this.headshots_count * 100) / (double) this.kills_count + 0.5);
    }
  }
}
