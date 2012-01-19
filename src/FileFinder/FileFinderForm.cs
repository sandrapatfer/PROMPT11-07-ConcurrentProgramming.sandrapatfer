using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace FileFinder
{
    public partial class FileFinderForm : Form
    {
        private bool _canceled;
        private int _count = 0;
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
            btnCancel.Enabled = true;
            txtResults.Text = "";
            _canceled = false;
/*            Thread t = new Thread(new ThreadStart(() =>
            {
                foreach (var fname in Directory.EnumerateFiles(fldr, patt, SearchOption.AllDirectories))
                {
                    if (_canceled)
                    {
                        break;
                    }
                    else
                    {
                        String name = fname;
                        String content = File.ReadAllText(name);
                        if (content.Contains(word))
                            txtResults.AddLine(name);
                    }
                }
            }));
            t.Start();*/

/*            ThreadPool.QueueUserWorkItem( new WaitCallback( (o)=>
            {
                foreach (var fname in Directory.EnumerateFiles(fldr, patt, SearchOption.AllDirectories))
                {
                    if (_canceled)
                    {
                        break;
                    }
                    else
                    {
                        String name = fname;
                        ThreadPool.QueueUserWorkItem(new WaitCallback((o2) =>
                            {
                                if (!_canceled)
                                {
                                    String content = File.ReadAllText(name);
                                    if (content.Contains(word))
                                        txtResults.AddLine(name);
                                }
                            }));
                    }
                }
            }));*/

/*            _count = 0;
            int count_active_work_items = 1;
            ThreadPool.QueueUserWorkItem( (o1)=>
            {
                foreach (var dname in Directory.EnumerateDirectories(fldr, "*", SearchOption.AllDirectories))
                {
                    if (!_canceled)
                    {
                        Interlocked.Increment(ref count_active_work_items);
                        ThreadPool.QueueUserWorkItem((o2) =>
                            {
                                if (!_canceled)
                                {
                                    foreach (var fname in Directory.EnumerateFiles(dname, patt))
                                    {
                                        if (_canceled)
                                        {
                                            break;
                                        }
                                        String name = fname;
                                        Interlocked.Increment(ref _count);
                                        String content = File.ReadAllText(name);
                                        if (content.Contains(word))
                                            txtResults.AddLine(name);
                                    }
                                    if (Interlocked.Decrement(ref count_active_work_items) == 0)
                                    {
                                        this.BeginInvoke(new Action(() =>
                                        {
                                            btnCancel.Enabled = false;
                                            btnSearch.Enabled = true;
                                            MessageBox.Show(string.Format("Finished", _count));
                                        }));
                                    }
                                }
                            });
                    }
                }
                if (Interlocked.Decrement(ref count_active_work_items) == 0)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        btnCancel.Enabled = false;
                        btnSearch.Enabled = true;
                        MessageBox.Show(string.Format("Finished", _count));
                    }));
                }
            });*/

            _count = 0;
            var itemsCounter = new ActiveWorkItemCounter(new Action(() =>
                                        {
                                            this.BeginInvoke(new Action(() =>
                                                {
                                                    btnCancel.Enabled = false;
                                                    btnSearch.Enabled = true;
                                                    MessageBox.Show(string.Format("Finished", _count));
                                                }));
                                        }));
            ThreadPool.QueueUserWorkItem( (o1)=>
            {
                foreach (var dname in Directory.EnumerateDirectories(fldr, "*", SearchOption.AllDirectories))
                {
                    if (!_canceled)
                    {
                        itemsCounter.Increment();
                        ThreadPool.QueueUserWorkItem((o2) =>
                            {
                                if (!_canceled)
                                {
                                    foreach (var fname in Directory.EnumerateFiles(dname, patt))
                                    {
                                        if (_canceled)
                                        {
                                            break;
                                        }
                                        String name = fname;
                                        Interlocked.Increment(ref _count);
                                        String content = File.ReadAllText(name);
                                        if (content.Contains(word))
                                            txtResults.AddLine(name);
                                    }
                                    itemsCounter.Decrement();
                                }
                            });
                    }
                }
                itemsCounter.Decrement();
            });
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _canceled = true;
            btnCancel.Enabled = false;
            btnSearch.Enabled = true;
            MessageBox.Show(string.Format("Processed {0} files", _count));
        }
    }

    static class TextBoxExtensions
    {
        public static TextBox AddLine(this TextBox textBox, String text)
        {
            textBox.BeginInvoke(new Action(() =>
                {
                    textBox.AppendText(text);
                    textBox.AppendText("\r\n");
                }));
            return textBox;
        }
    }
}
