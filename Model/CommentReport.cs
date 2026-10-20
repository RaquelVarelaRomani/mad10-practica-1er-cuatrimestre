using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    [Table("CommentReport")]
    public partial class CommentReport
    {
        [Key]
        public long reportId { get; set; }
        public long commentId { get; set; }
        public long userId { get; set; }
        public string reason { get; set; }
        public DateTime reportDate { get; set; }

        public virtual Comment Comment { get; set; }
        public virtual UserProfile User { get; set; }

        public override bool Equals(object obj)
        {
            CommentReport target = obj as CommentReport;
            if (target == null)
            {
                return false;
            }

            return (this.reportId == target.reportId)
                && (this.commentId == target.commentId)
                && (this.userId == target.userId)
                && (this.reason == target.reason)
                && (this.reportDate == target.reportDate);
        }

        public override int GetHashCode()
        {
            return this.reportId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ reportId = {0} | commentId = {1} | userId = {2} | reason = {3} | reportDate = {4} ]",
                this.reportId, this.commentId, this.userId, this.reason, this.reportDate);
        }
    }
}

