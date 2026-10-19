using System.Collections.Generic;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public class Tag
    {
        public long tagId { get; set; }
        public string tagName { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public Tag()
        {
            Comments = new HashSet<Comment>();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Tag;
            if (other == null) return false;
            return tagId == other.tagId || (tagName != null && tagName == other.tagName);
        }

        public override int GetHashCode()
        {
            return tagName != null ? tagName.GetHashCode() : tagId.GetHashCode();
        }

        public override string ToString()
        {
            return $"Tag [tagId={tagId}, tagName={tagName}]";
        }
    }
}

