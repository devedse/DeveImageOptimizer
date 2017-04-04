using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DeveImageOptimizer.State
{
    public interface IChangable
    {
        bool IsChanged { get; set; }
    }
}
