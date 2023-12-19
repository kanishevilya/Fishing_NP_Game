using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Reflection;

namespace Game
{
    public partial class FormGame : Form
    {
        public Player player = new Player(); // Обьек игрка

        public Dictionary<string, PlayerInfoForRecieve> playerDictionary = new Dictionary<string, PlayerInfoForRecieve>(); // Инфа про плееров
        public enum Direction
        {
            Left,
            Right,
            Stop,
        }
        public int backgroundPositionX = -1000; // позиция подложки
        public Image backgroundImg; // Подложка
        private Links lns = new Links(); // ссылки на чотоам картЫнки

        public int borderLeftRiver = -400; //левая гранЫца где рЭка
        public int borderRightRiver = -3200; //  правая гранЫца где река
        public FormGame()
        {
            InitializeComponent();
            FormManager.images = new ImagesArray(); // обьект с картинками 
            FormManager.MainFormInstance = this; // передается инстанс, чтобы могли использовать инвалидейт где угодно

        }

        private IPAddress ipserver = IPAddress.Parse("10.13.12.60");
        private int port = 12345;
        private Socket client = null;
        private byte[] buffer = new byte[1024];
        private bool isConnected = false;
        private static Mutex sendMutex = new Mutex();
        private AutoResetEvent resetEvent = new AutoResetEvent(false);

        public FishingWindow fishingWindow; // менюшка где ловится рыба полоска тудым сюдым
        public FishingTackleAndBait fishingTackleAndBait; // класс для наживки и снасти
        public FishWindow fishWindow; // менюшка когда словили рыбу, инфа о рыбе
        public ShopWindow shopWindow; // говнокодеры еще не сделали это

        public void Start(string name = "")
        {


            Connect(); // подключение к серверу
            player.init(); // инициализрируются переменные игрока
            player.Name = name;
            initForm(); // иницилазируинтсяесйце форма
            fishingWindow = new FishingWindow();
            fishingTackleAndBait = new FishingTackleAndBait();
            fishWindow = new FishWindow();
            shopWindow = new ShopWindow();
            gameTimer.Start();

            fishWindow.changeFish(fishingTackleAndBait.getBaitLikeFloat()); // смена цены в зависимости от качества

        }
        public void Connect()
        {

            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(ipserver, port);
                if (client.Connected)
                {

                    isConnected = true;
                    Thread thread = new Thread(RecieveCycle);
                    Thread thread2 = new Thread(SendCycle);
                    thread.Start(client);
                    thread2.Start(client);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "ERROR");
            }
        }
        private void SendCoordsToServer()
        {

            string temp = "COORD" + (player.playerX) + " " + player.playerY + " " + backgroundPositionX + " " + player.playerXOffset + " " + player.playerYOffset + "|" + "ANIM" + player.links.currentPath + " " + (player.currentDir == Direction.Left) + " " + player.Name + "$";


            sendMutex.WaitOne(); // Захватываем мьютекс для отправки данных
            try
            {
                client.Send(Encoding.UTF8.GetBytes(temp));
            }
            finally
            {
                sendMutex.ReleaseMutex(); // Освобождаем мьютекс после отправки данных
            }
        }
        //private void SendAnimationValue(){
        //	string temp = "ANIM" + player.lns.currentPath +" "+(player.currentDir==Direction.Left);
        //	Invoke(new Action(() => {
        //		Text = temp;
        //	}));

        //	sendMutex.WaitOne(); 
        //	try {
        //		client.Send(Encoding.UTF8.GetBytes(temp));
        //	} finally {
        //		sendMutex.ReleaseMutex(); 
        //	}
        //}

