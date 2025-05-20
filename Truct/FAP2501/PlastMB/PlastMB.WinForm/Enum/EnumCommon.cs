using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.Enum
{
    public enum Status
    {
        [Description("Invalid flg")]
        Invalid = 0,
        [Description("Valid flg")]
        Valid = 1,
    }
}
