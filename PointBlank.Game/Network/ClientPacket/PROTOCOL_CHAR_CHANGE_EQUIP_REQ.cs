using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Network.ServerPacket;
using System;
using System.Linq;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_CHAR_CHANGE_EQUIP_REQ : ReceivePacket
  {
    private PointBlank.Game.Data.Model.Account Player;
    private PlayerEquipedItems Equip;
    private uint Error;
    private int CharCount;
    private int ItemEnable;
    private int ItemDisable;

    public PROTOCOL_CHAR_CHANGE_EQUIP_REQ(GameClient Client, byte[] Buffer)
    {
      this.makeme(Client, Buffer);
    }

    public override void read()
    {
      this.Equip = new PlayerEquipedItems();
      this.Player = this._client._player;
      this.ItemDisable = (int) this.readC();
      for (int index = 0; index < this.ItemDisable; ++index)
        this.ReadAndUpdateItemDisable(this.Player);
      int num1 = (int) this.readC();
      int num2 = (int) this.readC();
      this.ItemEnable = (int) this.readC();
      for (int index = 0; index < this.ItemEnable; ++index)
        this.ReadAndUpdateItemEnable(this.Player);
      int num3 = (int) this.readC();
      int num4 = (int) this.readC();
      this.CharCount = (int) this.readC();
      for (int index = 0; index < this.CharCount; ++index)
        this.ReadAndUpdateCharacterEquipment(this.Player, this.Equip);
      int num5 = (int) this.readC();
      this.ReadAndUpdateSlot(this.Player, this.Equip);
    }

    public override void run()
    {
      DBQuery query = new DBQuery();
      Room room = this.Player._room;
      if (room != null)
        AllUtils.updateSlotEquips(this.Player, room);
      if (this.Equip._dino == 0)
        this.Equip._dino = this.Player._equip._dino;
      if (this.Player != null)
      {
        if (this.CharCount > 0)
        {
          PlayerManager.updateWeapons(this.Equip, this.Player._equip, query);
          PlayerManager.updateChars(this.Equip, this.Player._equip, query);
        }
        else
          PlayerManager.updateCharSlots(this.Equip, this.Player._equip, query);
      }
      else
        this.Error = 2147483648U;
      if (ComDiv.updateDB("accounts", "player_id", (object) this.Player.player_id, query.GetTables(), query.GetValues()))
        this.Update(this.Player);
      this._client.SendPacket((SendPacket) new PROTOCOL_CHAR_CHANGE_EQUIP_ACK(this.Error));
    }

    private void ReadAndUpdateItemDisable(PointBlank.Game.Data.Model.Account Player)
    {
      int CouponId = this.readD();
      ItemsModel itemsModel = Player._inventory._items.FirstOrDefault<ItemsModel>((Func<ItemsModel, bool>) (x => x._id == CouponId));
      if (itemsModel == null)
        return;
      CouponFlag couponEffect = CouponEffectManager.getCouponEffect(itemsModel._id);
      if (couponEffect == null || couponEffect.EffectFlag <= (CouponEffects) 0 || !Player.effects.HasFlag((Enum) couponEffect.EffectFlag))
        return;
      Player.effects -= couponEffect.EffectFlag;
      PlayerManager.updateCupomEffects(Player.player_id, Player.effects);
    }

    private void ReadAndUpdateItemEnable(PointBlank.Game.Data.Model.Account Player)
    {
      int CouponId = this.readD();
      ItemsModel itemsModel = Player._inventory._items.FirstOrDefault<ItemsModel>((Func<ItemsModel, bool>) (x => x._id == CouponId));
      if (itemsModel == null)
        return;
      bool flag = Player._bonus.AddBonuses(itemsModel._id);
      CouponFlag couponEffect = CouponEffectManager.getCouponEffect(itemsModel._id);
      if (couponEffect != null && couponEffect.EffectFlag > (CouponEffects) 0 && !Player.effects.HasFlag((Enum) couponEffect.EffectFlag))
      {
        Player.effects |= couponEffect.EffectFlag;
        PlayerManager.updateCupomEffects(Player.player_id, Player.effects);
      }
      if (!flag)
        return;
      PlayerManager.updatePlayerBonus(Player.player_id, Player._bonus.bonuses, Player._bonus.freepass);
    }

    private void ReadAndUpdateSlot(PointBlank.Game.Data.Model.Account Player, PlayerEquipedItems Equip)
    {
      Equip._dino = this.readD();
      this.readD();
      int num1 = (int) this.readC();
      int num2 = (int) this.readC();
      for (int index = 0; index < num2; ++index)
      {
        int Slot = (int) this.readC();
        Character characterSlot = Player.getCharacterSlot(Slot);
        if (characterSlot != null)
        {
          switch (index)
          {
            case 0:
              Equip._red = characterSlot.Id;
              continue;
            case 1:
              Equip._blue = characterSlot.Id;
              continue;
            default:
              continue;
          }
        }
      }
    }

    private void ReadAndUpdateCharacterEquipment(PointBlank.Game.Data.Model.Account Player, PlayerEquipedItems Equip)
    {
      int Slot = (int) this.readC();
      Player.getCharacterSlot(Slot);
      for (int index = 0; index < 14; ++index)
      {
        int ItemId = this.readD();
        this.readD();
        if (Player._inventory._items.FirstOrDefault<ItemsModel>((Func<ItemsModel, bool>) (x => x._id == ItemId)) != null)
        {
          switch (index)
          {
            case 0:
              Equip._primary = ItemId;
              continue;
            case 1:
              Equip._secondary = ItemId;
              continue;
            case 2:
              Equip._melee = ItemId;
              continue;
            case 3:
              Equip._grenade = ItemId;
              continue;
            case 4:
              Equip._special = ItemId;
              continue;
            case 5:
              if (ComDiv.getIdStatics(ItemId, 2) == 1)
              {
                Equip._red = ItemId;
                Equip._blue = Player._equip._blue;
                continue;
              }
              if (ComDiv.getIdStatics(ItemId, 2) == 2)
              {
                Equip._red = Player._equip._red;
                Equip._blue = ItemId;
                continue;
              }
              continue;
            case 6:
              Equip.face = ItemId;
              continue;
            case 7:
              Equip._helmet = ItemId;
              continue;
            case 8:
              Equip.jacket = ItemId;
              continue;
            case 9:
              Equip.poket = ItemId;
              continue;
            case 10:
              Equip.glove = ItemId;
              continue;
            case 11:
              Equip.belt = ItemId;
              continue;
            case 12:
              Equip.holster = ItemId;
              continue;
            case 13:
              Equip.skin = ItemId;
              continue;
            default:
              continue;
          }
        }
      }
      Equip._beret = this.readD();
      this.readD();
    }

    private void Update(PointBlank.Game.Data.Model.Account p)
    {
      if (this.CharCount > 0)
      {
        p._equip._primary = this.Equip._primary;
        p._equip._secondary = this.Equip._secondary;
        p._equip._melee = this.Equip._melee;
        p._equip._grenade = this.Equip._grenade;
        p._equip._special = this.Equip._special;
        p._equip._red = this.Equip._red;
        p._equip._blue = this.Equip._blue;
        p._equip._helmet = this.Equip._helmet;
        p._equip._beret = this.Equip._beret;
        p._equip._dino = this.Equip._dino;
        p._equip.face = this.Equip.face;
        p._equip.jacket = this.Equip.jacket;
        p._equip.poket = this.Equip.poket;
        p._equip.glove = this.Equip.glove;
        p._equip.belt = this.Equip.belt;
        p._equip.holster = this.Equip.holster;
        p._equip.skin = this.Equip.skin;
      }
      else
      {
        p._equip._red = this.Equip._red;
        p._equip._blue = this.Equip._blue;
        p._equip._dino = this.Equip._dino;
      }
    }
  }
}
