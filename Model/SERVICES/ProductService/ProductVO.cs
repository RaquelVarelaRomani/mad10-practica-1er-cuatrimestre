
using System;
using System.Collections.Generic;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    [Serializable]
    public class ProductVO
    {
        public String Name { get; private set; }
        public String CategoryName { get; private set; }
        public DateTime CreationDate { get; private set; }
        public long SellerId { get; private set; }
        public String SellerName { get; private set; }

        public ProductVO(string name, string categoryName, DateTime creationDate, long sellerId, string sellerName)
        {
            this.Name = name;
            this.CategoryName = categoryName;
            this.CreationDate = creationDate;
            this.SellerId = sellerId;
            this.SellerName = sellerName;
        }

        public override bool Equals(object obj)
        {
            return obj is ProductVO details &&
                   Name == details.Name &&
                   CategoryName == details.CategoryName &&
                   CreationDate == details.CreationDate &&
                   SellerId == details.SellerId &&
                   SellerName == details.SellerName;
        }

        public override int GetHashCode()
        {
            int hashCode = -782935284;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CategoryName);
            hashCode = hashCode * -1521134295 + CreationDate.GetHashCode();
            hashCode = hashCode * -1521134295 + SellerId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SellerName);
            return hashCode;
        }
    }
}
