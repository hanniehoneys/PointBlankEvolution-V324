using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_NEW_JOIN_ROOM_SCORE_REQ : ReceivePacket
  {
    public PROTOCOL_BATTLE_NEW_JOIN_ROOM_SCORE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      if (this._client == null)
        return;
      if (this._client._player == null)
        return;
      try
      {
        Account player = this._client._player;
        Room room = player._room;
        if (room == null || room._state < RoomState.Loading || room._slots[player._slotId].state != SlotState.NORMAL)
          return;
        this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_NEW_JOIN_ROOM_SCORE_ACK(room));
      }
      catch
      {
      }
    }
  }
}
