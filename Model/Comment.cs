using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    [Table("Comment")]
    public partial class Comment
    {
        public Comment()
        {
            this.CommentReports = new HashSet<CommentReport>();
            this.Tags = new HashSet<Tag>();
        }

        [Key]
        public long commentId { get; set; }
        public long productId { get; set; }
        public long userId { get; set; }
        public string commentText { get; set; }
        public DateTime commentDate { get; set; }
        public bool isInReview { get; set; }

        public virtual Product Product { get; set; }
        public virtual UserProfile User { get; set; }
        public virtual ICollection<CommentReport> CommentReports { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        public override bool Equals(object obj)
        {
            Comment target = obj as Comment;
            if (target == null)
            {
                return false;
            }

            return (this.commentId == target.commentId)
                && (this.productId == target.productId)
                && (this.userId == target.userId)
                && (this.commentText == target.commentText)
                && (this.commentDate == target.commentDate)
                && (this.isInReview == target.isInReview);
        }

        public override int GetHashCode()
        {
            return this.commentId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ commentId = {0} | productId = {1} | userId = {2} | commentText = {3} | commentDate = {4} | isInReview = {5} ]",
                this.commentId, this.productId, this.userId, this.commentText, this.commentDate, this.isInReview);
        }
    }
}

