using System;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando se intenta modificar un comentario que se encuentra en revision.
    /// </summary>
    [Serializable]
    public class CommentInReviewException : ModelException
    {
        public long CommentId { get; private set; }

        public CommentInReviewException(long commentId)
            : base(String.Format("El comentario con ID {0} esta en revision y no puede ser modificado.", commentId))
        {
            this.CommentId = commentId;
        }
    }
}

