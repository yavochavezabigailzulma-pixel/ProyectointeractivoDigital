using System;

/// <summary>
/// Cualquier hint controlado por HintSequencer debe implementar esto.
/// El hint no decide cuándo ocultarse: solo sabe CÓMO hacerlo (con su
/// animación de fade propia) cuando el Sequencer se lo ordena.
/// </summary>
public interface IHintAnimado
{
    void Ocultar(Action alTerminar);
}