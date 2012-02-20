namespace MoviesInfo
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.photosPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.movieIn = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.titleBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.yearBox = new System.Windows.Forms.TextBox();
            this.directorBox = new System.Windows.Forms.TextBox();
            this.posterBox = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.plotBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.langIn = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.yearIn = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.reviewsList = new System.Windows.Forms.ListView();
            this.Reviewer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Resume = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Url = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.posterBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.photosPanel);
            this.groupBox1.Location = new System.Drawing.Point(5, 231);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(822, 157);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "photos";
            // 
            // photosPanel
            // 
            this.photosPanel.AutoScroll = true;
            this.photosPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.photosPanel.Location = new System.Drawing.Point(2, 15);
            this.photosPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.photosPanel.Name = "photosPanel";
            this.photosPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.photosPanel.Size = new System.Drawing.Size(818, 140);
            this.photosPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "movie";
            // 
            // movieIn
            // 
            this.movieIn.Location = new System.Drawing.Point(44, 11);
            this.movieIn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.movieIn.Name = "movieIn";
            this.movieIn.Size = new System.Drawing.Size(475, 20);
            this.movieIn.TabIndex = 1;
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(685, 12);
            this.searchButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(59, 20);
            this.searchButton.TabIndex = 0;
            this.searchButton.TabStop = false;
            this.searchButton.Text = "search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Enabled = false;
            this.cancelButton.Location = new System.Drawing.Point(758, 11);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(59, 20);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 17);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "title";
            // 
            // titleBox
            // 
            this.titleBox.Location = new System.Drawing.Point(44, 13);
            this.titleBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.titleBox.Name = "titleBox";
            this.titleBox.Size = new System.Drawing.Size(623, 20);
            this.titleBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 41);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "director";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 63);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "year";
            // 
            // yearBox
            // 
            this.yearBox.Location = new System.Drawing.Point(44, 58);
            this.yearBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.yearBox.Name = "yearBox";
            this.yearBox.Size = new System.Drawing.Size(86, 20);
            this.yearBox.TabIndex = 6;
            // 
            // directorBox
            // 
            this.directorBox.Location = new System.Drawing.Point(44, 36);
            this.directorBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.directorBox.Name = "directorBox";
            this.directorBox.Size = new System.Drawing.Size(623, 20);
            this.directorBox.TabIndex = 5;
            // 
            // posterBox
            // 
            this.posterBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.posterBox.Location = new System.Drawing.Point(684, 11);
            this.posterBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.posterBox.Name = "posterBox";
            this.posterBox.Size = new System.Drawing.Size(132, 158);
            this.posterBox.TabIndex = 11;
            this.posterBox.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 81);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "plot";
            // 
            // plotBox
            // 
            this.plotBox.Location = new System.Drawing.Point(44, 80);
            this.plotBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.plotBox.Multiline = true;
            this.plotBox.Name = "plotBox";
            this.plotBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.plotBox.Size = new System.Drawing.Size(623, 88);
            this.plotBox.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.langIn);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.yearIn);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.cancelButton);
            this.groupBox2.Controls.Add(this.searchButton);
            this.groupBox2.Controls.Add(this.movieIn);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(4, 2);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(823, 44);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            // 
            // langIn
            // 
            this.langIn.Location = new System.Drawing.Point(638, 12);
            this.langIn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.langIn.Name = "langIn";
            this.langIn.Size = new System.Drawing.Size(29, 20);
            this.langIn.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(613, 14);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "lang";
            // 
            // yearIn
            // 
            this.yearIn.Location = new System.Drawing.Point(554, 13);
            this.yearIn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.yearIn.Name = "yearIn";
            this.yearIn.Size = new System.Drawing.Size(43, 20);
            this.yearIn.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(529, 13);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "year";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.reviewsList);
            this.groupBox3.Location = new System.Drawing.Point(5, 392);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Size = new System.Drawing.Size(822, 108);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "reviews";
            // 
            // reviewsList
            // 
            this.reviewsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Reviewer,
            this.Resume,
            this.Url});
            this.reviewsList.Location = new System.Drawing.Point(5, 13);
            this.reviewsList.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.reviewsList.Name = "reviewsList";
            this.reviewsList.Size = new System.Drawing.Size(816, 89);
            this.reviewsList.TabIndex = 0;
            this.reviewsList.UseCompatibleStateImageBehavior = false;
            this.reviewsList.View = System.Windows.Forms.View.Details;
            // 
            // Reviewer
            // 
            this.Reviewer.Text = "Reviewer";
            this.Reviewer.Width = 120;
            // 
            // Resume
            // 
            this.Resume.Width = 500;
            // 
            // Url
            // 
            this.Url.Text = "Url";
            this.Url.Width = 500;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.directorBox);
            this.groupBox4.Controls.Add(this.yearBox);
            this.groupBox4.Controls.Add(this.plotBox);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.titleBox);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.posterBox);
            this.groupBox4.Location = new System.Drawing.Point(5, 51);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Size = new System.Drawing.Size(822, 175);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 510);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximumSize = new System.Drawing.Size(852, 548);
            this.MinimumSize = new System.Drawing.Size(852, 548);
            this.Name = "Form1";
            this.Text = "Movies Info";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.posterBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox movieIn;
		private System.Windows.Forms.Button searchButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox titleBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox yearBox;
		private System.Windows.Forms.TextBox directorBox;
		private System.Windows.Forms.PictureBox posterBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox plotBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Panel photosPanel;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox yearIn;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox langIn;
		private System.Windows.Forms.ListView reviewsList;
		private System.Windows.Forms.ColumnHeader Reviewer;
		private System.Windows.Forms.ColumnHeader Resume;
		private System.Windows.Forms.ColumnHeader Url;
	}
}

