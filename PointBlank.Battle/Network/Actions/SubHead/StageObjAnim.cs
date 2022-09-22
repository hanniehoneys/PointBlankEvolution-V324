using PointBlank.Battle.Data.Models.SubHead;

namespace PointBlank.Battle.Network.Actions.SubHead
{
  public class StageObjAnim
  {
    public static byte[] ReadInfo(ReceivePacket p)
    {
      return p.readB(9);
    }

    public static StageAnimInfo ReadInfo(ReceivePacket p, bool genLog)
    {
      StageAnimInfo stageAnimInfo = new StageAnimInfo() { _unk = p.readC(), _life = p.readUH(), _syncDate = p.readT(), _anim1 = p.readC(), _anim2 = p.readC() };
      if (genLog)
        Logger.warning("[StageObjAnim] Unk: " + (object) stageAnimInfo._unk + " Life: " + (object) stageAnimInfo._life + " Sync: " + (object) stageAnimInfo._syncDate + " Animation[1]: " + (object) stageAnimInfo._anim1 + " Animation[2]: " + (object) stageAnimInfo._anim2);
      return stageAnimInfo;
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p)
    {
      s.writeB(StageObjAnim.ReadInfo(p));
    }

    public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
    {
      StageAnimInfo stageAnimInfo = StageObjAnim.ReadInfo(p, genLog);
      s.writeC(stageAnimInfo._unk);
      s.writeH(stageAnimInfo._life);
      s.writeT(stageAnimInfo._syncDate);
      s.writeC(stageAnimInfo._anim1);
      s.writeC(stageAnimInfo._anim2);
    }
  }
}
