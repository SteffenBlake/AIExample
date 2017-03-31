using System.Collections.Generic;
using SimpleAI.Models;

namespace SimpleAI.Models
{
    public interface INerve
    {
        IList<ILink> Links { get; set; }

        bool Armed { get; set; }

        bool Fired { get; set; }

        double Output { get; }
    }
}

public class Nerve : INerve
{
    public IList<ILink> Links { get; set; }

    public bool Armed { get; set; }

    public bool Fired { get; set; }

    public double Output => 1D;

    public Nerve()
    {
        Links = new List<ILink>();
        Fired = false;
    }
}
