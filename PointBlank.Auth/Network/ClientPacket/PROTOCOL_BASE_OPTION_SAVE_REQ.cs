using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;

namespace PointBlank.Auth.Network.ClientPacket
{
  public class PROTOCOL_BASE_OPTION_SAVE_REQ : ReceivePacket
  {
    private DBQuery query = new DBQuery();
    private int type;
    private int value;

    public PROTOCOL_BASE_OPTION_SAVE_REQ(AuthClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      PointBlank.Auth.Data.Model.Account player = this._client._player;
      if (player == null || player._config == null)
        return;
      this.type = (int) this.readC();
      this.value = (int) this.readC();
      int num1 = (int) this.readH();
      if ((this.type & 1) == 1)
      {
        player._config.blood = (int) this.readH();
        player._config.sight = (int) this.readC();
        player._config.hand = (int) this.readC();
        player._config.config = this.readD();
        player._config.audio_enable = (int) this.readC();
        this.readB(5);
        player._config.audio1 = (int) this.readC();
        player._config.audio2 = (int) this.readC();
        player._config.fov = (int) this.readH();
        player._config.sensibilidade = (int) this.readC();
        player._config.mouse_invertido = (int) this.readC();
        int num2 = (int) this.readC();
        int num3 = (int) this.readC();
        player._config.msgConvite = (int) this.readC();
        player._config.chatSussurro = (int) this.readC();
        player._config.macro = (int) this.readC();
        int num4 = (int) this.readC();
        int num5 = (int) this.readC();
        int num6 = (int) this.readC();
      }
      if ((this.type & 2) == 2)
      {
        this.readB(5);
        byte[] numArray = this.readB(235);
        player._config.keys = numArray;
      }
      if ((this.type & 4) != 4)
        return;
      player._config.macro_1 = this.readUnicode((int) this.readC() * 2);
      player._config.macro_2 = this.readUnicode((int) this.readC() * 2);
      player._config.macro_3 = this.readUnicode((int) this.readC() * 2);
      player._config.macro_4 = this.readUnicode((int) this.readC() * 2);
      player._config.macro_5 = this.readUnicode((int) this.readC() * 2);
    }

    public override void run()
    {
      PointBlank.Auth.Data.Model.Account player = this._client._player;
      if (player == null || player._config == null)
        return;
      PlayerConfig config = player._config;
      if ((this.type & 1) == 1)
        PlayerManager.updateConfigs(this.query, config);
      if ((this.type & 2) == 2)
        this.query.AddQuery("keys", (object) config.keys);
      if ((this.type & 4) == 4)
        PlayerManager.updateMacros(this.query, config, this.value);
      ComDiv.updateDB("player_configs", "owner_id", (object) player.player_id, this.query.GetTables(), this.query.GetValues());
    }
  }
}
