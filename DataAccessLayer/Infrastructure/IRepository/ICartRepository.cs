using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        int IncrementCartItem(Cart cart, int count);
    }
}