        private void RecieveCycle(object par)
        {
            try
            {
                while (isConnected)
                {
                    int size = client.Receive(buffer); //Строка от других клиентов

                    if (size > 0)
                    {
                        string temp = Encoding.UTF8.GetString(buffer, 0, size);
                        //Invoke(new Action(() => {
                        //	Text += "A";
                        //}));
                        if (temp.IndexOf("NEW") != -1)
                        { //если подключается новый игрок
                            temp = temp.Substring(3); //Убрать флаг //temp=RemoteEndPoint
                            PlayerInfoForRecieve info = new PlayerInfoForRecieve();
                            lock (playerDictionary)
                            {

                                playerDictionary.Add(temp, info); //key=remEP info=void at start
                                                                  //playerDic[temp].imgPath = lns.StandingPath;
                                                                  //playerDic[temp].playerImg = FormManager.imgs.imgDic[lns.StandingPath].defaultImg;
                                                                  //ImageAnimator.Animate(playerDic[temp].playerImg, this.FrameChangeHandler);
                            }
                        }
                        if (temp.IndexOf("INFO") != -1)
                        { //Каждые 20 мс получаю инфу от игрок/-ов/-а
                            temp = temp.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries).First();
                            //Чтобы не было повторений
                            temp = temp.Substring(4); //Убрать флаг
                            InfoSetMethod(temp); //парс инфорации
                        }
                        if (temp.IndexOf("END") != -1)
                        {
                            temp = temp.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries).First();
                            temp = temp.Substring(3);
                            string remEP = temp;
                            playerDictionary.Remove(remEP);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void InfoSetMethod(string temp)
        {
            //0;0
            //client.RemoteEndPoint + " " + info.coords + " " +
            //info.backgroundX + " " + info.coordsOffset + " " +
            //info.imgPath + " " + info.isMirror + "$";

            string[] info = temp.Split(' '); //Вся инфа общая в массив
            string[] coords = info[1].Split(';'); //вытаскиваем координаты т.к. первый элемент
                                                  //info[0] - RemoteEndPoint уже добавлен при NEW
            playerDictionary[info[0]].coords = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
            //парс координат
            playerDictionary[info[0]].backgroundX = int.Parse(info[2]);
            //парс сдвига подложки
            //backgroundPositionX-позиц моей подложки
            int backgrXDiff = backgroundPositionX - playerDictionary[info[0]].backgroundX;
            //Реальный сдвиг игрока. Т.к. координаты игрока никогда не выходят за границы камеры
            //Иначе мы бы видели всех игроков на экране, не зависимо от сдвига поля


            playerDictionary[info[0]].coords.X += backgrXDiff;
            //к фантому прибавляем сдвиг поля, для корректн. отображения

            string[] coordsOffset = info[3].Split(';');
            playerDictionary[info[0]].coordsOffset = new Point(int.Parse(coordsOffset[0]), int.Parse(coordsOffset[1]));
            //парс сдвига игрока (т.к. анимации разного размера, см. в class Player)

            bool isMirror = (info[5] == "True" ? true : false);
            //отзеркалена ли анимация

            if (playerDictionary[info[0]].imgPath != info[4])
            {
                playerDictionary[info[0]].imgPath = info[4]; //задается ссылка для новой анимации
            }



            playerDictionary[info[0]].isMirror = isMirror;
            if (!playerDictionary[info[0]].isGetInfo || playerDictionary[info[0]].Name == "")
            {
                playerDictionary[info[0]].Name = info[6];

            }
            playerDictionary[info[0]].isGetInfo = true; //получение первичной инфы, далее не имеет значения

            if (playerDictionary[info[0]].isMirror)
            {
                if (playerDictionary[info[0]].playerImg != FormManager.images.imgDic[playerDictionary[info[0]].imgPath].mirrorImg)
                {
                    playerDictionary[info[0]].playerImg = FormManager.images.imgDic[playerDictionary[info[0]].imgPath].mirrorImg;
                    //поставить изображение
                    //если изображение до этого имело другое направление
                }
            }
            else //см выше
            {
                if (playerDictionary[info[0]].playerImg != FormManager.images.imgDic[playerDictionary[info[0]].imgPath].defaultImg)
                {
                    playerDictionary[info[0]].playerImg = FormManager.images.imgDic[playerDictionary[info[0]].imgPath].defaultImg;

                }

            }
        }
        private void SendCycle(object par)
        {
            try
            {
                while (isConnected)
                {
                    SendCoordsToServer();
                    Thread.Sleep(20);

                    //SendAnimationValue();

                    //SendForGetInfo();
                    //resetEvent.WaitOne();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        private void initForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);  //убирает мирцание и тд
                                                                                                                                 // эта для аптимизациииии анимациии
            initImages(); // ставэться падлёшка 
        }
        private void initImages()
        {
            backgroundImg = FormManager.images.imgDic[lns.backgroundPath].defaultImg;

        }
        private void gameTimer_Tick(object sender, EventArgs e)
        {


            this.Invalidate();
            ImageAnimator.UpdateFrames();

            // каждые 20 минисекунд меняется фрЭйм для Онимасьон гифак


            plMovePlayerAndOther();

            player.gameTimer_Tick();

        }


        private void plMovePlayerAndOther()
        {
            int border = 50;
            float cameraBorder = this.Width / 3 * 2; // эта камера, занимает две трети всей подложки

            player.goRightAndLeftAlg(border, cameraBorder); // логика для движения перса или подложки
            player.HitboxLinkAlg(); // связывание с хитбоксом

        }
        public bool CheckCollision(int object1X, int object1Y, int object1Width, int object1Height, int object2X, int object2Y, int object2Width, int object2Height)
        {
            if (object1X + object1Width <= object2X || object1X >= object2X + object2Width || object1Y + object1Height <= object2Y || object1Y >= object2Y + object2Height)
            {
                return false;
            }
            else
            {
                return true;
            }

            // этот метод проверяет если хитбоксы игроков пересекаются
        }
        private void MainForm_Paint(object sender, PaintEventArgs e)
        { //прорисовывает все
            paintGifs(e); // при инвалидейте вызывается этот метод который прорисовывает гифки анимацииииииииииииии

        }
        public Font font = new Font("Microsoft Sans Serif", 16);
        public SolidBrush brush = new SolidBrush(Color.Black);

        private void DrawPlayers(PaintEventArgs e, string str)
        {
            PointF coords = new PointF(20, 20);
            e.Graphics.DrawString(str, font, brush, coords);
        }
        private void paintGifs(PaintEventArgs e)
        {
            e.Graphics.DrawImage(backgroundImg, new Point(backgroundPositionX, 0)); // прорисовка подложки
            e.Graphics.DrawImage(player.playerImg, new Point(player.playerX + player.playerXOffset, player.playerY + player.playerYOffset)); // игрока
            e.Graphics.DrawString(player.Name, font, brush, new Point(player.playerHeatBox.X, player.playerHeatBox.Top - 40));
            // прорисовка фантомов из майнкрафта ( подключенных челиков )
            string names = "1) "+player.Name+"\r\n";
            int i = 2;
            foreach (KeyValuePair<string, PlayerInfoForRecieve> playerInfo in playerDictionary)
            {
                PlayerInfoForRecieve plT = playerDictionary[playerInfo.Key];

                if (plT.isGetInfo)
                {
                    e.Graphics.DrawImage(plT.playerImg, new Point(plT.coords.X + plT.coordsOffset.X, plT.coords.Y + plT.coordsOffset.Y));
                    e.Graphics.DrawString(plT.Name, font, brush, new Point(plT.coords.X + plT.coordsOffset.X + 40, player.playerHeatBox.Top - 40));

                    names += i + ") "+ plT.Name + "\r\n";
                    i++;
                }
            }
            DrawPlayers(e, names);

            e.Graphics.DrawRectangle(new Pen(Color.AliceBlue), player.playerHeatBox); // хитбокс рисуется

            if (player.isFishing)
            { // если чел рыбачит, то открывается менюшка где полоска двигается
                fishingWindow.Paint(e);
            }
            if (player.isGetFish)
            { // если чел удачно порыбачил, то открывается менюшка с информацией о рыбе
                fishWindow.Paint(e);
            }
            if (player.isInShop)
            {
                shopWindow.Paint(e);
            }
            //Text = backgroundPositionX.ToString();
        }

        public void FrameChangeHandler(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void FormGame_KeyDown(object sender, KeyEventArgs e)
        {
            player.MainForm_KeyDown(sender, e);
        }

        private void FormGame_KeyUp(object sender, KeyEventArgs e)
        {
            player.MainForm_KeyUp(sender, e);
        }

        private void FormGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null)
            {
                client.Close();
            }
        }
    }

    // информация о юзере, передаются серверу при подключении нового клиента
    //Фантомный юзер
    public class PlayerInfoForRecieve
    {
        public Point coords;
        public string Name = "";
        public float Money = 0;
        public Point coordsOffset;
        public int backgroundX;
        public string imgPath;
        public bool isMirror;
        public Image playerImg;

        public bool isGetInfo = false; // получение первичной информации
    }

    // класс игрока
    public class Player
    {
        //public delegate void Invalidate();
        //переменные
        public string Name = "";

        //player
        public FormGame.Direction currentDir = Game.FormGame.Direction.Right;

        public Rectangle playerHeatBox;

        public const int defaultPlayerY = 535; // чтобы чел по воздуху не гулял

        public int frameAmount; // коилчество кадров в гифке

        //public int endFrameForStandUse; 
        //public int endFrameForCheckCollisions; 

        //4 frame
        //20 

        public float additionNumForFrame; // текущий фрейм в флоатах ( для проверки конца анимации )
        public float additionNumForHooking; // тоже самое для анимации когда чел кидает удочку

        public float tempNumForAddition; // скорость изменения фреймов ( прибавляется к addiitionNumForFrame )

        public bool isGoLeft, isGoRight; // два стейта, которые указывают текущее состояние чела, if both == false { direction.idle }

        public bool isDirectionSet; // если задано направление
        public bool isPlayingAction;                 //происходит ли сейчас какое-либо действие
        public bool isHooking; // кидает удочку
        public bool isRevHooking; // реверс
        public bool isFishing; // рыбачит
        public bool isGetFish; // поймал
        public bool isInShop; // поймал

        public int plSpeed; // скорость игрока

        // координаты и сдвиг
        public int playerX;
        public int playerXOffset;
        public int playerY;
        public int playerYOffset;

        public Image playerImg;

        public Links links = new Links();

        public List<float> fishesPrices = new List<float>();

        public float money = 0;

        public FishingCondition fCondition;
        public void init()
        {
            FormManager.player = this;
            #region
            currentDir = FormGame.Direction.Right;

            frameAmount = 0;



            additionNumForFrame = 0;
            additionNumForHooking = 0;
            tempNumForAddition = 0.2f;

            isGoLeft = false; isGoRight = false;


            isDirectionSet = false;
            isPlayingAction = false;
            isHooking = false;
            isRevHooking = false;
            isFishing = false;
            isInShop = false;
            isGetFish = false;

            plSpeed = 8;

            playerX = 500;
            playerXOffset = 0;
            playerYOffset = 0;
            playerY = 535;

            #endregion
            playerImg = FormManager.images.imgDic[links.StandingPath].defaultImg;

            playerHeatBox = new Rectangle(playerX + 50, playerY + 20, 70, 150);
            fCondition = new FishingCondition(-400, -3200);

            SetStartPackAnimations();
            plSetUpAnimation(playerImg);

        }
        private void SetStartPackAnimations()
        {
            //если убрать и не подвигаться до подключения 2 клиента будет прикол с анимацией
            plMoveRunPlayerAnimation(FormGame.Direction.Left);
            plMoveRunPlayerAnimation(FormGame.Direction.Right);

            isHooking = true;
            currentDir = FormGame.Direction.Right;
            plHookAnimation();
            currentDir = FormGame.Direction.Left;
            plHookAnimation();
            isHooking = false;

            isRevHooking = true;
            currentDir = FormGame.Direction.Right;
            plRevHookAnimation();
            currentDir = FormGame.Direction.Left;
            plRevHookAnimation();
            isRevHooking = false;

            isFishing = true;
            currentDir = FormGame.Direction.Right;
            plFishingAnimation();
            currentDir = FormGame.Direction.Left;
            plFishingAnimation();
            isFishing = false;

            currentDir = FormGame.Direction.Right;
            isGoLeft = false;
            isGoRight = false;
            isDirectionSet = false;
            plSetDefaultAnimation();
        }

        public void plSetUpAnimation(Image playerImg)
        {

            ImageAnimator.Animate(playerImg, this.FrameChangeHandler); //анимирует GIF

            FrameDimension dimentions = new FrameDimension(playerImg.FrameDimensionsList[0]); //возвращает массив кол-ва фреймов у GIF
            frameAmount = playerImg.GetFrameCount(dimentions);
            //endFrameForStandUse = frameAmount - 2;
        }
        private void FrameChangeHandler(object sender, EventArgs e)
        {
            FormManager.MainFormInstance.Invalidate();
        }


        public void gameTimer_Tick()
        {
            //FormManager.MainFormInstance.Invalidate();
            //ImageAnimator.UpdateFrames();


            //plMovePlayerAndOther();

            if (isPlayingAction)
            { // если чел чето делает
                if (additionNumForFrame < frameAmount)
                {
                    additionNumForFrame += tempNumForAddition;
                }
            }
            if (isHooking || isRevHooking)
            { // если кидает удочку или реверс тоже самое
                if (additionNumForHooking < frameAmount)
                {
                    additionNumForHooking += tempNumForAddition;
                }
            }
            if (additionNumForHooking >= frameAmount)
            { // конец анимации
                if (isHooking)
                { // бросок => вызывается анимация рыбачества

                    isHooking = false;
                    isFishing = true;
                    plFishingAnimation();
                }
                else
                { // достал => деф аним
                    isRevHooking = false;
                    isFishing = false;
                    plSetDefaultAnimation();
                }
                //isDirectionSet = false;
            }
            if (additionNumForFrame >= frameAmount)
            {
                plSetDefaultAnimation();
                isDirectionSet = false;
            }
        }

        public void plSetDefaultAnimation()
        {
            links.currentPath = links.StandingPath;
            playerXOffset = 0;
            playerYOffset = 0;
            isPlayingAction = false;
            isHooking = false;
            isRevHooking = false;
            isFishing = false;
            isInShop = false;

            //isGetFish = false;
            plMirrorGif();
            if (currentDir == FormGame.Direction.Left)
            {

                playerImg = FormManager.images.imgDic[links.currentPath].mirrorImg;
            }
            else
            {

                playerImg = FormManager.images.imgDic[links.currentPath].defaultImg;
            }
            additionNumForFrame = 0;
            additionNumForHooking = 0;

            plSetUpAnimation(playerImg);
        }
        public void plMirrorGif()
        {
            int idxMain = links.currentPath.LastIndexOf('\\');

            int idxMirror = links.currentMirrorPath.LastIndexOf('\\');


            //links.currentMirrorPath = links.currentMirrorPath.Substring(0, idxMirror) + links.currentPath.Substring(idxMain);

            if (currentDir == FormGame.Direction.Right && isGoRight)
            {
                //currentDir = MainForm.Direction.Left;
                playerImg = FormManager.images.imgDic[links.currentPath].defaultImg;
            }
            else if (currentDir == FormGame.Direction.Left && isGoLeft)
            {
                //currentDir = MainForm.Direction.Right;
                playerImg = FormManager.images.imgDic[links.currentPath].mirrorImg;
            }
            if (currentDir == FormGame.Direction.Left && (isHooking || isRevHooking))
            {
                playerImg = FormManager.images.imgDic[links.currentPath].mirrorImg;
                playerXOffset *= -1;
                playerXOffset -= 20;
            }
            else if (currentDir == FormGame.Direction.Right && (isHooking || isRevHooking))
            {
                playerImg = FormManager.images.imgDic[links.currentPath].defaultImg;

            }

            if (currentDir == FormGame.Direction.Left && isFishing)
            {
                playerImg = FormManager.images.imgDic[links.currentPath].mirrorImg;
                playerXOffset *= -1;
                playerXOffset -= 15;
            }
            else if (currentDir == FormGame.Direction.Right && isFishing)
            {
                playerImg = FormManager.images.imgDic[links.currentPath].defaultImg;

            }

        }





        public void MainForm_KeyDown(object sender, KeyEventArgs e)
        {

            if (!e.Shift)
            {

                //isCanDrawFishWindow = false;
                if (e.KeyCode == Keys.Left && !isDirectionSet && !isHooking && !isFishing && !isRevHooking)
                { //Чтобы заново не происходил вызов анимации
                    isGetFish = false;
                    isInShop = false;
                    plMoveRunPlayerAnimation(FormGame.Direction.Left);
                }
                if (e.KeyCode == Keys.Right && !isDirectionSet && !isHooking && !isFishing && !isRevHooking)
                {
                    isGetFish = false;
                    isInShop = false;
                    plMoveRunPlayerAnimation(FormGame.Direction.Right);
                }
                if (e.KeyCode == Keys.X && !isHooking && !isFishing && !isRevHooking)
                {
                    isGetFish = false;
                    isInShop = false;
                    if (fCondition.CheckFishingCondition(playerX, FormManager.MainFormInstance.backgroundPositionX, currentDir))
                    {
                        isHooking = true;
                        FormManager.MainFormInstance.fishingWindow.setCursor();
                        FormManager.MainFormInstance.fishingWindow.setChance(FormManager.MainFormInstance.fishingTackleAndBait.getTackleLikeFloat());
                        plHookAnimation();
                    }
                }
                if (e.KeyCode == Keys.Z && !isHooking && isFishing && !isRevHooking)
                {
                    isGetFish = false;
                    isFishing = false;
                    isRevHooking = true;
                    plRevHookAnimation();
                    checkFishMenu();
                }
                if (e.KeyCode == Keys.I && !isHooking && !isFishing && !isRevHooking)
                {
                    isInShop = !isInShop;
                    if (isInShop)
                    {
                        FormManager.MainFormInstance.shopWindow.setList(fishesPrices);
                    }
                }
                if (isInShop)
                {
                    FormManager.MainFormInstance.shopWindow.Sell(e.KeyCode);
                }
            }


        }
        public void checkFishMenu()
        {
            isGetFish = FormManager.MainFormInstance.fishingWindow.getWinOrLose();
            if (isGetFish)
            { // если поймали рыбку кильку
                FormManager.MainFormInstance.fishWindow.changeFish(FormManager.MainFormInstance.fishingTackleAndBait.getBaitLikeFloat()); // changes information about fish
                fishesPrices.Add(FormManager.MainFormInstance.fishWindow.fish.setPriceByQuality()); // ставится цена в зависимости от качества
            }
        }
        public void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.Left || e.KeyCode == Keys.Down || e.KeyCode == Keys.B) && !isPlayingAction && !isFishing)
            {
                // если крч нажали кнопку, и потом отпустили, и щас ниче не делаем ващеее, то короче все стейты сбрасываются и становится деф аним
                isGoLeft = false;
                isGoRight = false;
                isDirectionSet = false;

                plSetDefaultAnimation();

            }
        }
        // анимации для каждых действиииийийийий 
        #region
        public void plHookAnimation()
        {
            links.currentPath = links.HookPath;
            isHooking = true;
            tempNumForAddition = 0.13f;
            playerXOffset = 40;
            playerYOffset = -20;
            plMirrorGif();
            plSetUpAnimation(playerImg);
        }
        public void plRevHookAnimation()
        {
            links.currentPath = links.HookReversePath;
            isRevHooking = true;
            tempNumForAddition = 0.13f;
            playerXOffset = 40;
            playerYOffset = -20;
            plMirrorGif();
            plSetUpAnimation(playerImg);
        }
        public void plFishingAnimation()
        {
            links.currentPath = links.FishingPath;
            tempNumForAddition = 0;
            additionNumForHooking = 0;
            playerXOffset = 45;
            playerYOffset = 25;
            plMirrorGif();
            plSetUpAnimation(playerImg);
        }
        public void plMoveRunPlayerAnimation(FormGame.Direction direction)
        {
            if (direction == Game.FormGame.Direction.Left)
            {
                isGoLeft = true;
                isGoRight = false;
            }
            if (direction == Game.FormGame.Direction.Right)
            {
                isGoRight = true;
                isGoLeft = false;
            }
            currentDir = direction;
            playerYOffset = 0;
            playerXOffset = 0;
            //playerImg = Image.FromFile(lns.jotaroRunningPath);
            links.currentPath = links.RunningPath;
            plMirrorGif();
            isDirectionSet = true;  //чтобы не происходило зажима, на защите объясню
            isPlayingAction = false;
            plSetUpAnimation(playerImg);

        }
        #endregion
        public void goRightAndLeftAlg(int border, float cameraBorder, int borderRiver = 320)
        {

            // крч так как у нас есть граница камер, если мы еще на краю то двигается чел, а если нет, то двигается только подложка
            FormGame frm = FormManager.MainFormInstance;
            bool isOnEndOfMap = frm.backgroundPositionX <= frm.borderRightRiver; // если короче мы дошли реки с ПРАВА
            bool isOnStartOfMap = frm.backgroundPositionX >= frm.borderLeftRiver; // если крч мы дошли до реки с ЛЕВА
            if (isGoLeft)
            { // если идем налева
                if (frm.backgroundPositionX < 0 && playerHeatBox.X < frm.Width - cameraBorder && !isOnStartOfMap)
                {
                    frm.backgroundPositionX += plSpeed; // если чел дошел до границы камеры, подложка идет
                }
                else if (playerHeatBox.X > border && !isOnStartOfMap)
                {
                    playerX -= plSpeed; // просто двигается 
                }
                else if (isOnStartOfMap)
                {
                    if (playerHeatBox.X > borderRiver)
                    {
                        playerX -= plSpeed; // если дошел до реки ( частный случай )
                    }
                }
            }
            // тот же принцип
            if (isGoRight)
            {
                if (frm.backgroundPositionX + frm.backgroundImg.Width > frm.ClientSize.Width + 20 && playerHeatBox.X > cameraBorder && !isOnEndOfMap)
                {
                    frm.backgroundPositionX -= plSpeed;
                }
                else if (playerHeatBox.X + playerHeatBox.Width < frm.ClientSize.Width - border && !isOnEndOfMap)
                {
                    playerX += plSpeed;
                }
                else if (isOnEndOfMap)
                {
                    if (playerHeatBox.X + playerHeatBox.Width < frm.ClientSize.Width - borderRiver)
                    {
                        playerX += plSpeed;
                    }
                }
            }
        }
        public void HitboxLinkAlg()
        {
            playerHeatBox.X = playerX + 50; //Отклонение хитбокса от точки начала гифки
            playerHeatBox.Y = playerY + 20;
            if (currentDir == FormGame.Direction.Left)
            {
                playerHeatBox.X = playerX + 30;

            }
        }

    }

