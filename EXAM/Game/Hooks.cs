using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DataGenerationTool.API;

namespace DataGenerationTool
{
    partial class Form1
    {
        public static Form1 form;
        List<Keys> _pressedKeys = new List<Keys>();
        List<Keys> _unPressedKeys = new List<Keys>();
        void GenerateAndCopy()
        {
            GenerateDate();
            if(textBoxResult.Text!=string.Empty)
            CopyToClipboard();
        }
        void InitGlobalHook()
        {
            Program.KeyUp+=KeyUpEvent;
            Program.KeyDown+=KeyDownEvent;
        }
        void InitHooks()
        {
            Hooks.KeyDown += KeyDownEvent;
            Hooks.KeyUp += KeyUpEvent;
            Hooks.InstallHook();
            this.FormClosed += (s, e) =>
            {
                Hooks.UnInstallHook();
            };
        }
        public void KeyUpEvent(LLKHEventArgs e)
        {
            toolStripStatusLabel1.Text = string.Join(" + ", _pressedKeys);
            _pressedKeys.Remove(e.Keys);
            if (!_pressedKeys.Any() && _unPressedKeys.Any())
            {
                toolStripStatusLabel2.Text = string.Join(" + ", _unPressedKeys);
                if (string.Join("", _unPressedKeys) == "LControlKeySpace") GenerateAndCopy();
                _unPressedKeys.Clear();
            }
        }

        public void KeyDownEvent(LLKHEventArgs e)
        {
            if (!_pressedKeys.Contains(e.Keys) && !_unPressedKeys.Contains(e.Keys))
            {
                _pressedKeys.Add(e.Keys);
                _unPressedKeys.Add(e.Keys);
            }
        }
        public void Log(string message)
        {
            toolStripStatusLabel1.Text = message;
        }
        public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)API.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                form.Log(((Keys)vkCode).ToString());
            }
            return API.CallNextHookEx(Hooks._hookID, nCode, wParam, lParam);
        }
    }

    public static class Hooks
    {
        #region Declarations
        public delegate void HookKeyPress(LLKHEventArgs e);
        public static event HookKeyPress KeyUp;
        public static event HookKeyPress KeyDown;

        //[StructLayout(LayoutKind.Sequential)]
        //struct KBDLLHOOKSTRUCT
        //{
        //    public uint vkCode;
        //    public uint scanCode;
        //    public uint time;
        //    public IntPtr dwExtraInfo;
        //}

        public static IntPtr _hookID = IntPtr.Zero;
        static IntPtr hHook = IntPtr.Zero;
        static bool hookInstall = false;
        static API.HookProc hookDel;
        static LowLevelKeyboardProc _proc = Form1.HookCallback;
        #endregion

        /// <summary>
        /// Hook install method.
        /// </summary>
        public static void InstallHook()
        {
            if (IsHookInstalled) return;
            hookDel = HookProcFunction;
            hHook = API.SetWindowsHookEx(API.HookType.WH_KEYBOARD,
                hookDel, IntPtr.Zero, AppDomain.GetCurrentThreadId());
            if (hHook != IntPtr.Zero)
                hookInstall = true;
            else
                throw new Win32Exception("Can't install low level keyboard hook!");

            //if (IsHookInstalled) return;
            //_hookID = SetHook(_proc);
            //if (_hookID!= IntPtr.Zero) hookInstall = true;
            //else throw new Win32Exception("Cant't install low level keyboard hook");
        }

        //private static IntPtr SetHook(LowLevelKeyboardProc proc)
        //{
        //    using (Process curProcess = Process.GetCurrentProcess())
        //    using (ProcessModule curModule = curProcess.MainModule)
        //    {
        //        return SetWindowsHookEx(API.HookType.WH_KEYBOARD_LL, proc,
        //            GetModuleHandle(curModule.ModuleName),(uint)Thread.CurrentThread.ManagedThreadId);
        //    }
        //}

        /// <summary>
        /// If hook installed return true, either false.
        /// </summary>
        public static bool IsHookInstalled => hookInstall && hHook != IntPtr.Zero;
        /// <summary>
        /// If true local hook will installed, either global.
        /// </summary>
        /// <summary>
        /// Uninstall hook method.
        /// </summary>
        public static void UnInstallHook()
        {
            if (IsHookInstalled)
            {
                if (!API.UnhookWindowsHookEx(hHook))
                    throw new Win32Exception("Can't uninstall low level keyboard hook!");
                hHook = IntPtr.Zero;
                hookInstall = false;
            }
        }
        /// <summary>
        /// Hook process messages.
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        static IntPtr HookProcFunction(int nCode, IntPtr wParam, [In] IntPtr lParam)
        {
            if (nCode == 0)
            {
                var pressed = lParam.ToInt32() >> 31 == 0;
                Keys keys = (Keys)wParam.ToInt32();
                var args = new LLKHEventArgs(keys, pressed, 0U, 0U);
                if (pressed)
                    KeyDown?.Invoke(args);
                else
                    KeyUp?.Invoke(args);
                if (args.Hooked)
                    return (IntPtr)1;
            }
            return API.CallNextHookEx(hHook, nCode, wParam, lParam);
        }

    }

    public class LLKHEventArgs
    {
        readonly Keys keys;
        readonly bool pressed;
        readonly uint time;
        readonly uint scCode;

        public LLKHEventArgs(Keys keys, bool pressed, uint time, uint scanCode)
        {
            this.keys = keys;
            this.pressed = pressed;
            this.time = time;
            this.scCode = scanCode;
        }

        /// <summary>
        /// Key.
        /// </summary>
        public Keys Keys
        { get { return keys; } }
        /// <summary>
        /// Is key pressed or no.
        /// </summary>
        public bool IsPressed
        { get { return pressed; } }
        /// <summary>
        /// The time stamp for this message, equivalent to what GetMessageTime would return for this message.
        /// </summary>
        public uint Time
        { get { return time; } }
        /// <summary>
        /// A hardware scan code for the key.
        /// </summary>
        public uint ScanCode
        { get { return scCode; } }
        /// <summary>
        /// Is user hook key.
        /// </summary>
        public bool Hooked { get; set; }
    }

    static class API
    {
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, [In] IntPtr lParam);
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, [In] IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn,
        IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn,
        IntPtr hMod, uint dwThreadId);

        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }
        public const int WM_KEYDOWN = 0x0100;

    }
}