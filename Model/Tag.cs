using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    [Table("Tag")]
    public partial class Tag
    {
        public Tag()
        {
            this.Comments = new HashSet<Comment>();
        }

        [Key]
        public long tagId { get; set; }
        public string tagName { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public override bool Equals(object obj)
        {
            Tag target = obj as Tag;
            if (target == null)
            {
                return false;
            }

            return (this.tagId == target.tagId)
                && (this.tagName == target.tagName);
        }

        public override int GetHashCode()
        {
            return this.tagId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ tagId = {0} | tagName = {1} ]",
                this.tagId, this.tagName);
        }
    }
}
