using System.Collections.Generic;
using System.Linq;
using SimpleAI.Models;

namespace SimpleAI.Nerve_Models
{
    public interface ILink
    {
        IList<INerve> Nerves { get; set; }

        double Weight { get; set; }

        double Threshould { get; set; }

        double Value { get;}

        bool CanFire { get; }
    }

    public class Link : ILink
    {
        public IList<INerve> Nerves { get; set; }

        public double Weight { get; set; }

        public double Threshould { get; set; }

        public bool CanFire => Nerves.Any(n => !n.Fired) && Nerves.Any(n => n.Armed);

        public double Value => CanFire && Nerves.Single(n => n.Armed).Output * Weight >= Threshould ? Nerves.Single(n => n.Armed).Output * Weight : 0D;

        public Link()
        {
            Weight = 1D;
            Threshould = 0D;
        }

    }
}
