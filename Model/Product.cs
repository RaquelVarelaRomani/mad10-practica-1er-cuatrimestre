using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            this.Comments = new HashSet<Comment>();
            this.SellerRatings = new HashSet<SellerRating>();
        }

        [Key]
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

        public override bool Equals(object obj)
        {
            Product target = obj as Product;
            if (target == null)
            {
                return false;
            }

            return (this.productId == target.productId)
                && (this.productName == target.productName)
                && (this.creationDate == target.creationDate)
                && (this.description == target.description)
                && (this.image == target.image)
                && (this.categoryId == target.categoryId)
                && (this.sellerId == target.sellerId);
        }

        public override int GetHashCode()
        {
            return this.productId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ productId = {0} | productName = {1} | creationDate = {2} | description = {3} | image = {4} | categoryId = {5} | sellerId = {6} ]",
                this.productId, this.productName, this.creationDate, this.description, this.image, this.categoryId, this.sellerId);
        }
    }
}

