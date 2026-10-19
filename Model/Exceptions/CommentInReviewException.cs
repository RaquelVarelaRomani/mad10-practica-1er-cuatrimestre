using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando se intenta modificar un comentario que se encuentra en revision.
    /// </summary>
    [Serializable]
    public class CommentInReviewException : Exception
    {
        public long CommentId { get; private set; }

        public CommentInReviewException(long commentId)
            : base($"El comentario con ID {commentId} esta en revision y no puede ser modificado.")
        {
            CommentId = commentId;
        }
    }
}

