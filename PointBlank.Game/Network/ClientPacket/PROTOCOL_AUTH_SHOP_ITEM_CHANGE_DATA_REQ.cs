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
  public class PROTOCOL_AUTH_SHOP_ITEM_CHANGE_DATA_REQ : ReceivePacket
  {
    private long ObjId;
    private uint Value;
    private uint Error;
    private byte[] Info;
    private string Text;

    public PROTOCOL_AUTH_SHOP_ITEM_CHANGE_DATA_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.ObjId = (long) this.readD();
      this.Info = this.readB((int) this.readC());
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        ItemsModel itemsModel = player == null ? (ItemsModel) null : player._inventory.getItem(this.ObjId);
        if (itemsModel != null && itemsModel._id > 1600000)
        {
          int itemId = ComDiv.CreateItemId(16, 0, ComDiv.getIdStatics(itemsModel._id, 3));
          switch (itemId)
          {
            case 1600005:
            case 1610052:
              this.Value = BitConverter.ToUInt32(this.Info, 0);
              break;
            case 1600010:
            case 1610047:
            case 1610051:
              this.Text = ComDiv.arrayToString(this.Info, this.Info.Length);
              break;
            default:
              if (this.Info.Length != 0)
              {
                this.Value = (uint) this.Info[0];
                break;
              }
              break;
          }
          this.CreateCouponEffects(itemId, player);
        }
        else
          this.Error = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEM_CHANGE_DATA_ACK(this.Error));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_AUTH_SHOP_ITEM_CHANGE_DATA_REQ: " + ex.ToString());
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEM_CHANGE_DATA_ACK(2147483648U));
      }
    }

    private void CreateCouponEffects(int cupomId, PointBlank.Game.Data.Model.Account p)
    {
      switch (cupomId)
      {
        case 1600005:
          PointBlank.Core.Models.Account.Clan.Clan clan1 = ClanManager.getClan(p.clanId);
          if (clan1._id > 0 && clan1.owner_id == this._client.player_id && ComDiv.updateDB("clan_data", "color", (object) (int) this.Value, "clan_id", (object) clan1._id))
          {
            clan1._name_color = (int) this.Value;
            this._client.SendPacket((SendPacket) new PROTOCOL_CS_REPLACE_COLOR_NAME_RESULT_ACK((byte) clan1._name_color));
            break;
          }
          this.Error = 2147483648U;
          break;
        case 1600006:
          if (ComDiv.updateDB("accounts", "name_color", (object) (int) this.Value, "player_id", (object) p.player_id))
          {
            p.name_color = (int) this.Value;
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_MYINFO_BASIC_ACK(p));
            if (p._room == null)
              break;
            using (PROTOCOL_ROOM_GET_NICKNAME_ACK roomGetNicknameAck = new PROTOCOL_ROOM_GET_NICKNAME_ACK(p._slotId, p.player_name, p.name_color))
            {
              p._room.SendPacketToPlayers((SendPacket) roomGetNicknameAck);
              break;
            }
          }
          else
          {
            this.Error = 2147483648U;
            break;
          }
        case 1600009:
          if ((int) this.Value >= 51 || (int) this.Value < p._rank - 10 || (int) this.Value > p._rank + 10)
          {
            this.Error = 2147483648U;
            break;
          }
          if (ComDiv.updateDB("player_bonus", "fakerank", (object) (int) this.Value, "player_id", (object) p.player_id))
          {
            p._bonus.fakeRank = (int) this.Value;
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(this.Info.Length, p));
            if (p._room == null)
              break;
            p._room.updateSlotsInfo();
            break;
          }
          this.Error = 2147483648U;
          break;
        case 1600010:
          if (string.IsNullOrEmpty(this.Text) || this.Text.Length < GameConfig.minNickSize || this.Text.Length > GameConfig.maxNickSize)
          {
            this.Error = 2147483648U;
            break;
          }
          if (ComDiv.updateDB("player_bonus", "fakenick", (object) p.player_name, "player_id", (object) p.player_id) && ComDiv.updateDB("accounts", "player_name", (object) this.Text, "player_id", (object) p.player_id))
          {
            p._bonus.fakeNick = p.player_name;
            p.player_name = this.Text;
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(this.Info.Length, p));
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(p.player_name));
            if (p._room == null)
              break;
            p._room.updateSlotsInfo();
            break;
          }
          this.Error = 2147483648U;
          break;
        case 1600014:
          if (ComDiv.updateDB("player_bonus", "sightcolor", (object) (int) this.Value, "player_id", (object) p.player_id))
          {
            p._bonus.sightColor = (int) this.Value;
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(this.Info.Length, p));
            break;
          }
          this.Error = 2147483648U;
          break;
        case 1600187:
          if (ComDiv.updateDB("player_bonus", "muzzle", (object) (int) this.Value, "player_id", (object) p.player_id))
          {
            p._bonus.muzzle = (int) this.Value;
            this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(this.Info.Length, p));
            break;
          }
          this.Error = 2147483648U;
          break;
        case 1610051:
          if (string.IsNullOrEmpty(this.Text) || this.Text.Length > 16)
          {
            this.Error = 2147483648U;
            break;
          }
          PointBlank.Core.Models.Account.Clan.Clan clan2 = ClanManager.getClan(p.clanId);
          if (clan2._id > 0 && clan2.owner_id == this._client.player_id)
          {
            if (!ClanManager.isClanNameExist(this.Text) && ComDiv.updateDB("clan_data", "clan_name", (object) this.Text, "clan_id", (object) p.clanId))
            {
              clan2._name = this.Text;
              using (PROTOCOL_CS_REPLACE_NAME_RESULT_ACK replaceNameResultAck = new PROTOCOL_CS_REPLACE_NAME_RESULT_ACK(this.Text))
              {
                ClanManager.SendPacket((SendPacket) replaceNameResultAck, p.clanId, -1L, true, true);
                break;
              }
            }
            else
            {
              this.Error = 2147483648U;
              break;
            }
          }
          else
          {
            this.Error = 2147483648U;
            break;
          }
        case 1610052:
          PointBlank.Core.Models.Account.Clan.Clan clan3 = ClanManager.getClan(p.clanId);
          if (clan3._id > 0 && clan3.owner_id == this._client.player_id && (!ClanManager.isClanLogoExist(this.Value) && PlayerManager.updateClanLogo(p.clanId, this.Value)))
          {
            clan3._logo = this.Value;
            using (PROTOCOL_CS_REPLACE_MARK_RESULT_ACK replaceMarkResultAck = new PROTOCOL_CS_REPLACE_MARK_RESULT_ACK(this.Value))
            {
              ClanManager.SendPacket((SendPacket) replaceMarkResultAck, p.clanId, -1L, true, true);
              break;
            }
          }
          else
          {
            this.Error = 2147483648U;
            break;
          }
        case 1610085:
          if (p._room != null)
          {
            PointBlank.Game.Data.Model.Account playerBySlot = p._room.getPlayerBySlot((int) this.Value);
            if (playerBySlot != null)
            {
              this._client.SendPacket((SendPacket) new PROTOCOL_ROOM_GET_USER_ITEM_ACK(playerBySlot));
              break;
            }
            this.Error = 2147483648U;
            break;
          }
          this.Error = 2147483648U;
          break;
        case 1800047:
          if (string.IsNullOrEmpty(this.Text) || this.Text.Length < GameConfig.minNickSize || (this.Text.Length > GameConfig.maxNickSize || p._inventory.getItem(1600010) != null))
          {
            this.Error = 2147483648U;
            break;
          }
          if (!PlayerManager.isPlayerNameExist(this.Text))
          {
            if (ComDiv.updateDB("accounts", "player_name", (object) this.Text, "player_id", (object) p.player_id))
            {
              NickHistoryManager.CreateHistory(p.player_id, p.player_name, this.Text, "เปลี่ยนชื่อ[ในเกม]");
              p.player_name = this.Text;
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
            this.Error = 2147483648U;
            break;
          }
          this.Error = 2147483923U;
          break;
        default:
          Logger.error("Coupon effect not found! Id: " + (object) cupomId);
          this.Error = 2147483648U;
          break;
      }
    }
  }
}
