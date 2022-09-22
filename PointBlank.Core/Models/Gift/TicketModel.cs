using PointBlank.Core.Models.Enums;

namespace PointBlank.Core.Models.Gift
{
  public class TicketModel
  {
    public string Ticket;
    public int ItemId;
    public int Count;
    public int Equip;
    public int Point;
    public int Cash;
    public TicketType Type;

    public TicketModel(TicketType Type, string Ticket)
    {
      this.Type = Type;
      this.Ticket = Ticket;
    }
  }
}
