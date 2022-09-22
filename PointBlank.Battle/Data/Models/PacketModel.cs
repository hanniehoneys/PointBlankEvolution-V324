using System;

namespace PointBlank.Battle.Data.Models
{
  public class PacketModel
  {
    public int Opcode;
    public int Slot;
    public int Round;
    public int Length;
    public int AccountId;
    public int Unk;
    public int Respawn;
    public int RoundNumber;
    public float Time;
    public byte[] Data;
    public byte[] withEndData;
    public byte[] noEndData;
    public DateTime ReceiveDate;
  }
}
