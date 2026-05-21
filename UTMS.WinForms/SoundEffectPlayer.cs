using System;
using System.IO;
using System.Media;

namespace UTMS.WinForms
{
    /// <summary>
    /// Přehrává krátký WAV zvuk uložený ve složce Assets v adresáři aplikace.
    /// </summary>
    internal sealed class SoundEffectPlayer : IDisposable
    {
        private readonly SoundPlayer soundPlayer;

        /// <summary>
        /// Načte zadaný zvukový soubor ze složky Assets.
        /// </summary>
        public SoundEffectPlayer(string assetFileName)
        {
            string fileName = Path.Combine(AppContext.BaseDirectory, "Assets", assetFileName);
            if (!File.Exists(fileName))
                return;

            soundPlayer = new SoundPlayer(fileName);
            soundPlayer.Load();
        }

        /// <summary>
        /// Přehraje zvuk, pokud je soubor dostupný.
        /// </summary>
        public void Play()
        {
            soundPlayer?.Play();
        }

        /// <summary>
        /// Uvolní přehrávač zvuku.
        /// </summary>
        public void Dispose()
        {
            soundPlayer?.Dispose();
        }
    }
}
