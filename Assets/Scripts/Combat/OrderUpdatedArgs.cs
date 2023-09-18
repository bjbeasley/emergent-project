using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OrderUpdatedArgs
{
    public IEnumerable<Agent> Order { get; }
    public IEnumerable<Agent> LastOrder { get; }

    public OrderUpdatedArgs (IEnumerable<Agent> order, IEnumerable<Agent> lastOrder)
    {
        Order = order;
        LastOrder = lastOrder;
    }
}

