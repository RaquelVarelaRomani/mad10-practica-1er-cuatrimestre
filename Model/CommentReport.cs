using System;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public class CommentReport
    {
        public long reportId { get; set; }
        public long commentId { get; set; }
        public long userId { get; set; }
        public string reason { get; set; }
        public DateTime reportDate { get; set; }

        public virtual Comment Comment { get; set; }
        public virtual UserProfile User { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CommentReport;
            if (other == null) return false;
            return reportId == other.reportId || (commentId == other.commentId && userId == other.userId);
        }

        public override int GetHashCode()
        {
            return reportId != 0 ? reportId.GetHashCode() : (commentId.GetHashCode() ^ userId.GetHashCode());
        }

        public override string ToString()
        {
            return $"CommentReport [reportId={reportId}, commentId={commentId}, userId={userId}, reason={reason}]";
        }
    }
}