    public class Fish
    {
        public float Price { get; set; } = 0;
        public const float MaxPrice = 1000.0f;
        public float Quality { get; set; } = 0;

        public float setPriceByQuality()
        {
            return (Price = MaxPrice * Quality);
        }
    }

    public class ShopWindow
    {
        public Rectangle BackGround;
        private SolidBrush brush = new SolidBrush(Color.LightPink);
        private Pen pen = new Pen(Color.RoyalBlue);
        public PointF pointStrLeft;
        public PointF pointStrRight;

        public Font font = new Font("MV Boli", 24);

        public List<float> prices = new List<float>();
        public string first = "";
        public string second = "";

        public float TotalPrice = 0;
        public ShopWindow()
        {
            FormGame temp = FormManager.MainFormInstance;
            int WindowWidth = temp.ClientSize.Width;

            int BackGroundWidth = 800; int BackY = 50;
            int BackGroundHeight = 600;

            BackGround = new Rectangle(WindowWidth / 2 - BackGroundWidth / 2, BackY, BackGroundWidth, BackGroundHeight);

            pointStrLeft = new PointF(BackGround.X + 20, BackGround.Y);
            pointStrRight = new PointF(BackGround.X + 400, BackGround.Y);

            pen.Width = 5;
        }

        public void setList(List<float> prcs)
        {
            prices = prcs;
            TotalPrice = 0;
            foreach (int price in prices)
            {
                TotalPrice += price;
            }
            first = $"Fish amount: {prices.Count}\r\n" +
            $"Total price: {TotalPrice}\r\n" +
            $"Money: {FormManager.player.money}";
            string[] NamesArr = Enum.GetNames(typeof(FishingTackleAndBait.Tackle));
            string names = "";
            int i = 1;
            foreach (string name in NamesArr)
            {
                names += i + ". " + name + " " + FishingTackleAndBait.GetPriceByTackle(name) + "\r\n";
                i++;
            }
            second = $"Tackles: \r\n{names}";
            NamesArr = Enum.GetNames(typeof(FishingTackleAndBait.Bait));
            names = "";
            foreach (string name in NamesArr)
            {
                names += i + ". " + name + " " + FishingTackleAndBait.GetPriceByBait(name) + "\r\n";
                i++;
            }
            second += $"\r\nBaits: \r\n{names}";

        }

