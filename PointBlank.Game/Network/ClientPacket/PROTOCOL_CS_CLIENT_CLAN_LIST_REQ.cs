using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CS_CLIENT_CLAN_LIST_REQ : ReceivePacket
  {
    private uint page;

    public PROTOCOL_CS_CLIENT_CLAN_LIST_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.page = this.readUD();
    }

    public override void run()
    {
      try
      {
        if (this._client == null || this._client._player == null)
          return;
        int count = 0;
        using (SendGPacket p = new SendGPacket())
        {
          lock (ClanManager._clans)
          {
            for (int index = (int) this.page * 15; index < ClanManager._clans.Count; ++index)
            {
              this.WriteData(ClanManager._clans[index], p);
              if (++count == 15)
                break;
            }
          }
          this._client.SendPacket((SendPacket) new PROTOCOL_CS_CLIENT_CLAN_LIST_ACK(this.page, count, p.mstream.ToArray()));
        }
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
    }

    private void WriteData(PointBlank.Core.Models.Account.Clan.Clan clan, SendGPacket p)
    {
      p.writeD(clan._id);
      p.writeUnicode(clan._name, 34);
      p.writeC((byte) clan._rank);
      p.writeC((byte) PlayerManager.getClanPlayers(clan._id));
      p.writeC((byte) clan.maxPlayers);
      p.writeD(clan.creationDate);
      p.writeD(clan._logo);
      p.writeC((byte) clan._name_color);
      p.writeC((byte) clan.effect);
      p.writeD(clan.partidas);
      p.writeD(clan.vitorias);
      p.writeD(clan.derrotas);
      p.writeD(0);
      p.writeF((double) clan._pontos);
      p.writeF((double) clan._pontos);
      p.writeC((byte) 0);
      p.writeH((short) clan._name.Length);
      p.writeUnicode(clan._name, clan._name.Length * 2);
    }
  }
}
