using System.Collections.Generic;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public class Category
    {
        public long categoryId { get; set; }
        public string categoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public Category()
        {
            Products = new HashSet<Product>();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Category;
            if (other == null) return false;
            return categoryId == other.categoryId || (categoryName != null && categoryName == other.categoryName);
        }

        public override int GetHashCode()
        {
            return categoryName != null ? categoryName.GetHashCode() : categoryId.GetHashCode();
        }

        public override string ToString()
        {
            return $"Category [categoryId={categoryId}, categoryName={categoryName}]";
        }
    }
}

