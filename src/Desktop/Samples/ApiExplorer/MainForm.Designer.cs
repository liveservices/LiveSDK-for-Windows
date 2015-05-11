// ------------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// ------------------------------------------------------------------------------

namespace Microsoft.Live.Desktop.Samples.ApiExplorer
{
    partial class MainForm
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
            this.signinButton = new System.Windows.Forms.Button();
            this.signOutWebBrowser = new System.Windows.Forms.WebBrowser();
            this.mePictureBox = new System.Windows.Forms.PictureBox();
            this.scopeListBox = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.currentScopeTextBox = new System.Windows.Forms.TextBox();
            this.meNameLabel = new System.Windows.Forms.Label();
            this.signOutButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.destPathLabel = new System.Windows.Forms.Label();
            this.destPathTextBox = new System.Windows.Forms.TextBox();
            this.methodComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.requestBodyTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.executeButton = new System.Windows.Forms.Button();
            this.connectGroupBox = new System.Windows.Forms.GroupBox();
            this.clearOutputButton = new System.Windows.Forms.Button();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.mePictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.connectGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // signinButton
            // 
            this.signinButton.Enabled = false;
            this.signinButton.Location = new System.Drawing.Point(247, 22);
            this.signinButton.Name = "signinButton";
            this.signinButton.Size = new System.Drawing.Size(108, 23);
            this.signinButton.TabIndex = 0;
            this.signinButton.Text = "Sign in/Authorize";
            this.signinButton.UseVisualStyleBackColor = true;
            this.signinButton.Click += new System.EventHandler(this.SigninButton_Click);
            // 
            // signOutWebBrowser
            // 
            this.signOutWebBrowser.Location = new System.Drawing.Point(647, 22);
            this.signOutWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.signOutWebBrowser.Name = "signOutWebBrowser";
            this.signOutWebBrowser.Size = new System.Drawing.Size(26, 25);
            this.signOutWebBrowser.TabIndex = 1;
            this.signOutWebBrowser.Visible = false;
            // 
            // mePictureBox
            // 
            this.mePictureBox.Location = new System.Drawing.Point(10, 43);
            this.mePictureBox.Name = "mePictureBox";
            this.mePictureBox.Size = new System.Drawing.Size(72, 72);
            this.mePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.mePictureBox.TabIndex = 3;
            this.mePictureBox.TabStop = false;
            // 
            // scopeListBox
            // 
            this.scopeListBox.FormattingEnabled = true;
            this.scopeListBox.Items.AddRange(new object[] {
            "wl.signin",
            "wl.basic",
            "onedrive.readonly",
            "onedrive.readwrite",
            "onedrive.appfolder",
            "wl.birthday",
            "wl.calendars",
            "wl.calendars_update",
            "wl.contacts_birthday",
            "wl.contacts_create",
            "wl.contacts_calendars",
            "wl.contacts_photos",
            "wl.contacts_skydrive",
            "wl.emails",
            "wl.events_create",
            "wl.offline_access",
            "wl.phone_numbers",
            "wl.photos",
            "wl.postal_addresses",
            "wl.share",
            "wl.skydrive",
            "wl.skydrive_update",
            "wl.work_profile"});
            this.scopeListBox.Location = new System.Drawing.Point(361, 22);
            this.scopeListBox.Name = "scopeListBox";
            this.scopeListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.scopeListBox.Size = new System.Drawing.Size(138, 121);
            this.scopeListBox.TabIndex = 5;
            this.scopeListBox.SelectedIndexChanged += new System.EventHandler(this.ScopeListBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.currentScopeTextBox);
            this.groupBox1.Controls.Add(this.meNameLabel);
            this.groupBox1.Controls.Add(this.mePictureBox);
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 131);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current Session";
            // 
            // currentScopeTextBox
            // 
            this.currentScopeTextBox.Location = new System.Drawing.Point(88, 43);
            this.currentScopeTextBox.Multiline = true;
            this.currentScopeTextBox.Name = "currentScopeTextBox";
            this.currentScopeTextBox.ReadOnly = true;
            this.currentScopeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.currentScopeTextBox.Size = new System.Drawing.Size(131, 72);
            this.currentScopeTextBox.TabIndex = 5;
            // 
            // meNameLabel
            // 
            this.meNameLabel.Location = new System.Drawing.Point(11, 21);
            this.meNameLabel.Name = "meNameLabel";
            this.meNameLabel.Size = new System.Drawing.Size(209, 19);
            this.meNameLabel.TabIndex = 4;
            // 
            // signOutButton
            // 
            this.signOutButton.Enabled = false;
            this.signOutButton.Location = new System.Drawing.Point(247, 51);
            this.signOutButton.Name = "signOutButton";
            this.signOutButton.Size = new System.Drawing.Size(108, 23);
            this.signOutButton.TabIndex = 7;
            this.signOutButton.Text = "Sign out";
            this.signOutButton.UseVisualStyleBackColor = true;
            this.signOutButton.Click += new System.EventHandler(this.SignOutButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Path";
            // 
            // destPathLabel
            // 
            this.destPathLabel.AutoSize = true;
            this.destPathLabel.Location = new System.Drawing.Point(25, 54);
            this.destPathLabel.Name = "destPathLabel";
            this.destPathLabel.Size = new System.Drawing.Size(57, 13);
            this.destPathLabel.TabIndex = 10;
            this.destPathLabel.Text = "Dest. Path";
            // 
            // destPathTextBox
            // 
            this.destPathTextBox.Location = new System.Drawing.Point(82, 51);
            this.destPathTextBox.Name = "destPathTextBox";
            this.destPathTextBox.Size = new System.Drawing.Size(392, 20);
            this.destPathTextBox.TabIndex = 9;
            // 
            // methodComboBox
            // 
            this.methodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.methodComboBox.FormattingEnabled = true;
            this.methodComboBox.Items.AddRange(new object[] {
            "GET",
            "PUT",
            "POST",
            "DELETE",
            "COPY",
            "MOVE",
            "UPLOAD",
            "DOWNLOAD"});
            this.methodComboBox.Location = new System.Drawing.Point(537, 25);
            this.methodComboBox.Name = "methodComboBox";
            this.methodComboBox.Size = new System.Drawing.Size(115, 21);
            this.methodComboBox.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(488, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Method";
            // 
            // requestBodyTextBox
            // 
            this.requestBodyTextBox.Location = new System.Drawing.Point(82, 77);
            this.requestBodyTextBox.Multiline = true;
            this.requestBodyTextBox.Name = "requestBodyTextBox";
            this.requestBodyTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.requestBodyTextBox.Size = new System.Drawing.Size(570, 63);
            this.requestBodyTextBox.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Request Body";
            // 
            // pathTextBox
            // 
            this.pathTextBox.Location = new System.Drawing.Point(82, 25);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(392, 20);
            this.pathTextBox.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Output";
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(82, 146);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(82, 23);
            this.executeButton.TabIndex = 17;
            this.executeButton.Text = "Execute";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.ExecuteButton_Click);
            // 
            // connectGroupBox
            // 
            this.connectGroupBox.Controls.Add(this.clearOutputButton);
            this.connectGroupBox.Controls.Add(this.outputTextBox);
            this.connectGroupBox.Controls.Add(this.label1);
            this.connectGroupBox.Controls.Add(this.label5);
            this.connectGroupBox.Controls.Add(this.executeButton);
            this.connectGroupBox.Controls.Add(this.destPathTextBox);
            this.connectGroupBox.Controls.Add(this.destPathLabel);
            this.connectGroupBox.Controls.Add(this.pathTextBox);
            this.connectGroupBox.Controls.Add(this.methodComboBox);
            this.connectGroupBox.Controls.Add(this.label4);
            this.connectGroupBox.Controls.Add(this.label3);
            this.connectGroupBox.Controls.Add(this.requestBodyTextBox);
            this.connectGroupBox.Enabled = false;
            this.connectGroupBox.Location = new System.Drawing.Point(15, 149);
            this.connectGroupBox.Name = "connectGroupBox";
            this.connectGroupBox.Size = new System.Drawing.Size(658, 394);
            this.connectGroupBox.TabIndex = 18;
            this.connectGroupBox.TabStop = false;
            // 
            // clearOutputButton
            // 
            this.clearOutputButton.Location = new System.Drawing.Point(577, 146);
            this.clearOutputButton.Name = "clearOutputButton";
            this.clearOutputButton.Size = new System.Drawing.Size(75, 23);
            this.clearOutputButton.TabIndex = 18;
            this.clearOutputButton.Text = "Clear";
            this.clearOutputButton.UseVisualStyleBackColor = true;
            this.clearOutputButton.Click += new System.EventHandler(this.ClearOutputButton_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(82, 176);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(570, 209);
            this.outputTextBox.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 555);
            this.Controls.Add(this.connectGroupBox);
            this.Controls.Add(this.signOutButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.scopeListBox);
            this.Controls.Add(this.signinButton);
            this.Controls.Add(this.signOutWebBrowser);
            this.Name = "MainForm";
            this.Text = "Live Connect API Explorer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ClientSizeChanged += new System.EventHandler(this.MainForm_ClientSizeChange);
            ((System.ComponentModel.ISupportInitialize)(this.mePictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.connectGroupBox.ResumeLayout(false);
            this.connectGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button signinButton;
        private System.Windows.Forms.WebBrowser signOutWebBrowser;
        private System.Windows.Forms.PictureBox mePictureBox;
        private System.Windows.Forms.ListBox scopeListBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label meNameLabel;
        private System.Windows.Forms.Button signOutButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label destPathLabel;
        private System.Windows.Forms.TextBox destPathTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox requestBodyTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.GroupBox connectGroupBox;
        private System.Windows.Forms.ComboBox methodComboBox;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button clearOutputButton;
        private System.Windows.Forms.TextBox currentScopeTextBox;
    }
}

