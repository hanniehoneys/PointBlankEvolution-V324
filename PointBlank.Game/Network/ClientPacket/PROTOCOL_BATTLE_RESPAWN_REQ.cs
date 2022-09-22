using PointBlank.Core;
using PointBlank.Core.Managers;
using PointBlank.Core.Models.Account.Players;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Sync;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_RESPAWN_REQ : ReceivePacket
  {
    private PlayerEquipedItems Equip;
    private int WeaponsFlag;

    public PROTOCOL_BATTLE_RESPAWN_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
      PointBlank.Game.Data.Model.Room room = this._client._player._room;
      bool flag = this._client._player._slotId % 2 == 0;
      this.Equip = new PlayerEquipedItems();
      this.Equip._primary = this.readD();
      this.readD();
      this.Equip._secondary = this.readD();
      this.readD();
      this.Equip._melee = this.readD();
      this.readD();
      this.Equip._grenade = this.readD();
      this.readD();
      this.Equip._special = this.readD();
      this.readD();
      if (room.room_type == RoomType.Boss || room.room_type == RoomType.CrossCounter)
      {
        if (!room.swapRound)
        {
          if (flag)
          {
            this.Equip._red = this._client._player._equip._red;
            this.Equip._blue = this._client._player._equip._blue;
            this.Equip._dino = this.readD();
            this.readD();
          }
          else
          {
            this.Equip._red = this._client._player._equip._red;
            this.Equip._blue = this.readD();
            this.Equip._dino = this._client._player._equip._dino;
            this.readD();
          }
        }
        else if (flag)
        {
          this.Equip._red = this._client._player._equip._red;
          this.Equip._blue = this.readD();
          this.Equip._dino = this._client._player._equip._dino;
          this.readD();
        }
        else
        {
          this.Equip._red = this._client._player._equip._red;
          this.Equip._blue = this._client._player._equip._blue;
          this.Equip._dino = this.readD();
          this.readD();
        }
      }
      else
      {
        if (flag)
        {
          this.Equip._red = this.readD();
          this.Equip._blue = this._client._player._equip._blue;
          this.readD();
        }
        else
        {
          this.Equip._red = this._client._player._equip._red;
          this.Equip._blue = this.readD();
          this.readD();
        }
        this.Equip._dino = this._client._player._equip._dino;
      }
      this.Equip.face = this.readD();
      this.readD();
      this.Equip._helmet = this.readD();
      this.readD();
      this.Equip.jacket = this.readD();
      this.readD();
      this.Equip.poket = this.readD();
      this.readD();
      this.Equip.glove = this.readD();
      this.readD();
      this.Equip.belt = this.readD();
      this.readD();
      this.Equip.holster = this.readD();
      this.readD();
      this.Equip.skin = this.readD();
      this.readD();
      this.Equip._beret = this.readD();
      this.readD();
      this.WeaponsFlag = (int) this.readH();
    }

    public override void run()
    {
      try
      {
        PointBlank.Game.Data.Model.Account player = this._client._player;
        if (player == null)
          return;
        PointBlank.Game.Data.Model.Room room = player._room;
        if (room == null || room._state != RoomState.Battle)
          return;
        Slot slot = room.getSlot(player._slotId);
        if (slot == null || slot.state != SlotState.BATTLE)
          return;
        if (slot._deathState.HasFlag((Enum) DeadEnum.Dead) || slot._deathState.HasFlag((Enum) DeadEnum.UseChat))
          slot._deathState = DeadEnum.Alive;
        PlayerManager.CheckEquipedItems(this.Equip, player._inventory._items, true);
        this.CheckEquipment(player, room, this.Equip);
        slot._equip = this.Equip;
        if ((this.WeaponsFlag & 8) > 0)
          this.InsertItem(this.Equip._primary, slot);
        if ((this.WeaponsFlag & 4) > 0)
          this.InsertItem(this.Equip._secondary, slot);
        if ((this.WeaponsFlag & 2) > 0)
          this.InsertItem(this.Equip._melee, slot);
        if ((this.WeaponsFlag & 1) > 0)
          this.InsertItem(this.Equip._grenade, slot);
        this.InsertItem(this.Equip._special, slot);
        if (slot._team == 0)
          this.InsertItem(this.Equip._red, slot);
        else
          this.InsertItem(this.Equip._blue, slot);
        if (room.MaskActive)
        {
          this.Equip._helmet = 1000800000;
          this.InsertItem(this.Equip._helmet, slot);
        }
        else
          this.InsertItem(this.Equip._helmet, slot);
        this.InsertItem(this.Equip._beret, slot);
        this.InsertItem(this.Equip._dino, slot);
        using (PROTOCOL_BATTLE_RESPAWN_ACK battleRespawnAck = new PROTOCOL_BATTLE_RESPAWN_ACK(room, slot))
          room.SendPacketToPlayers((SendPacket) battleRespawnAck, SlotState.BATTLE, 0);
        if (slot.firstRespawn)
        {
          slot.firstRespawn = false;
          GameSync.SendUDPPlayerSync(room, slot, player.effects, 0);
        }
        else
          GameSync.SendUDPPlayerSync(room, slot, player.effects, 2);
      }
      catch (Exception ex)
      {
        Logger.warning("PROTOCOL_BATTLE_RESPAWN_REQ: " + ex.ToString());
      }
    }

    public void CheckEquipment(PointBlank.Game.Data.Model.Account Player, PointBlank.Game.Data.Model.Room Room, PlayerEquipedItems Equip)
    {
      if (Room.BarrettActive && (Equip._primary == 105032 || Equip._primary == 105082 || (Equip._primary == 105232 || Equip._primary == 105292)))
        Equip._primary = !Room.SniperMode ? 103004 : 105003;
      if (Room.ShotgunActive && Equip._primary < 107000 && Equip._primary > 106000)
        Equip._primary = 103004;
      if (!Room.GameRuleActive)
        return;
      for (int index = 0; index < GameRuleManager.GameRules.Count; ++index)
      {
        GameRule gameRule = GameRuleManager.GameRules[index];
        int idStatics1 = ComDiv.getIdStatics(gameRule.WeaponId, 1);
        int idStatics2 = ComDiv.getIdStatics(gameRule.WeaponId, 2);
        if (idStatics1 == 1 && Equip._primary == gameRule.WeaponId)
        {
          Equip._primary = !Room.SniperMode ? 103004 : 105003;
          Equip._primary = !Room.ShotgunMode ? 103004 : 106001;
        }
        if (idStatics1 == 2 && Equip._secondary == gameRule.WeaponId)
          Equip._secondary = 202003;
        if (idStatics1 == 3 && Equip._melee == gameRule.WeaponId)
          Equip._melee = 301001;
        if (idStatics1 == 4 && Equip._grenade == gameRule.WeaponId)
          Equip._grenade = 407001;
        if (idStatics1 == 5 && Equip._special == gameRule.WeaponId)
          Equip._special = 508001;
        if (idStatics1 == 6)
        {
          switch (idStatics2)
          {
            case 1:
              if (Equip._red == gameRule.WeaponId)
              {
                Equip._red = 601001;
                break;
              }
              break;
            case 2:
              if (Equip._blue == gameRule.WeaponId)
              {
                Equip._blue = 602002;
                break;
              }
              break;
          }
        }
        if (idStatics1 == 27 && Equip._beret == gameRule.WeaponId)
          Equip._beret = 0;
        if (idStatics1 == 8 && Equip._helmet == gameRule.WeaponId)
          Equip._helmet = 1000800000;
      }
    }

    private void InsertItem(int id, Slot slot)
    {
      lock (slot.armas_usadas)
      {
        if (slot.armas_usadas.Contains(id))
          return;
        slot.armas_usadas.Add(id);
      }
    }
  }
}
