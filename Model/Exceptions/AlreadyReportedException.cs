using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando un usuario intenta reportar un comentario que ya ha reportado previamente.
    /// </summary>
    [Serializable]
    public class AlreadyReportedException : Exception
    {
        public long CommentId { get; private set; }
        public long UserId { get; private set; }

        public AlreadyReportedException(long commentId, long userId)
            : base($"El usuario con ID {userId} ya ha reportado el comentario con ID {commentId}.")
        {
            CommentId = commentId;
            UserId = userId;
        }
    }
}

