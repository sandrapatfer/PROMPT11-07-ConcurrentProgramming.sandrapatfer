using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FileFinder
{
    public partial class FileFinderForm : Form
    {
        public FileFinderForm()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            String fldr = txtFolder.Text;
            if (String.IsNullOrWhiteSpace(fldr) || !Directory.Exists(fldr))
            {
                MessageBox.Show("Please indicate a valid base folder");
                return;
            }

            String patt = txtPattern.Text;
            if (String.IsNullOrWhiteSpace(patt))
            {
                MessageBox.Show("Please indicate a valid pattern");
                return;
            }

            String word = txtWord.Text;
            if (String.IsNullOrWhiteSpace(word))
            {
                MessageBox.Show("Please indicate a word");
                return;
            }

            btnSearch.Enabled = false;
            txtResults.Text = "";
            foreach (var fname in Directory.EnumerateFiles(fldr, patt, SearchOption.AllDirectories))
            {
                String name = fname;
                String content = File.ReadAllText(name);
                if (content.Contains(word))
                    txtResults.AddLine(name);
            }
            btnSearch.Enabled = true;
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select Folder";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = folderDialog.SelectedPath;
            }
 
        }
    }

    static class TextBoxExtensions
    {
        public static TextBox AddLine(this TextBox textBox, String text)
        {
            textBox.AppendText(text);
            textBox.AppendText("\r\n");
            return textBox;
        }
    }
}
