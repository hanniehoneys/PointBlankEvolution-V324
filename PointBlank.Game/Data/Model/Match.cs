using PointBlank.Core.Models.Enums;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Utils;
using PointBlank.Game.Data.Xml;
using PointBlank.Game.Network.ServerPacket;
using System.Collections.Generic;
using System.Threading;

namespace PointBlank.Game.Data.Model
{
  public class Match
  {
    public int _matchId = -1;
    public SlotMatch[] _slots = new SlotMatch[8];
    public MatchState _state = MatchState.Ready;
    public PointBlank.Core.Models.Account.Clan.Clan clan;
    public int formação;
    public int serverId;
    public int channelId;
    public int _leader;
    public int friendId;

    public Match(PointBlank.Core.Models.Account.Clan.Clan clan)
    {
      this.clan = clan;
      for (int slot = 0; slot < 8; ++slot)
        this._slots[slot] = new SlotMatch(slot);
    }

    public bool getSlot(int slotId, out SlotMatch slot)
    {
      lock (this._slots)
      {
        slot = (SlotMatch) null;
        if (slotId >= 0 && slotId < 16)
          slot = this._slots[slotId];
        return slot != null;
      }
    }

    public SlotMatch getSlot(int slotId)
    {
      lock (this._slots)
      {
        if (slotId >= 0 && slotId < 16)
          return this._slots[slotId];
        return (SlotMatch) null;
      }
    }

    public void setNewLeader(int leader, int oldLeader)
    {
      Monitor.Enter((object) this._slots);
      if (leader == -1)
      {
        for (int index = 0; index < this.formação; ++index)
        {
          if (index != oldLeader && this._slots[index]._playerId > 0L)
          {
            this._leader = index;
            break;
          }
        }
      }
      else
        this._leader = leader;
      Monitor.Exit((object) this._slots);
    }

    public bool addPlayer(PointBlank.Game.Data.Model.Account player)
    {
      lock (this._slots)
      {
        for (int index = 0; index < this.formação; ++index)
        {
          SlotMatch slot = this._slots[index];
          if (slot._playerId == 0L && slot.state == SlotMatchState.Empty)
          {
            slot._playerId = player.player_id;
            slot.state = SlotMatchState.Normal;
            player._match = this;
            player.matchSlot = index;
            player._status.updateClanMatch((byte) this.friendId);
            AllUtils.syncPlayerToClanMembers(player);
            return true;
          }
        }
      }
      return false;
    }

    public PointBlank.Game.Data.Model.Account getPlayerBySlot(SlotMatch slot)
    {
      try
      {
        long playerId = slot._playerId;
        return playerId > 0L ? AccountManager.getAccount(playerId, true) : (PointBlank.Game.Data.Model.Account) null;
      }
      catch
      {
        return (PointBlank.Game.Data.Model.Account) null;
      }
    }

    public PointBlank.Game.Data.Model.Account getPlayerBySlot(int slotId)
    {
      try
      {
        long playerId = this._slots[slotId]._playerId;
        return playerId > 0L ? AccountManager.getAccount(playerId, true) : (PointBlank.Game.Data.Model.Account) null;
      }
      catch
      {
        return (PointBlank.Game.Data.Model.Account) null;
      }
    }

    public List<PointBlank.Game.Data.Model.Account> getAllPlayers(int exception)
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      lock (this._slots)
      {
        for (int index = 0; index < 8; ++index)
        {
          long playerId = this._slots[index]._playerId;
          if (playerId > 0L && index != exception)
          {
            PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(playerId, true);
            if (account != null)
              accountList.Add(account);
          }
        }
      }
      return accountList;
    }

    public List<PointBlank.Game.Data.Model.Account> getAllPlayers()
    {
      List<PointBlank.Game.Data.Model.Account> accountList = new List<PointBlank.Game.Data.Model.Account>();
      lock (this._slots)
      {
        for (int index = 0; index < 8; ++index)
        {
          long playerId = this._slots[index]._playerId;
          if (playerId > 0L)
          {
            PointBlank.Game.Data.Model.Account account = AccountManager.getAccount(playerId, true);
            if (account != null)
              accountList.Add(account);
          }
        }
      }
      return accountList;
    }

    public void SendPacketToPlayers(SendPacket packet)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = this.getAllPlayers();
      if (allPlayers.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("Match.SendPacketToPlayers(SendPacket)");
      for (int index = 0; index < allPlayers.Count; ++index)
        allPlayers[index].SendCompletePacket(completeBytes);
    }

    public void SendPacketToPlayers(SendPacket packet, int exception)
    {
      List<PointBlank.Game.Data.Model.Account> allPlayers = this.getAllPlayers(exception);
      if (allPlayers.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("Match.SendPacketToPlayers(SendPacket,int)");
      for (int index = 0; index < allPlayers.Count; ++index)
        allPlayers[index].SendCompletePacket(completeBytes);
    }

    public PointBlank.Game.Data.Model.Account getLeader()
    {
      try
      {
        return AccountManager.getAccount(this._slots[this._leader]._playerId, true);
      }
      catch
      {
        return (PointBlank.Game.Data.Model.Account) null;
      }
    }

    public int getServerInfo()
    {
      return this.channelId + this.serverId * 10;
    }

    public int getCountPlayers()
    {
      lock (this._slots)
      {
        int num = 0;
        for (int index = 0; index < this._slots.Length; ++index)
        {
          if (this._slots[index]._playerId > 0L)
            ++num;
        }
        return num;
      }
    }

    private void BaseRemovePlayer(PointBlank.Game.Data.Model.Account p)
    {
      lock (this._slots)
      {
        SlotMatch slot;
        if (!this.getSlot(p.matchSlot, out slot) || slot._playerId != p.player_id)
          return;
        slot._playerId = 0L;
        slot.state = SlotMatchState.Empty;
      }
    }

    public bool RemovePlayer(PointBlank.Game.Data.Model.Account p)
    {
      Channel channel = ChannelsXml.getChannel(this.channelId);
      if (channel == null)
        return false;
      this.BaseRemovePlayer(p);
      if (this.getCountPlayers() == 0)
      {
        channel.RemoveMatch(this._matchId);
      }
      else
      {
        if (p.matchSlot == this._leader)
          this.setNewLeader(-1, -1);
        using (PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK registMercenaryAck = new PROTOCOL_CLAN_WAR_REGIST_MERCENARY_ACK(this))
          this.SendPacketToPlayers((SendPacket) registMercenaryAck);
      }
      p.matchSlot = -1;
      p._match = (Match) null;
      return true;
    }
  }
}
