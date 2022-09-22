using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Title;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_USER_TITLE_RELEASE_REQ : ReceivePacket
  {
    private int slotIdx;
    private uint erro;

    public PROTOCOL_BASE_USER_TITLE_RELEASE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.slotIdx = (int) this.readC();
      int num = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null || this.slotIdx >= 3 || player._titles == null)
          return;
        PlayerTitles titles = player._titles;
        int equip = titles.GetEquip(this.slotIdx);
        if (equip > 0 && TitleManager.getInstance().updateEquipedTitle(titles.ownerId, this.slotIdx, 0))
        {
          titles.SetEquip(this.slotIdx, 0);
          if (TitleAwardsXml.Contains(equip, player._equip._beret) && ComDiv.updateDB("accounts", "char_beret", (object) 0, "player_id", (object) player.player_id))
          {
            player._equip._beret = 0;
            Room room = player._room;
            if (room != null)
              AllUtils.updateSlotEquips(player, room);
          }
        }
        else
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_USER_TITLE_RELEASE_ACK(this.erro));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BASE_USER_TITLE_RELEASE_REQ: " + ex.ToString());
      }
    }
  }
}
