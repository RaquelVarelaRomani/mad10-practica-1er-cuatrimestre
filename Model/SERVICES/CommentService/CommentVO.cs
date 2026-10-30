using System;
using System.Collections.Generic;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    [Serializable]
    public class CommentVO
    {
        public long CommentId { get; private set; }
        public long UserId { get; private set; }
        public string UserName { get; private set; }
        public long ProductId { get; private set; }
        public string Text { get; private set; }
        public DateTime Date { get; private set; }
        public bool IsInReview { get; private set; }
        public ICollection<TagVO> Tags { get; private set; }

        public CommentVO(long commentId, long userId,
            string userName, long productId, string text,
            DateTime date, bool isInReview, ICollection<TagVO> tags)
        {
            this.CommentId = commentId;
            this.UserId = userId;
            this.UserName = userName;
            this.ProductId = productId;
            this.Text = text;
            this.Date = date;
            this.IsInReview = isInReview;
            this.Tags = tags ?? new List<TagVO>();
        }
    }
}
