using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Game {
	public class ImagesArray {
		public Dictionary<string, MyImage> imgDic = new Dictionary<string, MyImage>();
		public Links l = new Links();
		public ImagesArray() {
			Add(l.backgroundPath);

			Add(l.StandingPath);
			Add(l.RunningPath);
			Add(l.HookPath);
			Add(l.HookReversePath);
			Add(l.FishingPath);
			Add(l.fishPath);
			
		}
		public void Add(string path) {
			imgDic.Add(path, new MyImage(path));
		}
	}
	public class MyImage {
		public string path;
		public Image defaultImg;
		public Image mirrorImg;

		public MyImage(string path) {
			this.path = path;
			defaultImg = Image.FromFile(path);
			int idxMain = path.LastIndexOf('\\');

			string mirrorPath;

			mirrorPath = path.Substring(0, idxMain) + "\\mirror" + path.Substring(idxMain);
			try {
				mirrorImg = Image.FromFile(mirrorPath);

			} catch (Exception e) {
				mirrorImg = null;
			}

		}
	}


	public class Links {
		public string currentPath = "Images\\Fishman_idle.gif";
		public string currentMirrorPath = "Images\\mirror\\Fishman_idle.gif";

		public string StandingPath = "Images\\Fishman_idle.gif";
		public string RunningPath = "Images\\Fishman_walk.gif";
		public string HookPath = "Images\\Fishman_hook.gif";
		public string FishingPath = "Images\\Fishman_fishing.gif";
		public string HookReversePath = "Images\\Fishman_hook_reverse.gif";

		public string backgroundPath = "Images\\Background.png";
		public string fishPath = "Images\\Fish.png";
	}
}
