using System.Windows.Forms;

namespace UTMS.WinForms
{
    /// <summary>
    /// Jednoduchý vstupní dialog používaný editorem přechodů.
    /// </summary>
    internal sealed partial class PromptDialog : Form
    {
        /// <summary>
        /// Vytvoří modální dialog s jedním textovým vstupem a tlačítky OK/Cancel.
        /// </summary>
        private PromptDialog(string title, string label, string defaultValue)
        {
            InitializeComponent();
            Text = title;
            lblValue.Text = label;
            txtValue.Text = defaultValue;
            txtValue.SelectAll();
        }

        /// <summary>
        /// Zobrazí vstupní dialog a vrátí potvrzený text nebo prázdný řetězec při zrušení.
        /// </summary>
        public static string Show(IWin32Window owner, string title, string label, string defaultValue)
        {
            using (PromptDialog dialog = new PromptDialog(title, label, defaultValue))
            {
                return dialog.ShowDialog(owner) == DialogResult.OK ? dialog.txtValue.Text : "";
            }
        }
    }
}
