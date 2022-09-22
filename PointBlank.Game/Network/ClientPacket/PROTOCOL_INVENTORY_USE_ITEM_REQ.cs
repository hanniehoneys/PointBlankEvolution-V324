using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Configs;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_INVENTORY_USE_ITEM_REQ : ReceivePacket
  {
    private uint erro = 1;
    private long objId;
    private uint value;
    private byte[] info;
    private string txt;

    public PROTOCOL_INVENTORY_USE_ITEM_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.objId = (long) this.readD();
      this.info = this.readB((int) this.readC());
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        ItemsModel itemsModel = player == null ? (ItemsModel) null : player._inventory.getItem(this.objId);
        if (itemsModel != null && itemsModel._id > 1700000)
        {
          int itemId = ComDiv.CreateItemId(16, 0, ComDiv.getIdStatics(itemsModel._id, 3));
          DateTime dateTime = DateTime.Now;
          dateTime = dateTime.AddDays((double) ComDiv.getIdStatics(itemsModel._id, 2));
          long int64 = Convert.ToInt64(dateTime.ToString("yyMMddHHmm"));
          switch (itemId)
          {
            case 1600005:
            case 1600052:
              this.value = BitConverter.ToUInt32(this.info, 0);
              break;
            case 1600010:
            case 1600047:
            case 1600051:
              this.txt = ComDiv.arrayToString(this.info, this.info.Length);
              break;
            default:
              if (this.info.Length != 0)
              {
                this.value = (uint) this.info[0];
                break;
              }
              break;
          }
          this.CreateCouponEffects(itemId, int64, player);
        }
        else
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEM_AUTH_ACK(this.erro, itemsModel, player));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_INVENTORY_USE_ITEM_REQ: " + ex.ToString());
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEM_AUTH_ACK(2147483648U, (ItemsModel) null, (PointBlank.Game.Data.Model.Account) null));
      }
    }

    private void CreateCouponEffects(int cupomId, long cuponDays, PointBlank.Game.Data.Model.Account p)
    {
      switch (cupomId)
      {
        case 1600005:
          PointBlank.Core.Models.Account.Clan.Clan clan1 = ClanManager.getClan(p.clanId);
          if (clan1._id > 0 && clan1.owner_id == this._client.player_id && ComDiv.updateDB("clan_data", "color", (object) (int) this.value, "clan_id", (object) clan1._id))
          {
            clan1._name_color = (int) this.value;
            this._client.SendPacket((SendPacket) new PROTOCOL_CS_REPLACE_COLOR_NAME_RESULT_ACK((byte) clan1._name_color));
            break;
          }
          this.erro = 2147483648U;
          break;
        case 1600006:
          if (ComDiv.updateDB("accounts", "name_color", (object) (int) this.value, "player_id", (object) p.player_id))
          {
            p.name_color = (int) this.value;
            this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, p, new ItemsModel(cupomId, 3, "Name Color [Active]", 2, cuponDays, 0L)));
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_MYINFO_BASIC_ACK(p));
            if (p._room == null)
              break;
            using (PROTOCOL_ROOM_GET_NICKNAME_ACK roomGetNicknameAck = new PROTOCOL_ROOM_GET_NICKNAME_ACK(p._slotId, p.player_name, p.name_color))
              p._room.SendPacketToPlayers((SendPacket) roomGetNicknameAck);
            using (PROTOCOL_ROOM_GET_COLOR_NICK_ACK roomGetColorNickAck = new PROTOCOL_ROOM_GET_COLOR_NICK_ACK(p._slotId, p.name_color))
            {
              p._room.SendPacketToPlayers((SendPacket) roomGetColorNickAck);
              break;
            }
          }
          else
          {
            this.erro = 2147483648U;
            break;
          }
        case 1600009:
          if ((int) this.value >= 51 || (int) this.value < p._rank - 10 || (int) this.value > p._rank + 10)
          {
            this.erro = 2147483648U;
            break;
          }
          if (ComDiv.updateDB("player_bonus", "fakerank", (object) (int) this.value, "player_id", (object) p.player_id))
          {
            p._bonus.fakeRank = (int) this.value;
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(this.info.Length, p));
            this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, p, new ItemsModel(cupomId, 3, "Fake Rank [Active]", 2, cuponDays, 0L)));
            if (p._room == null)
              break;
            using (PROTOCOL_ROOM_GET_RANK_ACK protocolRoomGetRankAck = new PROTOCOL_ROOM_GET_RANK_ACK(p._slotId, p.getRank()))
              p._room.SendPacketToPlayers((SendPacket) protocolRoomGetRankAck);
            p._room.updateSlotsInfo();
            break;
          }
          this.erro = 2147483648U;
          break;
        case 1600010:
          if (string.IsNullOrEmpty(this.txt) || this.txt.Length < GameConfig.minNickSize || this.txt.Length > GameConfig.maxNickSize)
          {
            this.erro = 2147483648U;
            break;
          }
          if (ComDiv.updateDB("player_bonus", "fakenick", (object) p.player_name, "player_id", (object) p.player_id) && ComDiv.updateDB("accounts", "player_name", (object) this.txt, "player_id", (object) p.player_id))
          {
            p._bonus.fakeNick = p.player_name;
            p.player_name = this.txt;
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(this.info.Length, p));
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(p.player_name));
            this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, p, new ItemsModel(cupomId, 3, "Fake Nick [Active]", 2, cuponDays, 0L)));
            if (p._room == null)
              break;
            p._room.updateSlotsInfo();
            break;
          }
          this.erro = 2147483648U;
          break;
        case 1600014:
          if (ComDiv.updateDB("player_bonus", "sightcolor", (object) (int) this.value, "player_id", (object) p.player_id))
          {
            p._bonus.sightColor = (int) this.value;
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(this.info.Length, p));
            this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, p, new ItemsModel(cupomId, 3, "Sight Color [Active]", 2, cuponDays, 0L)));
            break;
          }
          this.erro = 2147483648U;
          break;
        case 1600047:
          if (string.IsNullOrEmpty(this.txt) || this.txt.Length < GameConfig.minNickSize || (this.txt.Length > GameConfig.maxNickSize || p._inventory.getItem(1600010) != null))
          {
            this.erro = 2147483648U;
            break;
          }
          if (!PlayerManager.isPlayerNameExist(this.txt))
          {
            if (ComDiv.updateDB("accounts", "player_name", (object) this.txt, "player_id", (object) p.player_id))
            {
              NickHistoryManager.CreateHistory(p.player_id, p.player_name, this.txt, "เปลี่ยนชื่อ[ในเกม]");
              p.player_name = this.txt;
              if (p._room != null)
              {
                using (PROTOCOL_ROOM_GET_NICKNAME_ACK roomGetNicknameAck = new PROTOCOL_ROOM_GET_NICKNAME_ACK(p._slotId, p.player_name, p.name_color))
                  p._room.SendPacketToPlayers((SendPacket) roomGetNicknameAck);
              }
              this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(p.player_name));
              if (p.clanId > 0)
              {
                using (PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK memberInfoChangeAck = new PROTOCOL_CS_MEMBER_INFO_CHANGE_ACK(p))
                  ClanManager.SendPacket((SendPacket) memberInfoChangeAck, p.clanId, -1L, true, true);
              }
              AllUtils.syncPlayerToFriends(p, true);
              break;
            }
            this.erro = 2147483648U;
            break;
          }
          this.erro = 2147483923U;
          break;
        case 1600051:
          if (string.IsNullOrEmpty(this.txt) || this.txt.Length > 16)
          {
            this.erro = 2147483648U;
            break;
          }
          PointBlank.Core.Models.Account.Clan.Clan clan2 = ClanManager.getClan(p.clanId);
          if (clan2._id > 0 && clan2.owner_id == this._client.player_id)
          {
            if (!ClanManager.isClanNameExist(this.txt) && ComDiv.updateDB("clan_data", "clan_name", (object) this.txt, "clan_id", (object) p.clanId))
            {
              clan2._name = this.txt;
              using (PROTOCOL_CS_REPLACE_NAME_RESULT_ACK replaceNameResultAck = new PROTOCOL_CS_REPLACE_NAME_RESULT_ACK(this.txt))
              {
                ClanManager.SendPacket((SendPacket) replaceNameResultAck, p.clanId, -1L, true, true);
                break;
              }
            }
            else
            {
              this.erro = 2147483648U;
              break;
            }
          }
          else
          {
            this.erro = 2147483648U;
            break;
          }
        case 1600052:
          PointBlank.Core.Models.Account.Clan.Clan clan3 = ClanManager.getClan(p.clanId);
          if (clan3._id > 0 && clan3.owner_id == this._client.player_id && (!ClanManager.isClanLogoExist(this.value) && PlayerManager.updateClanLogo(p.clanId, this.value)))
          {
            clan3._logo = this.value;
            using (PROTOCOL_CS_REPLACE_MARK_RESULT_ACK replaceMarkResultAck = new PROTOCOL_CS_REPLACE_MARK_RESULT_ACK(this.value))
            {
              ClanManager.SendPacket((SendPacket) replaceMarkResultAck, p.clanId, -1L, true, true);
              break;
            }
          }
          else
          {
            this.erro = 2147483648U;
            break;
          }
        case 1600085:
          if (p._room != null)
          {
            PointBlank.Game.Data.Model.Account playerBySlot = p._room.getPlayerBySlot((int) this.value);
            if (playerBySlot != null)
            {
              this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_GET_USER_ITEM_ACK(playerBySlot));
              break;
            }
            this.erro = 2147483648U;
            break;
          }
          this.erro = 2147483648U;
          break;
        case 1600187:
          if (ComDiv.updateDB("player_bonus", "muzzle", (object) (int) this.value, "player_id", (object) p.player_id))
          {
            p._bonus.muzzle = (int) this.value;
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(this.info.Length, p));
            this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, p, new ItemsModel(cupomId, 3, "Muzzle Color [Active]", 2, cuponDays, 0L)));
            break;
          }
          this.erro = 2147483648U;
          break;
        case 1600193:
          PointBlank.Core.Models.Account.Clan.Clan clan4 = ClanManager.getClan(p.clanId);
          if (clan4._id > 0 && clan4.owner_id == this._client.player_id)
          {
            if (ComDiv.updateDB("clan_data", "effect", (object) (int) this.value, "clan_id", (object) p.clanId))
            {
              clan4.effect = (int) this.value;
              using (PROTOCOL_CS_REPLACE_MARKEFFECT_RESULT_ACK markeffectResultAck = new PROTOCOL_CS_REPLACE_MARKEFFECT_RESULT_ACK((int) this.value))
              {
                ClanManager.SendPacket((SendPacket) markeffectResultAck, p.clanId, -1L, true, true);
                break;
              }
            }
            else
            {
              this.erro = 2147483648U;
              break;
            }
          }
          else
          {
            this.erro = 2147483648U;
            break;
          }
        default:
          Logger.error("Coupon effect not found! Id: " + (object) cupomId);
          this.erro = 2147483648U;
          break;
      }
    }
  }
}
