using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_START_KICKVOTE_REQ : ReceivePacket
  {
    private int motive;
    private int slotIdx;
    private uint erro;

    public PROTOCOL_BATTLE_START_KICKVOTE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.slotIdx = (int) this.readC();
      this.motive = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room == null || room._state != RoomState.Battle || player._slotId == this.slotIdx)
          return;
        Slot slot = room.getSlot(player._slotId);
        if (slot == null || slot.state != SlotState.BATTLE || room._slots[this.slotIdx].state != SlotState.BATTLE)
          return;
        int RedPlayers;
        int BluePlayers;
        room.getPlayingPlayers(true, out RedPlayers, out BluePlayers);
        if (player._rank < GameConfig.minRankVote && !player.HaveGMLevel())
          this.erro = 2147487972U;
        else if (room.vote.Timer != null)
          this.erro = 2147487968U;
        else if (slot.NextVoteDate > DateTime.Now)
          this.erro = 2147487969U;
        this._client.SendPacket((SendPacket) new PROTOCOL_BATTLE_SUGGEST_KICKVOTE_ACK(this.erro));
        if (this.erro > 0U)
          return;
        slot.NextVoteDate = DateTime.Now.AddMinutes(1.0);
        room.votekick = new PointBlank.Core.Models.Room.VoteKick(slot._id, this.slotIdx)
        {
          motive = this.motive
        };
        this.ChargeVoteKickArray(room);
        using (PROTOCOL_BATTLE_START_KICKVOTE_ACK startKickvoteAck = new PROTOCOL_BATTLE_START_KICKVOTE_ACK(room.votekick))
          room.SendPacketToPlayers((SendPacket) startKickvoteAck, SlotState.BATTLE, 0, player._slotId, this.slotIdx);
        room.StartVote();
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BATTLE_START_KICKVOTE_REQ: " + ex.ToString());
      }
    }

    private void ChargeVoteKickArray(PointBlank.Game.Data.Model.Room room)
    {
      for (int index = 0; index < 16; ++index)
      {
        Slot slot = room._slots[index];
        room.votekick.TotalArray[index] = slot.state == SlotState.BATTLE;
      }
    }
  }
}
