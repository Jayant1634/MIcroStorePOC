namespace Free_MS_Store_Apps
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            label1 = new Label();
            button1 = new Button();
            progressBar1 = new ProgressBar();
            label2 = new Label();
            listView1 = new ListView();
            label3 = new Label();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(65, 82);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(328, 25);
            textBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            label1.Location = new Point(65, 35);
            label1.Name = "label1";
            label1.Size = new Size(332, 30);
            label1.TabIndex = 1;
            label1.Text = "Enter the url of the ms store app";
            label1.Click += label1_Click;
            // 
            // button1
            // 
            button1.Location = new Point(76, 138);
            button1.Name = "button1";
            button1.Size = new Size(190, 32);
            button1.TabIndex = 2;
            button1.Text = "Fetch Packages For This..";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(707, 47);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(264, 26);
            progressBar1.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            label2.Location = new Point(700, 10);
            label2.Name = "label2";
            label2.Size = new Size(103, 30);
            label2.TabIndex = 4;
            label2.Text = "Progress:";
            label2.Click += label2_Click;
            // 
            // listView1
            // 
            listView1.Location = new Point(65, 199);
            listView1.Name = "listView1";
            listView1.Size = new Size(899, 407);
            listView1.TabIndex = 5;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.DoubleClick += listView1_DblClickChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            label3.Location = new Point(700, 71);
            label3.Name = "label3";
            label3.Size = new Size(0, 30);
            label3.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(993, 638);
            Controls.Add(label3);
            Controls.Add(listView1);
            Controls.Add(label2);
            Controls.Add(progressBar1);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Label label1;
        private Button button1;
        private ProgressBar progressBar1;
        private Label label2;
        private ListBox listBox1;
        private ListView listView1;
        private Label label3;
    }
}