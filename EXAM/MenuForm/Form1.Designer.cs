﻿namespace Game {
	partial class FormMenu {
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent() {
			this.btnConnect = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnConnect
			// 
			this.btnConnect.BackColor = System.Drawing.Color.SkyBlue;
			this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnConnect.ForeColor = System.Drawing.Color.Transparent;
			this.btnConnect.Location = new System.Drawing.Point(464, 265);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(420, 121);
			this.btnConnect.TabIndex = 0;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = false;
			// 
			// btnExit
			// 
			this.btnExit.BackColor = System.Drawing.Color.Pink;
			this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnExit.ForeColor = System.Drawing.Color.Transparent;
			this.btnExit.Location = new System.Drawing.Point(464, 429);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(420, 121);
			this.btnExit.TabIndex = 1;
			this.btnExit.Text = "Exit";
			this.btnExit.UseVisualStyleBackColor = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Lucida Handwriting", 69.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(71)))), ((int)(((byte)(114)))));
			this.label1.Location = new System.Drawing.Point(342, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(664, 120);
			this.label1.TabIndex = 2;
			this.label1.Text = "Just fishing";
			// 
			// FormMenu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::MenuForm.Properties.Resources.MenuBackground;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(1348, 690);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnConnect);
			this.MaximumSize = new System.Drawing.Size(1364, 729);
			this.MinimumSize = new System.Drawing.Size(1364, 729);
			this.Name = "FormMenu";
			this.Text = "Menu";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Label label1;
	}
}

