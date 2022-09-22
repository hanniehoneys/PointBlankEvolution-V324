using PointBlank.Core.Managers;

namespace PointBlank.Core.Models.Account.Clan
{
  public class Clan
  {
    public int maxPlayers = 50;
    public string _name = "";
    public string _info = "";
    public string _news = "";
    public uint _logo = uint.MaxValue;
    public float _pontos = 1000f;
    public ClanBestPlayers BestPlayers = new ClanBestPlayers();
    public int _id;
    public int creationDate;
    public int partidas;
    public int vitorias;
    public int derrotas;
    public int autoridade;
    public int limite_rank;
    public int limite_idade;
    public int limite_idade2;
    public int _exp;
    public int _rank;
    public int _name_color;
    public int effect;
    public long owner_id;

    public int getClanUnit()
    {
      return this.getClanUnit(PlayerManager.getClanPlayers(this._id));
    }

    public int getClanUnit(int count)
    {
      if (count >= 250)
        return 7;
      if (count >= 200)
        return 6;
      if (count >= 150)
        return 5;
      if (count >= 100)
        return 4;
      if (count >= 50)
        return 3;
      if (count >= 30)
        return 2;
      return count >= 10 ? 1 : 0;
    }
  }
}