        public void Paint(PaintEventArgs e)
        {
            brush.Color = Color.MediumSlateBlue;
            e.Graphics.FillRectangle(brush, BackGround);

            e.Graphics.DrawRectangle(pen, BackGround);

            brush.Color = Color.Khaki;

            e.Graphics.DrawString(first, font, brush, pointStrLeft);
            e.Graphics.DrawString(second, font, brush, pointStrRight);
        }

        //case "Standart":
        //            return 600;
        //        case "Good":
        //            return 1350;
        //        case "Perfect":
        //            return 2000;

        public void Sell(Keys key)
        {
            string message = "";
            int price = 0;
            switch (key)
            {
                case Keys.D0:
                    FormManager.player.money += TotalPrice;
                    TotalPrice = 0;
                    prices.Clear();
                    setList(prices);
                    break;
                case Keys.D1:
                    price = FishingTackleAndBait.GetPriceByTackle("Standart");
                    if (FormManager.player.money >= price && FormManager.MainFormInstance.fishingTackleAndBait.tackle != FishingTackleAndBait.Tackle.Standart)
                    {
                        FormManager.player.money -= price;
                        FormManager.MainFormInstance.fishingTackleAndBait.tackle = FishingTackleAndBait.Tackle.Standart;
                        message = "Вы приобрели стандартную снасть";
                    }
                    else if (FormManager.player.money < price)
                    {
                        message = "Недостаточно средтств";
                    }
                    else
                    {
                        message = "У вас уже приобретена эта снасть";
                    }
                    MessageBox.Show(message);
                    break;
                case Keys.D2:
                    price = FishingTackleAndBait.GetPriceByTackle("Good");
                    if (FormManager.player.money >= price && FormManager.MainFormInstance.fishingTackleAndBait.tackle != FishingTackleAndBait.Tackle.Good)
                    {
                        FormManager.player.money -= price;
                        FormManager.MainFormInstance.fishingTackleAndBait.tackle = FishingTackleAndBait.Tackle.Good;
                        message = "Вы приобрели хорошую снасть";
                    }
                    else if (FormManager.player.money < price)
                    {
                        message = "Недостаточно средтств";
                    }
                    else
                    {
                        message = "У вас уже приобретена эта снасть";
                    }
                    MessageBox.Show(message);
                    break;
                case Keys.D3:
                    price = FishingTackleAndBait.GetPriceByTackle("Perfect");
                    if (FormManager.player.money >= price && FormManager.MainFormInstance.fishingTackleAndBait.tackle != FishingTackleAndBait.Tackle.Perfect)
                    {
                        FormManager.player.money -= price;
                        FormManager.MainFormInstance.fishingTackleAndBait.tackle = FishingTackleAndBait.Tackle.Perfect;
                        message = "Вы приобрели идеальную снасть";
                    }
                    else if (FormManager.player.money < price)
                    {
                        message = "Недостаточно средтств";
                    }
                    else
                    {
                        message = "У вас уже приобретена эта снасть";
                    }
                    MessageBox.Show(message);
                    break;
                //case "Standart":
                //    return 150;
                //case "Good":
                //    return 600;
                //case "Perfect":
                //    return 1000;
                case Keys.D4:
                    price = FishingTackleAndBait.GetPriceByBait("Standart");
                    if (FormManager.player.money >= price && FormManager.MainFormInstance.fishingTackleAndBait.bait != FishingTackleAndBait.Bait.Standart)
                    {
                        FormManager.player.money -= price;
                        FormManager.MainFormInstance.fishingTackleAndBait.bait = FishingTackleAndBait.Bait.Standart;
                        message = "Вы приобрели стандартную наживку";
                    }
                    else if (FormManager.player.money < price)
                    {
                        message = "Недостаточно средств";
                    }
                    else
                    {
                        message = "У вас уже приобретена эта наживка";
                    }
                    MessageBox.Show(message);
                    break;
                case Keys.D5:
                    price = FishingTackleAndBait.GetPriceByBait("Good");
                    if (FormManager.player.money >= price && FormManager.MainFormInstance.fishingTackleAndBait.bait != FishingTackleAndBait.Bait.Good)
                    {
                        FormManager.player.money -= price;
                        FormManager.MainFormInstance.fishingTackleAndBait.bait = FishingTackleAndBait.Bait.Good;
                        message = "Вы приобрели хорошую наживку";
                    }
                    else if (FormManager.player.money < price)
                    {
                        message = "Недостаточно средтств";
                    }
                    else
                    {
                        message = "У вас уже приобретена эта наживка";
                    }
                    MessageBox.Show(message);
                    break;
                case Keys.D6:
                    price = FishingTackleAndBait.GetPriceByBait("Perfect");
                    if (FormManager.player.money >= price && FormManager.MainFormInstance.fishingTackleAndBait.bait != FishingTackleAndBait.Bait.Perfect)
                    {
                        FormManager.player.money -= price;
                        FormManager.MainFormInstance.fishingTackleAndBait.bait = FishingTackleAndBait.Bait.Perfect;
                        message = "Вы приобрели идеальную наживку";
                    }
                    else if (FormManager.player.money < price)
                    {
                        message = "Недостаточно средтств";
                    }
                    else
                    {
                        message = "У вас уже приобретена эта наживка";
                    }
                    MessageBox.Show(message);
                    break;
            }
            setList(prices);
        }

    }
    public class FishWindow
    {
        public Rectangle BackGround;
        private SolidBrush brush = new SolidBrush(Color.MediumSlateBlue);
        private Pen pen = new Pen(Color.Khaki);
        public static Links links = new Links();
        public Fish fish = new Fish();
        public static Image image = FormManager.images.imgDic[links.fishPath].defaultImg;

