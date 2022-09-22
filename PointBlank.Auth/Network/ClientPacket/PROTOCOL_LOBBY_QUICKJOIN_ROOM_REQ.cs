using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Models.Servers;
using PointBlank.Core.Network;
using System;
using System.Collections.Generic;

namespace PointBlank.Auth.Network.ClientPacket
{
  public class PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ : ReceivePacket
  {
    private List<QuickStart> Quicks = new List<QuickStart>();
    private int Select;

    public PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ(AuthClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.Select = (int) this.readC();
      for (int index = 0; index < 3; ++index)
        this.Quicks.Add(new QuickStart()
        {
          MapId = (int) this.readC(),
          Rule = (int) this.readC(),
          StageOptions = (int) this.readC(),
          Type = (int) this.readC()
        });
    }

    public override void run()
    {
      try
      {
        this._client.SendPacket((SendPacket) new PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK("ระบบนี้ยังไม่เปิดใช้งาน"));
      }
      catch (Exception ex)
      {
        Logger.error("PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ: " + ex.ToString());
      }
    }
  }
}
