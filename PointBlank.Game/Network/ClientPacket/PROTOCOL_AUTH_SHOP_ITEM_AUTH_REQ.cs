using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Randombox;
using PointBlank.Core.Network;
using PointBlank.Core.Xml;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_AUTH_SHOP_ITEM_AUTH_REQ : ReceivePacket
  {
    private static readonly Random getrandom = new Random();
    private static readonly object syncLock = new object();
    private uint erro = 1;
    private long objId;
    private int itemId;
    private long oldCOUNT;

    public PROTOCOL_AUTH_SHOP_ITEM_AUTH_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      this.objId = (long) this.readD();
    }

    public override void run()
    {
      if (this._client == null)
        return;
      if (this._client._player == null)
        return;
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        ItemsModel itemsModel1 = player._inventory.getItem(this.objId);
        if (itemsModel1 != null)
        {
          this.itemId = itemsModel1._id;
          this.oldCOUNT = itemsModel1._count;
          if (itemsModel1._category == 3 && player._inventory._items.Count >= 500)
          {
            this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEM_AUTH_ACK(2147487785U, (ItemsModel) null, (PointBlank.Game.Data.Model.Account) null));
            return;
          }
          if (this.itemId == 1800049)
          {
            if (PlayerManager.updateKD(player.player_id, 0, 0, player._statistic.headshots_count, player._statistic.totalkills_count))
            {
              player._statistic.kills_count = 0;
              player._statistic.deaths_count = 0;
              this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_MYINFO_RECORD_ACK(player._statistic));
            }
            else
              this.erro = 2147483648U;
          }
          else if (this.itemId == 1800048)
          {
            if (PlayerManager.updateFights(0, 0, 0, 0, player._statistic.totalfights_count, player.player_id))
            {
              player._statistic.fights = 0;
              player._statistic.fights_win = 0;
              player._statistic.fights_lost = 0;
              player._statistic.fights_draw = 0;
              this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_MYINFO_RECORD_ACK(player._statistic));
            }
            else
              this.erro = 2147483648U;
          }
          else if (this.itemId == 1800050)
          {
            if (ComDiv.updateDB("accounts", "escapes", (object) 0, "player_id", (object) player.player_id))
            {
              player._statistic.escapes = 0;
              this._client.SendPacket((SendPacket) new PROTOCOL_BASE_GET_MYINFO_RECORD_ACK(player._statistic));
            }
            else
              this.erro = 2147483648U;
          }
          else if (this.itemId == 1800053)
          {
            if (PlayerManager.updateClanBattles(player.clanId, 0, 0, 0))
            {
              PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
              if (clan._id > 0 && clan.owner_id == this._client.player_id)
              {
                clan.partidas = 0;
                clan.vitorias = 0;
                clan.derrotas = 0;
                this._client.SendPacket((SendPacket) new PROTOCOL_CS_RECORD_RESET_RESULT_ACK());
              }
              else
                this.erro = 2147483648U;
            }
            else
              this.erro = 2147483648U;
          }
          else if (this.itemId == 1800055)
          {
            PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
            if (clan._id > 0 && clan.owner_id == this._client.player_id)
            {
              if (clan.maxPlayers + 50 <= 250 && ComDiv.updateDB("clan_data", "max_players", (object) (clan.maxPlayers + 50), "clan_id", (object) player.clanId))
              {
                clan.maxPlayers += 50;
                this._client.SendPacket((SendPacket) new PROTOCOL_CS_REPLACE_PERSONMAX_ACK(clan.maxPlayers));
              }
              else
                this.erro = 2147487830U;
            }
            else
              this.erro = 2147487830U;
          }
          else if (this.itemId == 1800056)
          {
            PointBlank.Core.Models.Account.Clan.Clan clan = ClanManager.getClan(player.clanId);
            if (clan._id > 0 && (double) clan._pontos != 1000.0)
            {
              if (ComDiv.updateDB("clan_data", "pontos", (object) 1000f, "clan_id", (object) player.clanId))
              {
                clan._pontos = 1000f;
                this._client.SendPacket((SendPacket) new PROTOCOL_CS_POINT_RESET_RESULT_ACK());
              }
              else
                this.erro = 2147487830U;
            }
            else
              this.erro = 2147487830U;
          }
          else if (this.itemId > 1800113 && this.itemId < 1800119)
          {
            int increase = this.itemId == 1800114 ? 500 : (this.itemId == 1800115 ? 1000 : (this.itemId == 1800116 ? 5000 : (this.itemId == 1800117 ? 10000 : 30000)));
            if (ComDiv.updateDB("accounts", "gp", (object) (player._gp + increase), "player_id", (object) player.player_id))
            {
              player._gp += increase;
              this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_PLUS_POINT_ACK(increase, player._gp, 0));
            }
            else
              this.erro = 2147483648U;
          }
          else if (itemsModel1._category == 3 && RandomBoxXml.ContainsBox(this.itemId))
          {
            RandomBoxModel box = RandomBoxXml.getBox(this.itemId);
            if (box != null)
            {
              List<RandomBoxItem> sortedList = box.getSortedList(PROTOCOL_AUTH_SHOP_ITEM_AUTH_REQ.GetRandomNumber(1, 100));
              List<RandomBoxItem> rewardList = box.getRewardList(sortedList, PROTOCOL_AUTH_SHOP_ITEM_AUTH_REQ.GetRandomNumber(0, sortedList.Count));
              if (rewardList.Count > 0)
              {
                int index1 = rewardList[0].index;
                List<ItemsModel> Rewards = new List<ItemsModel>();
                for (int index2 = 0; index2 < rewardList.Count; ++index2)
                {
                  RandomBoxItem randomBoxItem = rewardList[index2];
                  if (randomBoxItem.item != null)
                    Rewards.Add(randomBoxItem.item);
                  else if (PlayerManager.updateAccountGold(player.player_id, player._gp + (int) randomBoxItem.count))
                  {
                    player._gp += (int) randomBoxItem.count;
                    this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_PLUS_POINT_ACK((int) randomBoxItem.count, player._gp, 0));
                  }
                  else
                  {
                    this.erro = 2147483648U;
                    break;
                  }
                  if (randomBoxItem.special)
                  {
                    using (PROTOCOL_AUTH_SHOP_JACKPOT_ACK authShopJackpotAck = new PROTOCOL_AUTH_SHOP_JACKPOT_ACK(player.player_name, this.itemId, index1))
                      GameManager.SendPacketToAllClients((SendPacket) authShopJackpotAck);
                  }
                }
                this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_CAPSULE_ACK(Rewards, this.itemId, index1));
                if (Rewards.Count > 0)
                {
                  for (int index2 = 0; index2 < Rewards.Count; ++index2)
                  {
                    ItemsModel itemsModel2 = Rewards[index2];
                    if (itemsModel2._id != 0)
                      this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(0, player, itemsModel2));
                  }
                }
              }
              else
                this.erro = 2147483648U;
            }
            else
              this.erro = 2147483648U;
          }
          else
          {
            int idStatics = ComDiv.getIdStatics(itemsModel1._id, 1);
            switch (idStatics)
            {
              case 0:
              case 1:
              case 2:
              case 3:
              case 4:
              case 5:
              case 6:
              case 7:
              case 8:
              case 27:
                if (itemsModel1._equip == 1)
                {
                  itemsModel1._equip = 2;
                  ItemsModel itemsModel2 = itemsModel1;
                  DateTime dateTime = DateTime.Now;
                  dateTime = dateTime.AddSeconds((double) itemsModel1._count);
                  long int64 = Convert.ToInt64(dateTime.ToString("yyMMddHHmm"));
                  itemsModel2._count = int64;
                  ComDiv.updateDB("player_items", "object_id", (object) this.objId, "owner_id", (object) player.player_id, new string[2]
                  {
                    "count",
                    "equip"
                  }, (object) itemsModel1._count, (object) itemsModel1._equip);
                  if (idStatics == 6)
                  {
                    Character character = player.getCharacter(itemsModel1._id);
                    if (character != null)
                    {
                      this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CHANGE_STATE_ACK(character));
                      break;
                    }
                    break;
                  }
                  break;
                }
                this.erro = 2147483648U;
                break;
              case 17:
                this.cupomIncreaseDays(player, itemsModel1._name);
                break;
              case 20:
                this.cupomIncreaseGold(player, itemsModel1._id);
                break;
              default:
                this.erro = 2147483648U;
                break;
            }
          }
        }
        else
          this.erro = 2147483648U;
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEM_AUTH_ACK(this.erro, itemsModel1, player));
      }
      catch (OverflowException ex)
      {
        Logger.error("Obj: " + (object) this.objId + " ItemId: " + (object) this.itemId + " Count: " + (object) this.oldCOUNT + " PlayerId: " + (object) this._client.player_id + " Name: '" + this._client._player.player_name + "'\r\n" + ex.ToString());
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEM_AUTH_ACK(2147483648U, (ItemsModel) null, (PointBlank.Game.Data.Model.Account) null));
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_AUTH_SHOP_ITEM_AUTH_REQ: " + ex.ToString());
        this._client.SendPacket((SendPacket) new PROTOCOL_AUTH_SHOP_ITEM_AUTH_ACK(2147483648U, (ItemsModel) null, (PointBlank.Game.Data.Model.Account) null));
      }
    }

    private static int GetRandomNumber(int min, int max)
    {
      lock (PROTOCOL_AUTH_SHOP_ITEM_AUTH_REQ.syncLock)
        return PROTOCOL_AUTH_SHOP_ITEM_AUTH_REQ.getrandom.Next(min, max);
    }

    private void cupomIncreaseDays(PointBlank.Game.Data.Model.Account p, string originalName)
    {
      int itemId = ComDiv.CreateItemId(16, 0, ComDiv.getIdStatics(this.itemId, 3));
      int idStatics = ComDiv.getIdStatics(this.itemId, 2);
      CouponEffects effects = p.effects;
      if (itemId == 1600065 && (effects & (CouponEffects.Defense20 | CouponEffects.Defense10 | CouponEffects.Defense5)) > (CouponEffects) 0 || itemId == 1600079 && (effects & (CouponEffects.Defense90 | CouponEffects.Defense10 | CouponEffects.Defense5)) > (CouponEffects) 0 || (itemId == 1600044 && (effects & (CouponEffects.Defense90 | CouponEffects.Defense20 | CouponEffects.Defense5)) > (CouponEffects) 0 || itemId == 1600030 && (effects & (CouponEffects.Defense90 | CouponEffects.Defense20 | CouponEffects.Defense10)) > (CouponEffects) 0) || (itemId == 1600078 && (effects & (CouponEffects.JackHollowPoint | CouponEffects.HollowPoint | CouponEffects.FullMetalJack)) > (CouponEffects) 0 || itemId == 1600032 && (effects & (CouponEffects.HollowPointPlus | CouponEffects.JackHollowPoint | CouponEffects.FullMetalJack)) > (CouponEffects) 0 || (itemId == 1600031 && (effects & (CouponEffects.HollowPointPlus | CouponEffects.JackHollowPoint | CouponEffects.HollowPoint)) > (CouponEffects) 0 || itemId == 1600036 && (effects & (CouponEffects.HollowPointPlus | CouponEffects.HollowPoint | CouponEffects.FullMetalJack)) > (CouponEffects) 0)) || (itemId == 1600028 && effects.HasFlag((Enum) CouponEffects.HP5) || itemId == 1600040 && effects.HasFlag((Enum) CouponEffects.HP10)))
      {
        this.erro = 2147483648U;
      }
      else
      {
        ItemsModel itemsModel1 = p._inventory.getItem(itemId);
        if (itemsModel1 == null)
        {
          bool flag = p._bonus.AddBonuses(itemId);
          CouponFlag couponEffect = CouponEffectManager.getCouponEffect(itemId);
          if (couponEffect != null && couponEffect.EffectFlag > (CouponEffects) 0 && !p.effects.HasFlag((Enum) couponEffect.EffectFlag))
          {
            p.effects |= couponEffect.EffectFlag;
            PlayerManager.updateCupomEffects(p.player_id, p.effects);
          }
          if (flag)
            PlayerManager.updatePlayerBonus(p.player_id, p._bonus.bonuses, p._bonus.freepass);
          GameClient client = this._client;
          PointBlank.Game.Data.Model.Account Player = p;
          int id = itemId;
          string name = originalName + " [Active]";
          DateTime dateTime = DateTime.Now;
          dateTime = dateTime.AddDays((double) idStatics);
          long int64 = Convert.ToInt64(dateTime.ToString("yyMMddHHmm"));
          ItemsModel itemsModel2 = new ItemsModel(id, 3, name, 2, int64, 0L);
          PROTOCOL_INVENTORY_GET_INFO_ACK inventoryGetInfoAck = new PROTOCOL_INVENTORY_GET_INFO_ACK(0, Player, itemsModel2);
          client.SendPacket((SendPacket) inventoryGetInfoAck);
        }
        else
        {
          DateTime exact = DateTime.ParseExact(itemsModel1._count.ToString(), "yyMMddHHmm", (IFormatProvider) CultureInfo.InvariantCulture);
          itemsModel1._count = Convert.ToInt64(exact.AddDays((double) idStatics).ToString("yyMMddHHmm"));
          ComDiv.updateDB("player_items", "count", (object) itemsModel1._count, "object_id", (object) itemsModel1._objId, "owner_id", (object) p.player_id);
          this._client.SendPacket((SendPacket) new PROTOCOL_INVENTORY_GET_INFO_ACK(1, p, itemsModel1));
        }
      }
    }

    private void cupomIncreaseGold(PointBlank.Game.Data.Model.Account p, int cupomId)
    {
      int increase = ComDiv.getIdStatics(cupomId, 3) * 100 + ComDiv.getIdStatics(cupomId, 2) * 100000;
      if (PlayerManager.updateAccountGold(p.player_id, p._gp + increase))
      {
        p._gp += increase;
        this._client.SendPacket((SendPacket) new PROTOCOL_SHOP_PLUS_POINT_ACK(increase, p._gp, 0));
      }
      else
        this.erro = 2147483648U;
    }
  }
}