        public Point imgPos;
        public PointF pointStr;
        public float price = 0;
        public string str = "";

        public Font font = new Font("MV Boli", 40);
        public FishWindow()
        {
            FormGame temp = FormManager.MainFormInstance;
            int WindowWidth = temp.ClientSize.Width;

            int BackGroundWidth = 800; int BackY = 50;
            int BackGroundHeight = 500;

            BackGround = new Rectangle(WindowWidth / 2 - BackGroundWidth / 2, BackY, BackGroundWidth, BackGroundHeight);

            imgPos = new Point(BackGround.Left + 20, BackGround.Top + 20);
            pointStr = new PointF(imgPos.X + image.Width + 30, imgPos.Y + 300);
            pen.Width = 5;
        }
        public void changeFish(float quality)
        {
            fish.Quality = quality;
            price = fish.setPriceByQuality();
            str = $"Fish quality: {quality * 100}% \r\n" +
            $"Fish price: {price}";

        }
        public void Paint(PaintEventArgs e)
        {
            brush.Color = Color.MediumSlateBlue;
            e.Graphics.FillRectangle(brush, BackGround);

            e.Graphics.DrawRectangle(pen, BackGround);
            e.Graphics.DrawImage(image, imgPos);

            brush.Color = Color.Khaki;

            e.Graphics.DrawString(str, font, brush, pointStr);
        }
    }
    public class FishingTackleAndBait
    {
        public enum Tackle
        {
            Standart, //10%
            Good, //20%
            Perfect //35%
        }

