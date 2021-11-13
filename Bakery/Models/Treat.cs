using System.Collections.Generic;
using System.ComponentModel;

namespace Bakery.Models
{
  public class Treat
  {
    public Treat()
    {
      this.FlavorTreatJoinEntity = new HashSet<FlavorTreat>();
    }
    public int TreatId { get; set; }

    [DisplayName("Treat Name `Last, First` ")]
    public string TreatName { get; set; }
    
    public virtual ICollection<FlavorTreat> FlavorTreatJoinEntity { get;}
  }
}