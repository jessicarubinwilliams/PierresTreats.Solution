using System.Collections.Generic;
using System.ComponentModel;

namespace Bakery.Models
{
  public class Flavor
  {
    public Flavor()
      {
        this.FlavorTreatJoinEntity = new HashSet<FlavorTreat>();
      }

      public int FlavorId { get; set; }

      [DisplayName("Flavor Name")]
      public string FlavorName { get; set; }
      public virtual ICollection<FlavorTreat> FlavorTreatJoinEntity { get; set; }
    }
}