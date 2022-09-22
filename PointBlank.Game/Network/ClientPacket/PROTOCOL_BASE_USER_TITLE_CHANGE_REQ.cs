using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Account.Title;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BASE_USER_TITLE_CHANGE_REQ : ReceivePacket
  {
    private int titleIdx;
    private uint erro;

    public PROTOCOL_BASE_USER_TITLE_CHANGE_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.titleIdx = (int) this.readC();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null || this.titleIdx >= 45)
          return;
        if (player._titles.ownerId == 0L)
        {
          TitleManager.getInstance().CreateTitleDB(player.player_id);
          player._titles = new PlayerTitles()
          {
            ownerId = player.player_id
          };
        }
        TitleQ title = TitlesXml.getTitle(this.titleIdx, true);
        if (title != null)
        {
          TitleQ title1;
          TitleQ title2;
          TitlesXml.get2Titles(title._req1, title._req2, out title1, out title2, false);
          if ((title._req1 == 0 || title1 != null) && (title._req2 == 0 || title2 != null) && (player._rank >= title._rank && player.brooch >= title._brooch && (player.medal >= title._medals && player.blue_order >= title._blueOrder)) && (player.insignia >= title._insignia && !player._titles.Contains(title._flag) && (player._titles.Contains(title1._flag) && player._titles.Contains(title2._flag))))
          {
            player.brooch -= title._brooch;
            player.medal -= title._medals;
            player.blue_order -= title._blueOrder;
            player.insignia -= title._insignia;
            long flags = player._titles.Add(title._flag);
            TitleManager.getInstance().updateTitlesFlags(player.player_id, flags);
            List<ItemsModel> awards = TitleAwardsXml.getAwards(this.titleIdx);
            if (awards.Count > 0)
            {
              for (int index = 0; index < awards.Count; ++index)
              {
                ItemsModel itemsModel = awards[index];
                if (itemsModel._id != 0)
                  this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, itemsModel));
              }
            }
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_MEDAL_GET_INFO_ACK(player));
            TitleManager.getInstance().updateRequi(player.player_id, player.medal, player.insignia, player.blue_order, player.brooch);
            if (player._titles.Slots < title._slot)
            {
              player._titles.Slots = title._slot;
              ComDiv.updateDB("player_titles", "titleslots", (object) player._titles.Slots, "owner_id", (object) player.player_id);
            }
          }
          else
            this.erro = 2147487875U;
        }
        else
          this.erro = 2147487875U;
        this._client.SendPacket((SendPacket) new PROTOCOL_BASE_USER_TITLE_CHANGE_ACK(this.erro, player._titles.Slots));
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_BASE_USER_TITLE_CHANGE_REQ: " + ex.ToString());
      }
    }
  }
}
