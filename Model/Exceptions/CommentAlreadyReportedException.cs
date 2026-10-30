using System;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando un usuario intenta reportar un comentario que ya ha reportado previamente.
    /// </summary>
    [Serializable]
    public class CommentAlreadyReportedException : ModelException
    {
        public long CommentId { get; private set; }
        public long UserId { get; private set; }

        public CommentAlreadyReportedException(long commentId, long userId)
            : base(String.Format("El usuario con ID {0} ya ha reportado el comentario con ID {1}.", userId, commentId))
        {
            this.CommentId = commentId;
            this.UserId = userId;
        }
    }
}

