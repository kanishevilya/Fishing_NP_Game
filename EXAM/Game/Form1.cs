using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game {
	public partial class FormMenu : Form {
		private FormGame formGame;

		public FormMenu() {
			InitializeComponent();
			FormManager.MenuFormInstance = this;
			formGame = new FormGame();
		}


		private void btnConnect_Click(object sender, EventArgs e) {
            if (txtName.Text == "") { return; }
			MessageBox.Show("Добро пожаловать в \"Просто рыбалку\"\r\nПравила просты:\r\n1)Подключаешься к игре\r\n2)Идешь к ближайшей реке\r\n3)Нажимаешь X, и пытайся попасть по \"Шанс\" полю (Нажать Z)\r\n4)Заходи в магазин (I) и продавай рыбки\r\n5)Покупай новые наживки и снасти, они облегчат тебе жизнь\r\n6)Удачной игры!", "START");
			formGame.Show();
			formGame.Start(txtName.Text);
			this.Hide();
		}

		private void btnExit_Click(object sender, EventArgs e) {
			this.Close();
		}
	}
}
