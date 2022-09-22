using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_OPTION_SAVE_REQ : ReceivePacket
  {
    private DBQuery query = new DBQuery();
    private int type;
    private int value;

    public PROTOCOL_BASE_OPTION_SAVE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      PointBlank.Game.Data.Model.Account player = this._client._player;
      if (player == null)
        return;
      bool flag = player._config != null;
      if (!flag)
      {
        flag = PlayerManager.CreateConfigDB(player.player_id);
        if (flag)
          player._config = new PlayerConfig();
      }
      if (!flag)
        return;
      PlayerConfig config = player._config;
      this.type = (int) this.readC();
      this.value = (int) this.readC();
      int num1 = (int) this.readH();
      if ((this.type & 1) == 1)
      {
        config.blood = (int) this.readH();
        config.sight = (int) this.readC();
        config.hand = (int) this.readC();
        config.config = this.readD();
        config.audio_enable = (int) this.readC();
        this.readB(5);
        config.audio1 = (int) this.readC();
        config.audio2 = (int) this.readC();
        config.fov = (int) this.readH();
        config.sensibilidade = (int) this.readC();
        config.mouse_invertido = (int) this.readC();
        int num2 = (int) this.readC();
        int num3 = (int) this.readC();
        config.msgConvite = (int) this.readC();
        config.chatSussurro = (int) this.readC();
        config.macro = (int) this.readC();
        int num4 = (int) this.readC();
        int num5 = (int) this.readC();
        int num6 = (int) this.readC();
      }
      if ((this.type & 2) == 2)
      {
        this.readB(5);
        byte[] numArray = this.readB(235);
        config.keys = numArray;
      }
      if ((this.type & 4) != 4)
        return;
      config.macro_1 = this.readUnicode((int) this.readC() * 2);
      config.macro_2 = this.readUnicode((int) this.readC() * 2);
      config.macro_3 = this.readUnicode((int) this.readC() * 2);
      config.macro_4 = this.readUnicode((int) this.readC() * 2);
      config.macro_5 = this.readUnicode((int) this.readC() * 2);
    }

    public override void run()
    {
      PointBlank.Game.Data.Model.Account player = this._client._player;
      if (player == null)
        return;
      PlayerConfig config = player._config;
      if (config == null)
        return;
      if ((this.type & 1) == 1)
        PlayerManager.updateConfigs(this.query, config);
      if ((this.type & 2) == 2)
        this.query.AddQuery("keys", (object) config.keys);
      if ((this.type & 4) == 4)
        PlayerManager.updateMacros(this.query, config, this.value);
      ComDiv.updateDB("player_configs", "owner_id", (object) this._client.player_id, this.query.GetTables(), this.query.GetValues());
    }
  }
}
