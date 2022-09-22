using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_BATTLE_MISSION_ROUND_PRE_START_ACK : SendPacket
  {
    private Room _r;
    private List<int> _dinos;
    private bool isBotMode;

    public PROTOCOL_BATTLE_MISSION_ROUND_PRE_START_ACK(Room r, List<int> dinos, bool isBotMode)
    {
      this._r = r;
      this._dinos = dinos;
      this.isBotMode = isBotMode;
    }

    public PROTOCOL_BATTLE_MISSION_ROUND_PRE_START_ACK(Room r)
    {
      this._r = r;
      this._dinos = AllUtils.getDinossaurs(r, false, -1);
      this.isBotMode = this._r.isBotMode();
    }

    public override void write()
    {
      this.writeH((short) 4127);
      this.writeH(AllUtils.getSlotsFlag(this._r, false, false));
      if (this.isBotMode)
        this.writeB(new byte[10]
        {
          byte.MaxValue,
          byte.MaxValue,
          byte.MaxValue,
          byte.MaxValue,
          byte.MaxValue,
          byte.MaxValue,
          byte.MaxValue,
          byte.MaxValue,
          byte.MaxValue,
          byte.MaxValue
        });
      else if (this._r.room_type == RoomType.Boss || this._r.room_type == RoomType.CrossCounter)
      {
        int num1 = this._dinos.Count == 1 || this._r.room_type == RoomType.CrossCounter ? (int) byte.MaxValue : this._r.TRex;
        this.writeC((byte) num1);
        this.writeC((byte) 10);
        for (int index = 0; index < this._dinos.Count; ++index)
        {
          int dino = this._dinos[index];
          if (dino != this._r.TRex && this._r.room_type == RoomType.Boss || this._r.room_type == RoomType.CrossCounter)
            this.writeC((byte) dino);
        }
        int num2 = 8 - this._dinos.Count - (num1 == (int) byte.MaxValue ? 1 : 0);
        for (int index = 0; index < num2; ++index)
          this.writeC(byte.MaxValue);
        this.writeC(byte.MaxValue);
      }
      else
        this.writeB(new byte[10]);
      this.writeC((byte) 0);
    }
  }
}
