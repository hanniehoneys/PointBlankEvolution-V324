using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Network;
using System;

namespace PointBlank.Game.Network.ServerPacket
{
  public class PROTOCOL_AUTH_SHOP_ITEM_AUTH_ACK : SendPacket
  {
    private uint erro;
    private ItemsModel item;

    public PROTOCOL_AUTH_SHOP_ITEM_AUTH_ACK(uint erro, ItemsModel item = null, PointBlank.Game.Data.Model.Account p = null)
    {
      this.erro = erro;
      if (erro != 1U)
        return;
      if (item != null)
      {
        switch (ComDiv.getIdStatics(item._id, 1))
        {
          case 17:
          case 18:
          case 20:
            if (item._count > 1L && item._equip == 1)
            {
              ComDiv.updateDB("player_items", "count", (object) --item._count, "object_id", (object) item._objId, "owner_id", (object) p.player_id);
              break;
            }
            PlayerManager.DeleteItem(item._objId, p.player_id);
            p._inventory.RemoveItem(item);
            item._id = 0;
            item._count = 0L;
            break;
          default:
            item._equip = 2;
            break;
        }
        this.item = item;
      }
      else
        this.erro = 2147483648U;
    }

    public override void write()
    {
      this.writeH((short) 1048);
      this.writeD(this.erro);
      if (this.erro != 1U)
        return;
      this.writeD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
      this.writeD((int) this.item._objId);
      if (this.item._category == 3 && this.item._equip == 2)
      {
        this.writeD(0);
        this.writeC((byte) 1);
        this.writeD(0);
      }
      else
      {
        this.writeD(this.item._id);
        this.writeC((byte) this.item._equip);
        this.writeD((int) this.item._count);
      }
    }
  }
}
