using PointBlank.Core;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Managers;
using PointBlank.Game.Data.Sync;
using System;
using System.Collections.Generic;

namespace PointBlank.Game.Data.Model
{
  public class Channel
  {
    public List<PlayerSession> _players = new List<PlayerSession>();
    public List<Room> _rooms = new List<Room>();
    public List<Match> _matchs = new List<Match>();
    private DateTime LastRoomsSync = DateTime.Now;
    public int _id;
    public int _type;
    public int serverId;

    public PlayerSession getPlayer(uint session)
    {
      lock (this._players)
      {
        try
        {
          for (int index = 0; index < this._players.Count; ++index)
          {
            PlayerSession player = this._players[index];
            if ((int) player._sessionId == (int) session)
              return player;
          }
        }
        catch (Exception ex)
        {
          Logger.warning(ex.ToString());
        }
        return (PlayerSession) null;
      }
    }

    public PlayerSession getPlayer(uint session, out int idx)
    {
      idx = -1;
      lock (this._players)
      {
        try
        {
          for (int index = 0; index < this._players.Count; ++index)
          {
            PlayerSession player = this._players[index];
            if ((int) player._sessionId == (int) session)
            {
              idx = index;
              return player;
            }
          }
        }
        catch (Exception ex)
        {
          Logger.warning(ex.ToString());
        }
        return (PlayerSession) null;
      }
    }

    public bool AddPlayer(PlayerSession pS)
    {
      lock (this._players)
      {
        if (this._players.Contains(pS))
          return false;
        this._players.Add(pS);
        GameSync.UpdateGSCount(this.serverId);
        return true;
      }
    }

    public void RemoveMatch(int matchId)
    {
      try
      {
        lock (this._matchs)
        {
          for (int index = 0; index < this._matchs.Count; ++index)
          {
            if (matchId == this._matchs[index]._matchId)
            {
              this._matchs.RemoveAt(index);
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
    }

    public void AddMatch(Match match)
    {
      lock (this._matchs)
      {
        if (this._matchs.Contains(match))
          return;
        this._matchs.Add(match);
      }
    }

    public void AddRoom(Room room)
    {
      lock (this._rooms)
        this._rooms.Add(room);
    }

    public void RemoveEmptyRooms()
    {
      try
      {
        lock (this._rooms)
        {
          if ((DateTime.Now - this.LastRoomsSync).TotalSeconds < 2.0)
            return;
          this.LastRoomsSync = DateTime.Now;
          for (int index = 0; index < this._rooms.Count; ++index)
          {
            if (this._rooms[index].getAllPlayers().Count < 1)
              this._rooms.RemoveAt(index--);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.warning("[Channel.RemoveEmptyRooms] " + ex.ToString());
      }
    }

    public Match getMatch(int id)
    {
      lock (this._matchs)
      {
        try
        {
          for (int index = 0; index < this._matchs.Count; ++index)
          {
            Match match = this._matchs[index];
            if (match._matchId == id)
              return match;
          }
        }
        catch (Exception ex)
        {
          Logger.warning(ex.ToString());
        }
        return (Match) null;
      }
    }

    public Match getMatch(int id, int clan)
    {
      lock (this._matchs)
      {
        try
        {
          for (int index = 0; index < this._matchs.Count; ++index)
          {
            Match match = this._matchs[index];
            if (match.friendId == id && match.clan._id == clan)
              return match;
          }
        }
        catch (Exception ex)
        {
          Logger.warning(ex.ToString());
        }
        return (Match) null;
      }
    }

    public Room getRoom(int id)
    {
      lock (this._rooms)
      {
        try
        {
          for (int index = 0; index < this._rooms.Count; ++index)
          {
            Room room = this._rooms[index];
            if (room._roomId == id)
              return room;
          }
        }
        catch (Exception ex)
        {
          Logger.warning(ex.ToString());
        }
        return (Room) null;
      }
    }

    public List<Account> getWaitPlayers()
    {
      List<Account> accountList = new List<Account>();
      lock (this._players)
      {
        for (int index = 0; index < this._players.Count; ++index)
        {
          Account account = AccountManager.getAccount(this._players[index]._playerId, true);
          if (account != null && account._room == null && !string.IsNullOrEmpty(account.player_name))
            accountList.Add(account);
        }
      }
      return accountList;
    }

    public void SendPacketToWaitPlayers(SendPacket packet)
    {
      List<Account> waitPlayers = this.getWaitPlayers();
      if (waitPlayers.Count == 0)
        return;
      byte[] completeBytes = packet.GetCompleteBytes("Channel.SendPacketToWaitPlayers");
      for (int index = 0; index < waitPlayers.Count; ++index)
        waitPlayers[index].SendCompletePacket(completeBytes);
    }

    public bool RemovePlayer(Account p)
    {
      bool flag = false;
      try
      {
        p.channelId = -1;
        if (p.Session != null)
        {
          lock (this._players)
            flag = this._players.Remove(p.Session);
          if (flag)
            GameSync.UpdateGSCount(this.serverId);
        }
      }
      catch (Exception ex)
      {
        Logger.warning(ex.ToString());
      }
      return flag;
    }
  }
}
