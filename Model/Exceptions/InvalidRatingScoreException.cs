using System;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando se proporciona una puntuacion de valoracion fuera del rango permitido (1 a 5).
    /// </summary>
    [Serializable]
    public class InvalidRatingScoreException : ModelException
    {
        public int Score { get; private set; }

        public InvalidRatingScoreException(int score)
            : base(String.Format("La puntuacion {0} no es valida. Debe estar comprendida entre 1 y 5.", score))
        {
            this.Score = score;
        }
    }
}

