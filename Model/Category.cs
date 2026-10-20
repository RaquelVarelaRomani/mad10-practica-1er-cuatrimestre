using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    [Table("Category")]
    public partial class Category
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
        }

        [Key]
        public long categoryId { get; set; }
        public string categoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public override bool Equals(object obj)
        {
            Category target = obj as Category;
            if (target == null)
            {
                return false;
            }

            return (this.categoryId == target.categoryId)
                && (this.categoryName == target.categoryName);
        }

        public override int GetHashCode()
        {
            return this.categoryId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ categoryId = {0} | categoryName = {1} ]",
                this.categoryId, this.categoryName);
        }
    }
}

