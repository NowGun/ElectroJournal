using System;
using System.Collections.Generic;

namespace EJAdmin.DataBaseClasses
{
    public partial class Status
    {
        public Status()
        {
            Bugreporters = new HashSet<Bugreporter>();
        }

        public uint Idstatus { get; set; }
        public string StatusName { get; set; } = null!;

        public virtual ICollection<Bugreporter> Bugreporters { get; set; }
    }
}
