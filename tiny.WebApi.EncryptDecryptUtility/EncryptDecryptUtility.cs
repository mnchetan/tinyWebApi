using System;
using System.Diagnostics;
using System.Windows.Forms;
namespace tiny.WebApi.EncryptDecryptUtility
{
    /// <summary>
    /// Class EncryptDecryptUtility.
    /// Implements the <see cref="System.Windows.Forms.Form" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    [DebuggerStepThrough]
    public partial class EncryptDecryptUtility : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptDecryptUtility"/> class.
        /// </summary>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public EncryptDecryptUtility()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Handles the Click event of the btnClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtPlainText.Text = "";
            txtKey.Text = "";
            txtOutPut.Text = "";
        }
        /// <summary>
        /// Handles the Click event of the btnDecrypt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPlainText.Text)) {
                MessageBox.Show("Please provide plain text to decrypt.");
                return;
            }
            if (string.IsNullOrEmpty(txtKey.Text)) {
                MessageBox.Show("Please provide decryption key to decrypt.");
                return;
            }
            try
            {
                txtOutPut.Text = EncryptFactory.Decrypt(txtPlainText.Text, txtKey.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Unable to decrypt the encrypted text due to some issues. {Environment.NewLine} Error : {ex.Message} {Environment.NewLine} StackTrace : {ex.StackTrace} {Environment.NewLine} Inner Exception : {ex.InnerException}");
            }
        }
        /// <summary>
        /// Handles the Click event of the btnEncrypt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPlainText.Text))
            {
                MessageBox.Show("Please provide plain text to encrypt.");
                return;
            }
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                MessageBox.Show("Please provide encryption key to encrypt.");
                return;
            }
            try
            {
                txtOutPut.Text = EncryptFactory.Encrypt(txtPlainText.Text, txtKey.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to encrypt the plain text due to some issues. {Environment.NewLine} Error : {ex.Message} {Environment.NewLine} StackTrace : {ex.StackTrace} {Environment.NewLine} Inner Exception : {ex.InnerException}");
            }
        }
    }
}
