namespace CS2Coach
{
    partial class CS2Coach
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
            button1 = new Button();
            button2 = new Button();
            richTextBox2 = new RichTextBox();
            richTextBox1 = new RichTextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 229);
            button1.Name = "button1";
            button1.Size = new Size(164, 23);
            button1.TabIndex = 1;
            button1.Text = "Start Capture";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(182, 229);
            button2.Name = "button2";
            button2.Size = new Size(147, 23);
            button2.TabIndex = 2;
            button2.Text = "End Capture";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // richTextBox2
            // 
            richTextBox2.Location = new Point(335, 230);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(171, 25);
            richTextBox2.TabIndex = 3;
            richTextBox2.Text = "Not running";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(12, 12);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(677, 211);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
            // 
            // CS2Coach
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(704, 259);
            Controls.Add(richTextBox1);
            Controls.Add(richTextBox2);
            Controls.Add(button2);
            Controls.Add(button1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "CS2Coach";
            Text = "CS2Coach";
            TopMost = true;
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private Button button2;
        private RichTextBox richTextBox2;
        private RichTextBox richTextBox1;
    }
}