        public Tackle tackle = Tackle.Standart;
        public float getTackleLikeFloat()
        {
            switch (tackle)
            {
                case Tackle.Standart:
                    return 0.1f;
                case Tackle.Good:
                    return 0.20f;
                case Tackle.Perfect:
                    return 0.35f;
            }
            return 0.1f;
        }

        public static int GetPriceByTackle(string tackle)
        {
            switch (tackle)
            {
                case "Standart":
                    return 1800;
                case "Good":
                    return 4050;
                case "Perfect":
                    return 6000;
            }
            return 0;
        }

        public enum Bait
        {
            Standart, //40%
            Good, //70%
            Perfect //100% //от цены
        }

        public static int GetPriceByBait(string bait)
        {
            switch (bait)
            {
                case "Standart":
                    return 450;
                case "Good":
                    return 1800;
                case "Perfect":
                    return 3000;
            }
            return 0;
        }

        public Bait bait = Bait.Standart;
        public Random rand = new Random();
        public float getBaitLikeFloat()
        {
            switch (bait)
            {
                case Bait.Standart:
                    return rand.Next(20, 50) / 100.0f;//0.4f;
                case Bait.Good:
                    return rand.Next(60, 80) / 100.0f; //0.7f;
                case Bait.Perfect:
                    return rand.Next(85, 100) / 100.0f; //0.7f;
            }
            return 0.1f;
        }

    }
    public class FishingWindow
    {
        public Rectangle BackGround;
        public Rectangle GamePole;
        public Rectangle ChancePole;

