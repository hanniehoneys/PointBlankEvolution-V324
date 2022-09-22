using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_MESSENGER_NOTE_DELETE_REQ : ReceivePacket
  {
    private List<object> objs = new List<object>();
    private uint erro;

    public PROTOCOL_MESSENGER_NOTE_DELETE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      int num = (int) this.readC();
      for (int index = 0; index < num; ++index)
        this.objs.Add((object) this.readD());
    }

    public override void run()
    {
      if (this._client._player == null)
        return;
      try
      {
        if (!MessageManager.DeleteMessages(this.objs, this._client.player_id))
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_MESSENGER_NOTE_DELETE_ACK(this.erro, this.objs));
        this.objs = (List<object>) null;
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_MESSENGER_NOTE_DELETE_REQ: " + ex.ToString());
      }
    }
  }
}
