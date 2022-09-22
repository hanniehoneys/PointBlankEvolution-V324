using PointBlank.Core.Models.Enums;

namespace PointBlank.Core.Xml
{
  public class Card
  {
    public ClassType _weaponReq;
    public MissionType _missionType;
    public int _missionId;
    public int _mapId;
    public int _weaponReqId;
    public int _missionLimit;
    public int _missionBasicId;
    public int _cardBasicId;
    public int _arrayIdx;
    public int _flag;

    public Card(int cardBasicId, int missionBasicId)
    {
      this._cardBasicId = cardBasicId;
      this._missionBasicId = missionBasicId;
      this._arrayIdx = this._cardBasicId * 4 + this._missionBasicId;
      this._flag = 15 << 4 * this._missionBasicId;
    }
  }
}
