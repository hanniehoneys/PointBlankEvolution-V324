using PointBlank.Core.Network;

namespace PointBlank.Core.Models.Account.Players
{
  public class PlayerInfo
  {
    public int _rank;
    public int _name_color;
    public long player_id;
    public string player_name;
    public bool _isOnline;
    public AccountStatus _status;

    public PlayerInfo(long player_id)
    {
      this.player_id = player_id;
      this._status = new AccountStatus();
    }

    public PlayerInfo(
      long player_id,
      int rank,
      int name_color,
      string name,
      bool isOnline,
      AccountStatus status)
    {
      this.player_id = player_id;
      this.SetInfo(rank, name_color, name, isOnline, status);
    }

    public void setOnlineStatus(bool state)
    {
      if (this._isOnline == state || !ComDiv.updateDB("accounts", "online", (object) state, "player_id", (object) this.player_id))
        return;
      this._isOnline = state;
    }

    public void SetInfo(
      int rank,
      int name_color,
      string name,
      bool isOnline,
      AccountStatus status)
    {
      this._rank = rank;
      this._name_color = name_color;
      this.player_name = name;
      this._isOnline = isOnline;
      this._status = status;
    }
  }
}
