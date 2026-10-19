using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando se proporciona una puntuacion de valoracion fuera del rango permitido (1 a 5).
    /// </summary>
    [Serializable]
    public class InvalidRatingScoreException : Exception
    {
        public int Score { get; private set; }

        public InvalidRatingScoreException(int score)
            : base($"La puntuacion {score} no es valida. Debe estar comprendida entre 1 y 5.")
        {
            Score = score;
        }
    }
}

