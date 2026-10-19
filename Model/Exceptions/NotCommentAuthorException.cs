using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando un usuario intenta modificar o borrar un comentario del que no es autor.
    /// </summary>
    [Serializable]
    public class NotCommentAuthorException : Exception
    {
        public long CommentId { get; private set; }
        public long UserId { get; private set; }

        public NotCommentAuthorException(long commentId, long userId)
            : base($"El usuario con ID {userId} no es el autor del comentario con ID {commentId}.")
        {
            CommentId = commentId;
            UserId = userId;
        }
    }
}