        public Rectangle Cursor;
        public int CursorAddition = 7;

        public float ChanceInPercent { get; set; } = 0.5f;

        private SolidBrush brush = new SolidBrush(Color.PaleVioletRed);

        public bool getWinOrLose()
        {
            if (Cursor.X >= ChancePole.Left && Cursor.X + Cursor.Width <= ChancePole.Right)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void setChance(float ch)
        {
            FormGame temp = FormManager.MainFormInstance;
            int WindowWidth = temp.ClientSize.Width;
            ChanceInPercent = ch;
            ChancePole.Width = (int)(GamePole.Width * ChanceInPercent);
            ChancePole = new Rectangle(WindowWidth / 2 - ChancePole.Width / 2, ChancePole.Y, ChancePole.Width, ChancePole.Height);
        }
        public void setCursor()
        {
            FormGame temp = FormManager.MainFormInstance;
            int WindowWidth = temp.ClientSize.Width;

            Cursor.Location = new Point(WindowWidth / 2 - GamePole.Width / 2, GamePole.Y);
            CursorAddition = 7;
        }
        public FishingWindow()
        {
            FormGame temp = FormManager.MainFormInstance;
            int WindowWidth = temp.ClientSize.Width;

            int BackGroundWidth = 400; int BackY = 50;
            int BackGroundHeight = 200;

            int GamePoleWidth = BackGroundWidth - 40; int GameY = BackY + 20;
            int GamePoleHeight = BackGroundHeight - 40;

            int ChancePoleHeight = GamePoleHeight;
            int ChancePoleWidth = (int)(GamePoleWidth * ChanceInPercent);

            int CursorWidth = 20;
            int CursorHeight = ChancePoleHeight;

            BackGround = new Rectangle(WindowWidth / 2 - BackGroundWidth / 2, BackY, BackGroundWidth, BackGroundHeight);
            GamePole = new Rectangle(WindowWidth / 2 - GamePoleWidth / 2, GameY, GamePoleWidth, GamePoleHeight);
            ChancePole = new Rectangle(WindowWidth / 2 - ChancePoleWidth / 2, GameY, ChancePoleWidth, ChancePoleHeight);
            Cursor = new Rectangle(WindowWidth / 2 - GamePoleWidth / 2, GameY, CursorWidth, CursorHeight);
        }
        public void Paint(PaintEventArgs e)
        {
            brush.Color = Color.PaleVioletRed;
            e.Graphics.FillRectangle(brush, BackGround);
            brush.Color = Color.Pink;
            e.Graphics.FillRectangle(brush, GamePole);
            brush.Color = Color.MediumSeaGreen;
            e.Graphics.FillRectangle(brush, ChancePole);
            brush.Color = Color.MediumSlateBlue;
            e.Graphics.FillRectangle(brush, Cursor);
            int Add = 0;
            if (Cursor.Location.X + Cursor.Width + CursorAddition >= GamePole.X + GamePole.Width)
            {

                CursorAddition *= -1;
                Add = GamePole.X + GamePole.Width - (Cursor.Location.X + Cursor.Width) - CursorAddition;
            }
            if (Cursor.Location.X + CursorAddition <= GamePole.X)
            {
                CursorAddition *= -1;
                Add = Cursor.Location.X - GamePole.X - CursorAddition;
            }
            Cursor.Location = new Point(Cursor.Location.X + CursorAddition + Add, Cursor.Location.Y);
        }
    }

    public class FishingCondition
    {

        private bool isCanFishing { get; set; }

        private int leftBorder { get; set; }

        private int rightBorder { get; set; }

        private const int leftPlayerPos = 284;

        private const int rightPlayerPos = 756;

        public FormGame.Direction dir { get; set; }

        public FishingCondition(int leftBorder, int rightBorder)
        {
            this.leftBorder = leftBorder;
            this.rightBorder = rightBorder;
        }

        public bool CheckFishingCondition(int playerPos, int poleOffset, FormGame.Direction dir)
        {
            if (dir == FormGame.Direction.Left)
            {
                if (playerPos - 120 <= leftPlayerPos && poleOffset == leftBorder)
                {
                    return true;
                }
            }
            else if (dir == FormGame.Direction.Right)
            {
                if (playerPos + 120 >= rightPlayerPos && poleOffset == rightBorder)
                {
                    return true;
                }
            }
            return false;
        }

    }
    public static class FormManager
    {
        public static FormGame MainFormInstance { get; set; }
        public static FormMenu MenuFormInstance { get; set; }
        public static ImagesArray images { get; set; }

        public static Player player { get; set; }
    }
}

