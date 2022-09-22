using PointBlank.Core.Network;

namespace PointBlank.Core.Models.Account.Players
{
  public class PlayerMissions
  {
    public byte[] list1 = new byte[40];
    public byte[] list2 = new byte[40];
    public byte[] list3 = new byte[40];
    public byte[] list4 = new byte[40];
    public int actualMission;
    public int card1;
    public int card2;
    public int card3;
    public int card4;
    public int mission1;
    public int mission2;
    public int mission3;
    public int mission4;
    public bool selectedCard;

    public PlayerMissions DeepCopy()
    {
      return (PlayerMissions) this.MemberwiseClone();
    }

    public byte[] getCurrentMissionList()
    {
      if (this.actualMission == 0)
        return this.list1;
      if (this.actualMission == 1)
        return this.list2;
      if (this.actualMission == 2)
        return this.list3;
      return this.list4;
    }

    public int getCurrentCard()
    {
      return this.getCard(this.actualMission);
    }

    public int getCard(int index)
    {
      switch (index)
      {
        case 0:
          return this.card1;
        case 1:
          return this.card2;
        case 2:
          return this.card3;
        default:
          return this.card4;
      }
    }

    public int getCurrentMissionId()
    {
      if (this.actualMission == 0)
        return this.mission1;
      if (this.actualMission == 1)
        return this.mission2;
      if (this.actualMission == 2)
        return this.mission3;
      return this.mission4;
    }

    public void UpdateSelectedCard()
    {
      if (ushort.MaxValue != ComDiv.getCardFlags(this.getCurrentMissionId(), this.getCurrentCard(), this.getCurrentMissionList()))
        return;
      this.selectedCard = true;
    }
  }
}
