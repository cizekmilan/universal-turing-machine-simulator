namespace UTMS.WinForms
{
    partial class PromptDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblValue = new System.Windows.Forms.Label();
            txtValue = new System.Windows.Forms.TextBox();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblValue
            // 
            lblValue.AutoSize = true;
            lblValue.Location = new System.Drawing.Point(12, 14);
            lblValue.Name = "lblValue";
            lblValue.Size = new System.Drawing.Size(38, 15);
            lblValue.TabIndex = 0;
            lblValue.Text = "Value:";
            // 
            // txtValue
            // 
            txtValue.Location = new System.Drawing.Point(12, 34);
            txtValue.Name = "txtValue";
            txtValue.Size = new System.Drawing.Size(296, 23);
            txtValue.TabIndex = 1;
            // 
            // btnOk
            // 
            btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOk.Location = new System.Drawing.Point(152, 72);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(75, 27);
            btnOk.TabIndex = 2;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(233, 72);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 27);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // PromptDialog
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(320, 112);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(txtValue);
            Controls.Add(lblValue);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PromptDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
