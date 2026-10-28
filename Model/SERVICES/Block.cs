using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public class Block<T>
    {
        public List<T> Items { get; private set; }
        public bool ExistsMoreItems { get; private set; }

        public Block(List<T> items, bool existsMoreItems)
        {
            Items = items;
            ExistsMoreItems = existsMoreItems;
        }
    }
}
