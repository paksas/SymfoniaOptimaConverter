namespace SymfoniaOptimaConverter.Dialogs
{
   partial class DBConnectionDialog
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
         this.okButton = new System.Windows.Forms.Button();
         this.Cancel = new System.Windows.Forms.Button();
         this.dbName = new System.Windows.Forms.ComboBox();
         this.userName = new System.Windows.Forms.TextBox();
         this.password = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.companyName = new System.Windows.Forms.ComboBox();
         this.label5 = new System.Windows.Forms.Label();
         this.contractorCategory = new System.Windows.Forms.ComboBox();
         this.SuspendLayout();
         // 
         // okButton
         // 
         this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.okButton.Location = new System.Drawing.Point(202, 157);
         this.okButton.Name = "okButton";
         this.okButton.Size = new System.Drawing.Size(75, 23);
         this.okButton.TabIndex = 0;
         this.okButton.Text = "Connect";
         this.okButton.UseVisualStyleBackColor = true;
         this.okButton.Click += new System.EventHandler(this.OK_Click);
         // 
         // Cancel
         // 
         this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.Cancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.Cancel.Location = new System.Drawing.Point(297, 157);
         this.Cancel.Name = "Cancel";
         this.Cancel.Size = new System.Drawing.Size(75, 23);
         this.Cancel.TabIndex = 1;
         this.Cancel.Text = "Cancel";
         this.Cancel.UseVisualStyleBackColor = true;
         // 
         // dbName
         // 
         this.dbName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.dbName.FormattingEnabled = true;
         this.dbName.Location = new System.Drawing.Point(98, 13);
         this.dbName.Name = "dbName";
         this.dbName.Size = new System.Drawing.Size(273, 21);
         this.dbName.TabIndex = 2;
         // 
         // userName
         // 
         this.userName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.userName.Location = new System.Drawing.Point(98, 41);
         this.userName.Name = "userName";
         this.userName.Size = new System.Drawing.Size(272, 20);
         this.userName.TabIndex = 3;
         // 
         // password
         // 
         this.password.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.password.Location = new System.Drawing.Point(98, 67);
         this.password.Name = "password";
         this.password.PasswordChar = '*';
         this.password.Size = new System.Drawing.Size(272, 20);
         this.password.TabIndex = 4;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(12, 19);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(56, 13);
         this.label1.TabIndex = 5;
         this.label1.Text = "Database:";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(12, 46);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(32, 13);
         this.label2.TabIndex = 6;
         this.label2.Text = "User:";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(12, 71);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(53, 13);
         this.label3.TabIndex = 7;
         this.label3.Text = "Password";
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(12, 100);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(54, 13);
         this.label4.TabIndex = 9;
         this.label4.Text = "Company:";
         // 
         // companyName
         // 
         this.companyName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.companyName.FormattingEnabled = true;
         this.companyName.Location = new System.Drawing.Point(98, 94);
         this.companyName.Name = "companyName";
         this.companyName.Size = new System.Drawing.Size(273, 21);
         this.companyName.TabIndex = 8;
         this.companyName.SelectedIndexChanged += new System.EventHandler(this.companyName_SelectedIndexChanged);
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(13, 127);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(52, 13);
         this.label5.TabIndex = 11;
         this.label5.Text = "Category:";
         // 
         // contractorCategory
         // 
         this.contractorCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.contractorCategory.FormattingEnabled = true;
         this.contractorCategory.Location = new System.Drawing.Point(99, 121);
         this.contractorCategory.Name = "contractorCategory";
         this.contractorCategory.Size = new System.Drawing.Size(273, 21);
         this.contractorCategory.TabIndex = 10;
         // 
         // DBConnectionDialog
         // 
         this.AcceptButton = this.okButton;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.AutoScroll = true;
         this.AutoSize = true;
         this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.CancelButton = this.Cancel;
         this.ClientSize = new System.Drawing.Size(384, 214);
         this.ControlBox = false;
         this.Controls.Add(this.label5);
         this.Controls.Add(this.contractorCategory);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.companyName);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.password);
         this.Controls.Add(this.userName);
         this.Controls.Add(this.dbName);
         this.Controls.Add(this.Cancel);
         this.Controls.Add(this.okButton);
         this.MinimumSize = new System.Drawing.Size(400, 230);
         this.Name = "DBConnectionDialog";
         this.Text = "DBConnectionDialog";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      public System.Windows.Forms.Button okButton;
      private System.Windows.Forms.Button Cancel;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      public System.Windows.Forms.ComboBox dbName;
      public System.Windows.Forms.TextBox userName;
      public System.Windows.Forms.TextBox password;
      private System.Windows.Forms.Label label4;
      public System.Windows.Forms.ComboBox companyName;
      private System.Windows.Forms.Label label5;
      public System.Windows.Forms.ComboBox contractorCategory;
   }
}