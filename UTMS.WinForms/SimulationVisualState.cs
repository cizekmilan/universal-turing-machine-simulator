namespace UTMS.WinForms
{
    /// <summary>
    /// Vizuální stav simulace zobrazovaný v panelu pásky.
    /// </summary>
    internal enum SimulationVisualState
    {
        Ready,
        Running,
        Paused,
        Finished,
        NoTransition,
        Overflow,
        StepLimit
    }
}
