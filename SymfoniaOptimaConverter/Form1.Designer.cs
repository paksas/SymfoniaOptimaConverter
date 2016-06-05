namespace SymfoniaOptimaConverter
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
         System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("message", System.Windows.Forms.HorizontalAlignment.Left);
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
         this.m_progressBar = new System.Windows.Forms.ProgressBar();
         this.m_openFileDialog = new System.Windows.Forms.OpenFileDialog();
         this.m_listView = new System.Windows.Forms.ListView();
         this.Message = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.m_processedInvoiceName = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // m_progressBar
         // 
         this.m_progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.m_progressBar.Location = new System.Drawing.Point(12, 56);
         this.m_progressBar.Name = "m_progressBar";
         this.m_progressBar.Size = new System.Drawing.Size(965, 28);
         this.m_progressBar.TabIndex = 0;
         // 
         // m_openFileDialog
         // 
         this.m_openFileDialog.FileName = "m_openFileDialog";
         // 
         // m_listView
         // 
         this.m_listView.Alignment = System.Windows.Forms.ListViewAlignment.Default;
         this.m_listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.m_listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Message});
         listViewGroup1.Header = "message";
         listViewGroup1.Name = "message";
         this.m_listView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1});
         this.m_listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
         this.m_listView.Location = new System.Drawing.Point(12, 90);
         this.m_listView.MultiSelect = false;
         this.m_listView.Name = "m_listView";
         this.m_listView.ShowGroups = false;
         this.m_listView.Size = new System.Drawing.Size(965, 409);
         this.m_listView.TabIndex = 1;
         this.m_listView.UseCompatibleStateImageBehavior = false;
         this.m_listView.View = System.Windows.Forms.View.Details;
         // 
         // Message
         // 
         this.Message.Width = 1024;
         // 
         // m_processedInvoiceName
         // 
         this.m_processedInvoiceName.AutoSize = true;
         this.m_processedInvoiceName.Location = new System.Drawing.Point(13, 13);
         this.m_processedInvoiceName.Name = "m_processedInvoiceName";
         this.m_processedInvoiceName.Size = new System.Drawing.Size(35, 13);
         this.m_processedInvoiceName.TabIndex = 2;
         this.m_processedInvoiceName.Text = "label1";
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(989, 511);
         this.Controls.Add(this.m_processedInvoiceName);
         this.Controls.Add(this.m_listView);
         this.Controls.Add(this.m_progressBar);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "Form1";
         this.Text = "Symfonia -> Optima converter";
         this.Load += new System.EventHandler(this.Form1_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ProgressBar m_progressBar;
      private System.Windows.Forms.OpenFileDialog m_openFileDialog;
      private System.Windows.Forms.ListView m_listView;
      private System.Windows.Forms.Label m_processedInvoiceName;
      private System.Windows.Forms.ColumnHeader Message;
   }
}

