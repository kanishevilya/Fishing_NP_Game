namespace Game {
	partial class FormGame {
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
			this.components = new System.ComponentModel.Container();
			this.gameTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// gameTimer
			// 
			this.gameTimer.Interval = 20;
			this.gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
			// 
			// FormGame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1184, 862);
			this.MaximumSize = new System.Drawing.Size(1200, 1000);
			this.MinimumSize = new System.Drawing.Size(1200, 851);
			this.Name = "FormGame";
			this.Text = "Game";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGame_FormClosing);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormGame_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormGame_KeyUp);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer gameTimer;
	}
}

