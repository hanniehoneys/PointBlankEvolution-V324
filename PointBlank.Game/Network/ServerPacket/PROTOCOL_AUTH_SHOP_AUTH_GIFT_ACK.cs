using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SHOP_AUTH_GIFT_ACK : SendPacket
  {
    private List<ItemsModel> charas = new List<ItemsModel>();
    private List<ItemsModel> weapons = new List<ItemsModel>();
    private List<ItemsModel> cupons = new List<ItemsModel>();
    private uint _erro;

    public PROTOCOL_AUTH_SHOP_AUTH_GIFT_ACK(uint erro, ItemsModel item = null, PointBlank.Game.Data.Model.Account p = null)
    {
      this._erro = erro;
      if (this._erro != 1U)
        return;
      this.get(item, p);
    }

    public override void write()
    {
      this.writeH((short) 541);
      this.writeD(this._erro);
      if (this._erro != 1U)
        return;
      this.writeD(this.charas.Count);
      this.writeD(this.weapons.Count);
      this.writeD(this.cupons.Count);
      this.writeD(0);
      for (int index = 0; index < this.charas.Count; ++index)
      {
        ItemsModel chara = this.charas[index];
        this.writeQ(chara._objId);
        this.writeD(chara._id);
        this.writeC((byte) chara._equip);
        this.writeD((int) chara._count);
      }
      for (int index = 0; index < this.weapons.Count; ++index)
      {
        ItemsModel weapon = this.weapons[index];
        this.writeQ(weapon._objId);
        this.writeD(weapon._id);
        this.writeC((byte) weapon._equip);
        this.writeD((int) weapon._count);
      }
      for (int index = 0; index < this.cupons.Count; ++index)
      {
        ItemsModel cupon = this.cupons[index];
        this.writeQ(cupon._objId);
        this.writeD(cupon._id);
        this.writeC((byte) cupon._equip);
        this.writeD((int) cupon._count);
      }
    }

    private void get(ItemsModel item, PointBlank.Game.Data.Model.Account p)
    {
      try
      {
        ItemsModel modelo = new ItemsModel(item)
        {
          _objId = item._objId
        };
        PlayerManager.tryCreateItem(modelo, p._inventory, p.player_id);
        if (modelo._category == 1)
          this.weapons.Add(modelo);
        else if (modelo._category == 2)
        {
          this.charas.Add(modelo);
        }
        else
        {
          if (modelo._category != 3)
            return;
          this.cupons.Add(modelo);
        }
      }
      catch (Exception ex)
      {
        Logger.error("PROTOCOL_AUTH_SHOP_AUTH_GIFT_ACK: " + ex.ToString());
      }
    }
  }
}
