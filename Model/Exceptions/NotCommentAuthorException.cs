using System;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando un usuario intenta modificar o borrar un comentario del que no es autor.
    /// </summary>
    [Serializable]
    public class NotCommentAuthorException : ModelException
    {
        public long CommentId { get; private set; }
        public long UserId { get; private set; }

        public NotCommentAuthorException(long commentId, long userId)
            : base(String.Format("El usuario con ID {0} no es el autor del comentario con ID {1}.", userId, commentId))
        {
            this.CommentId = commentId;
            this.UserId = userId;
        }
    }
}

