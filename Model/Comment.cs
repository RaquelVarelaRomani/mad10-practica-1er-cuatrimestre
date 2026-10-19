using System;
using System.Collections.Generic;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public class Comment
    {
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

        public Comment()
        {
            CommentReports = new HashSet<CommentReport>();
            Tags = new HashSet<Tag>();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Comment;
            if (other == null) return false;
            return commentId == other.commentId;
        }

        public override int GetHashCode()
        {
            return commentId.GetHashCode();
        }

        public override string ToString()
        {
            return $"Comment [commentId={commentId}, productId={productId}, userId={userId}, isInReview={isInReview}, commentDate={commentDate}]";
        }
    }
}

