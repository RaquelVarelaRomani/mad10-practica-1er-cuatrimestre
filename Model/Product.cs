using System;
using System.Collections.Generic;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public class Product
    {
        public long productId { get; set; }
        public string productName { get; set; }
        public DateTime creationDate { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public long categoryId { get; set; }
        public long sellerId { get; set; }

        public virtual Category Category { get; set; }
        public virtual UserProfile Seller { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<SellerRating> SellerRatings { get; set; }

        public Product()
        {
            Comments = new HashSet<Comment>();
            SellerRatings = new HashSet<SellerRating>();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Product;
            if (other == null) return false;
            return productId == other.productId;
        }

        public override int GetHashCode()
        {
            return productId.GetHashCode();
        }

        public override string ToString()
        {
            return $"Product [productId={productId}, productName={productName}, categoryId={categoryId}, sellerId={sellerId}]";
        }
    }
}

