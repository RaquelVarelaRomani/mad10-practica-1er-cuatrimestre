using System.Data.Entity;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public partial class PracticaMADModelEntities : DbContext
    {
        public PracticaMADModelEntities(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }
    }
}

