using Microsoft.Win32.SafeHandles;
using PointBlank.Auth.Data.Configs;
using PointBlank.Auth.Data.Model;
using PointBlank.Auth.Data.Sync;
using PointBlank.Auth.Data.Sync.Server;
using PointBlank.Auth.Network;
using PointBlank.Auth.Network.ClientPacket;
using PointBlank.Auth.Network.ServerPacket;
using PointBlank.Core;
using PointBlank.Core.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace PointBlank.Auth
{
  public class AuthClient : IDisposable
  {
    private SafeHandle handle = (SafeHandle) new SafeFileHandle(IntPtr.Zero, true);
    public Socket _client;
    public Account _player;
    public DateTime ConnectDate;
    public uint SessionId;
    public ushort SessionSeed;
    public int Shift;
    public int firstPacketId;
    private byte[] lastCompleteBuffer;
    private bool disposed;
    private bool closed;

    public void Dispose()
    {
      try
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }
      catch
      {
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      try
      {
        if (this.disposed)
          return;
        this._player = (Account) null;
        if (this._client != null)
        {
          this._client.Dispose();
          this._client = (Socket) null;
        }
        if (disposing)
          this.handle.Dispose();
        this.disposed = true;
      }
      catch
      {
      }
    }

    public AuthClient(Socket client)
    {
      this._client = client;
      this._client.NoDelay = true;
      this.SessionSeed = IdFactory.GetInstance().NextSeed();
    }

    public void Start()
    {
      this.Shift = (int) (this.SessionId % 7U) + 1;
      new Thread(new ThreadStart(this.Connect)).Start();
      new Thread(new ThreadStart(this.Read)).Start();
      this.ConnectDate = DateTime.Now;
    }

    private void ConnectionCheck()
    {
      Thread.Sleep(10000);
      if (this._client != null)
        return;
      Logger.warning("Connection destroyed due to socket null.");
      AuthManager.RemoveSocket(this);
      this.Dispose();
    }

    public string GetIPAddress()
    {
      try
      {
        if (this._client != null && this._client.RemoteEndPoint != null)
          return ((IPEndPoint) this._client.RemoteEndPoint).Address.ToString();
        return "";
      }
      catch
      {
        return "";
      }
    }

    public IPAddress GetAddress()
    {
      try
      {
        if (this._client != null && this._client.RemoteEndPoint != null)
          return ((IPEndPoint) this._client.RemoteEndPoint).Address;
        return (IPAddress) null;
      }
      catch
      {
        return (IPAddress) null;
      }
    }

    private void Connect()
    {
      this.SendPacket((SendPacket) new PROTOCOL_BASE_CONNECT_ACK(this));
    }

    public void SendCompletePacket(byte[] data)
    {
      try
      {
        if (data.Length < 4)
          return;
        if (AuthConfig.debugMode)
        {
          ushort uint16 = BitConverter.ToUInt16(data, 2);
          string str1 = "";
          string str2 = BitConverter.ToString(data);
          char[] chArray = new char[5]{ '-', ',', '.', ':', '\t' };
          foreach (string str3 in str2.Split(chArray))
            str1 = str1 + " " + str3;
          Logger.debug("Opcode: [" + (object) uint16 + "]");
        }
        this._client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), (object) this._client);
      }
      catch
      {
        this.Close(0, true);
      }
    }

    public void SendPacket(byte[] data)
    {
      try
      {
        if (data.Length < 2)
          return;
        ushort uint16_1 = Convert.ToUInt16(data.Length - 2);
        List<byte> byteList = new List<byte>(data.Length + 2);
        byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(uint16_1));
        byteList.AddRange((IEnumerable<byte>) data);
        byte[] array = byteList.ToArray();
        if (AuthConfig.debugMode)
        {
          ushort uint16_2 = BitConverter.ToUInt16(data, 0);
          string str1 = "";
          string str2 = BitConverter.ToString(array);
          char[] chArray = new char[5]{ '-', ',', '.', ':', '\t' };
          foreach (string str3 in str2.Split(chArray))
            str1 = str1 + " " + str3;
          Logger.debug("Opcode: [" + (object) uint16_2 + "]");
        }
        if (array.Length != 0)
          this._client.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), (object) this._client);
        byteList.Clear();
      }
      catch
      {
        this.Close(0, true);
      }
    }

    public void SendPacket(SendPacket bp)
    {
      try
      {
        using (bp)
        {
          bp.write();
          byte[] array1 = bp.mstream.ToArray();
          if (array1.Length < 2)
            return;
          ushort uint16_1 = Convert.ToUInt16(array1.Length - 2);
          List<byte> byteList = new List<byte>(array1.Length + 2);
          byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes(uint16_1));
          byteList.AddRange((IEnumerable<byte>) array1);
          byte[] array2 = byteList.ToArray();
          if (AuthConfig.debugMode)
          {
            ushort uint16_2 = BitConverter.ToUInt16(array1, 0);
            string str1 = "";
            string str2 = BitConverter.ToString(array2);
            char[] chArray = new char[5]{ '-', ',', '.', ':', '\t' };
            foreach (string str3 in str2.Split(chArray))
              str1 = str1 + " " + str3;
            Logger.debug("Opcode: [" + (object) uint16_2 + "]");
          }
          if (array2.Length != 0)
            this._client.BeginSend(array2, 0, array2.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), (object) this._client);
          bp.mstream.Close();
          byteList.Clear();
        }
      }
      catch
      {
        this.Close(0, true);
      }
    }

    private void SendCallback(IAsyncResult ar)
    {
      try
      {
        Socket asyncState = (Socket) ar.AsyncState;
        if (asyncState == null || !asyncState.Connected)
          return;
        asyncState.EndSend(ar);
      }
      catch
      {
        this.Close(0, true);
      }
    }

    private void Read()
    {
      try
      {
        AuthClient.StateObject stateObject = new AuthClient.StateObject();
        stateObject.workSocket = this._client;
        this._client.BeginReceive(stateObject.buffer, 0, 8096, SocketFlags.None, new AsyncCallback(this.OnReceiveCallback), (object) stateObject);
      }
      catch
      {
        this.Close(0, true);
      }
    }

    public void Close(int time, bool destroyConnection)
    {
      if (this.closed)
        return;
      try
      {
        this.closed = true;
        AuthManager.RemoveSocket(this);
        Account player = this._player;
        if (destroyConnection)
        {
          if (player != null)
          {
            player.setOnlineStatus(false);
            if (player._status.serverId == (byte) 0)
              SendRefresh.RefreshAccount(player, false);
            player._status.ResetData(player.player_id);
            player.SimpleClear();
            player.updateCacheInfo();
            this._player = (Account) null;
          }
          this._client.Close(time);
          Thread.Sleep(time);
          this.Dispose();
        }
        else if (player != null)
        {
          player.SimpleClear();
          player.updateCacheInfo();
          this._player = (Account) null;
        }
        AuthSync.UpdateGSCount(0);
      }
      catch (Exception ex)
      {
        Logger.warning("AuthClient.Close " + ex.ToString());
      }
    }

    private void OnReceiveCallback(IAsyncResult ar)
    {
      AuthClient.StateObject asyncState = (AuthClient.StateObject) ar.AsyncState;
      try
      {
        int length = asyncState.workSocket.EndReceive(ar);
        if (length <= 0)
          return;
        byte[] buffer = new byte[length];
        Array.Copy((Array) asyncState.buffer, 0, (Array) buffer, 0, length);
        int FirstLength = (int) BitConverter.ToUInt16(buffer, 0) & (int) short.MaxValue;
        byte[] numArray = new byte[FirstLength + 2];
        Array.Copy((Array) buffer, 2, (Array) numArray, 0, numArray.Length);
        this.lastCompleteBuffer = buffer;
        this.RunPacket(ComDiv.Decrypt(numArray, this.Shift), numArray);
        this.CheckOut(buffer, FirstLength);
        new Thread(new ThreadStart(this.Read)).Start();
      }
      catch (ObjectDisposedException ex)
      {
        ex.ToString();
      }
      catch
      {
        this.Close(0, true);
      }
    }

    public void CheckOut(byte[] buffer, int FirstLength)
    {
      int length = buffer.Length;
      try
      {
        byte[] numArray = new byte[length - FirstLength - 4];
        Array.Copy((Array) buffer, FirstLength + 4, (Array) numArray, 0, numArray.Length);
        if (numArray.Length == 0)
          return;
        int FirstLength1 = (int) BitConverter.ToUInt16(numArray, 0) & (int) short.MaxValue;
        byte[] data = new byte[FirstLength1 + 2];
        Array.Copy((Array) numArray, 2, (Array) data, 0, data.Length);
        byte[] buff = new byte[FirstLength1 + 2];
        Array.Copy((Array) ComDiv.Decrypt(data, this.Shift), 0, (Array) buff, 0, buff.Length);
        this.RunPacket(buff, numArray);
        this.CheckOut(numArray, FirstLength1);
      }
      catch
      {
      }
    }

    private void FirstPacketCheck(ushort packetId)
    {
      if (this.firstPacketId != 0)
        return;
      this.firstPacketId = (int) packetId;
      if (packetId == (ushort) 257 || packetId == (ushort) 517)
        return;
      Logger.warning("Connection destroyed due to unknown first packet. [" + (object) packetId + "]");
      this.Close(0, true);
    }

    private void RunPacket(byte[] buff, byte[] simple)
    {
      ushort uint16 = BitConverter.ToUInt16(buff, 0);
      this.FirstPacketCheck(uint16);
      if (this.closed)
        return;
      ReceivePacket receivePacket = (ReceivePacket) null;
      switch (uint16)
      {
        case 257:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_LOGIN_REQ(this, buff);
          goto case 517;
        case 515:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_LOGOUT_REQ(this, buff);
          goto case 517;
        case 517:
          if (receivePacket == null)
            break;
          new Thread(new ThreadStart(receivePacket.run)).Start();
          break;
        case 520:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_GAMEGUARD_REQ(this, buff);
          goto case 517;
        case 522:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_GET_SYSTEM_INFO_REQ(this, buff);
          goto case 517;
        case 524:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_GET_USER_INFO_REQ(this, buff);
          goto case 517;
        case 526:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_GET_INVEN_INFO_REQ(this, buff);
          goto case 517;
        case 528:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_GET_OPTION_REQ(this, buff);
          goto case 517;
        case 530:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_OPTION_SAVE_REQ(this, buff);
          goto case 517;
        case 536:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_USER_LEAVE_REQ(this, buff);
          goto case 517;
        case 540:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_GET_CHANNELLIST_REQ(this, buff);
          goto case 517;
        case 666:
          receivePacket = (ReceivePacket) new PROTOCOL_BASE_GET_MAP_INFO_REQ(this, buff);
          goto case 517;
        case 1057:
          receivePacket = (ReceivePacket) new PROTOCOL_AUTH_GET_POINT_CASH_REQ(this, buff);
          goto case 517;
        case 5377:
          receivePacket = (ReceivePacket) new PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ(this, buff);
          goto case 517;
        default:
          Logger.error("Opcode not found: " + (object) uint16);
          goto case 517;
      }
    }

    private class StateObject
    {
      public byte[] buffer = new byte[8096];
      public Socket workSocket;
      public const int BufferSize = 8096;
    }
  }
}
