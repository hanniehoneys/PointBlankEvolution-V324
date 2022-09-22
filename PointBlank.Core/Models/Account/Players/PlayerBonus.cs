namespace PointBlank.Core.Models.Account.Players
{
  public class PlayerBonus
  {
    public int sightColor = 4;
    public int muzzle = 0;
    public int fakeRank = 55;
    public string fakeNick = "";
    public int bonuses;
    public int freepass;
    public long ownerId;

    public bool RemoveBonuses(int itemId)
    {
      int bonuses = this.bonuses;
      int freepass = this.freepass;
      switch (itemId)
      {
        case 1600001:
          this.Decrease(1);
          break;
        case 1600002:
          this.Decrease(2);
          break;
        case 1600003:
          this.Decrease(4);
          break;
        case 1600004:
          this.Decrease(32);
          break;
        case 1600011:
          this.freepass = 0;
          break;
        case 1600037:
          this.Decrease(8);
          break;
        case 1600038:
          this.Decrease(128);
          break;
        case 1600119:
          this.Decrease(64);
          break;
      }
      return this.bonuses != bonuses || this.freepass != freepass;
    }

    public bool AddBonuses(int itemId)
    {
      int bonuses = this.bonuses;
      int freepass = this.freepass;
      switch (itemId)
      {
        case 1600001:
          this.Increase(1);
          break;
        case 1600002:
          this.Increase(2);
          break;
        case 1600003:
          this.Increase(4);
          break;
        case 1600004:
          this.Increase(32);
          break;
        case 1600011:
          this.freepass = 1;
          break;
        case 1600037:
          this.Increase(8);
          break;
        case 1600038:
          this.Increase(128);
          break;
        case 1600119:
          this.Increase(64);
          break;
      }
      return this.bonuses != bonuses || this.freepass != freepass;
    }

    private void Decrease(int value)
    {
      this.bonuses &= ~value;
    }

    private void Increase(int value)
    {
      this.bonuses |= value;
    }
  }
}
