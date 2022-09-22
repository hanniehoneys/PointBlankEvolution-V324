using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_DEATH_ACK : SendPacket
  {
    private PointBlank.Game.Data.Model.Room room;
    private FragInfos kills;
    private Slot killer;
    private bool isBotMode;

    public PROTOCOL_BATTLE_DEATH_ACK(PointBlank.Game.Data.Model.Room r, FragInfos kills, Slot killer, bool isBotMode)
    {
      this.room = r;
      this.kills = kills;
      this.killer = killer;
      this.isBotMode = isBotMode;
    }

    public override void write()
    {
      this.writeH((short) 4112);
      this.writeC((byte) this.kills.killingType);
      this.writeC(this.kills.killsCount);
      this.writeC(this.kills.killerIdx);
      this.writeD(this.kills.weapon);
      this.writeT(this.kills.x);
      this.writeT(this.kills.y);
      this.writeT(this.kills.z);
      this.writeC(this.kills.flag);
      for (int index = 0; index < this.kills.frags.Count; ++index)
      {
        Frag frag = this.kills.frags[index];
        this.writeC(frag.victimWeaponClass);
        this.writeC(frag.hitspotInfo);
        this.writeH((short) frag.killFlag);
        this.writeC(frag.flag);
        this.writeT(frag.x);
        this.writeT(frag.y);
        this.writeT(frag.z);
        this.writeC((byte) frag.AssistSlot);
      }
      this.writeH((short) this.room._redKills);
      this.writeH((short) this.room._redDeaths);
      this.writeH((short) this.room._redAssists);
      this.writeH((short) this.room._blueKills);
      this.writeH((short) this.room._blueDeaths);
      this.writeH((short) this.room._blueAssists);
      for (int index = 0; index < 16; ++index)
      {
        Slot slot = this.room._slots[index];
        this.writeH((short) slot.allKills);
        this.writeH((short) slot.allDeaths);
        this.writeH((short) slot.allAssists);
      }
      if (this.killer.killsOnLife == 2)
        this.writeC((byte) 1);
      else if (this.killer.killsOnLife == 3)
        this.writeC((byte) 2);
      else if (this.killer.killsOnLife > 3)
        this.writeC((byte) 3);
      else
        this.writeC((byte) 0);
      this.writeH((ushort) this.kills.Score);
      this.writeH((ushort) this.room.red_dino);
      this.writeH((ushort) this.room.blue_dino);
    }
  }
}
