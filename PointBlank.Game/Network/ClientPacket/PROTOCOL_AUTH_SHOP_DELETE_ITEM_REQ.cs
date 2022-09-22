using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_SHOP_DELETE_ITEM_REQ : ReceivePacket
  {
    private uint erro = 1;
    private long objId;

    public PROTOCOL_AUTH_SHOP_DELETE_ITEM_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.objId = (long) this.readD();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        ItemsModel itemsModel = player._inventory.getItem(this.objId);
        PlayerBonus bonus = player._bonus;
        if (itemsModel == null)
          this.erro = 2147483648U;
        else if (ComDiv.getIdStatics(itemsModel._id, 1) == 16)
        {
          if (bonus == null)
          {
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_DELETE_ITEM_ACK(2147483648U, 0L));
            return;
          }
          if (!bonus.RemoveBonuses(itemsModel._id))
          {
            if (itemsModel._id == 1600014)
            {
              if (ComDiv.updateDB("player_bonus", "sightcolor", (object) 4, "player_id", (object) player.player_id))
              {
                bonus.sightColor = 4;
                this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(0, player));
              }
              else
                this.erro = 2147483648U;
            }
            else if (itemsModel._id == 1600010)
            {
              if (bonus.fakeNick.Length == 0)
                this.erro = 2147483648U;
              else if (ComDiv.updateDB("accounts", "player_name", (object) bonus.fakeNick, "player_id", (object) player.player_id) && ComDiv.updateDB("player_bonus", "fakenick", (object) "", "player_id", (object) player.player_id))
              {
                player.player_name = bonus.fakeNick;
                bonus.fakeNick = "";
                this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(0, player));
                this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(player.player_name));
              }
              else
                this.erro = 2147483648U;
            }
            else if (itemsModel._id == 1600009)
            {
              if (ComDiv.updateDB("player_bonus", "fakerank", (object) 55, "player_id", (object) player.player_id))
              {
                bonus.fakeRank = 55;
                this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(0, player));
              }
              else
                this.erro = 2147483648U;
            }
            else if (itemsModel._id == 1600187)
            {
              if (ComDiv.updateDB("player_bonus", "muzzle", (object) 0, "player_id", (object) player.player_id))
              {
                bonus.muzzle = 0;
                this._client.SendPacket((SendPacket) new PROTOCOL_BASE_INV_ITEM_DATA_ACK(0, player));
              }
              else
                this.erro = 2147483648U;
            }
            else if (itemsModel._id == 1600006)
            {
              if (ComDiv.updateDB("accounts", "name_color", (object) 0, "player_id", (object) player.player_id))
              {
                player.name_color = 0;
                this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_MYINFO_BASIC_ACK(player));
                Room room = player._room;
                if (room != null)
                {
                  using (PROTOCOL_ROOM_GET_NICKNAME_ACK roomGetNicknameAck = new PROTOCOL_ROOM_GET_NICKNAME_ACK(player._slotId, player.player_name, player.name_color))
                    room.SendPacketToPlayers((SendPacket) roomGetNicknameAck);
                }
              }
              else
                this.erro = 2147483648U;
            }
          }
          else
            PlayerManager.updatePlayerBonus(player.player_id, bonus.bonuses, bonus.freepass);
          CouponFlag couponEffect = CouponEffectManager.getCouponEffect(itemsModel._id);
          if (couponEffect != null && couponEffect.EffectFlag > (CouponEffects) 0 && player.effects.HasFlag((Enum) couponEffect.EffectFlag))
          {
            player.effects -= couponEffect.EffectFlag;
            PlayerManager.updateCupomEffects(player.player_id, player.effects);
          }
        }
        if (this.erro == 1U && itemsModel != null)
        {
          if (PlayerManager.DeleteItem(itemsModel._objId, player.player_id))
            player._inventory.RemoveItem(itemsModel);
          else
            this.erro = 2147483648U;
        }
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_DELETE_ITEM_ACK(this.erro, this.objId));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_AUTH_SHOP_DELETE_ITEM_ACK: " + ex.ToString());
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_DELETE_ITEM_ACK(2147483648U, 0L));
      }
    }
  }
}
