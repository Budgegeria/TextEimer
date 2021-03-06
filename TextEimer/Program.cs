﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;

using TextEimer.Windows;
using TextEimer.Log;
using TextEimer.Clipboard;
using TextEimer.Config;
using MovablePython;

namespace TextEimer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Threading.Mutex Mu = new System.Threading.Mutex(false, "{ed4a1d54-416d-47eb-adb6-9a2d47e96774}");
            if (Mu.WaitOne(0, false))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Settings settings = new Settings();

                Writer log = new Writer(settings);

                ForegroundWindow foregroundWindow = new ForegroundWindow();

                #region NotifyIconMenu
                ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                NotifyIconMenu notifyIconMenu = new NotifyIconMenu(contextMenuStrip, settings);
                notifyIconMenu.FocusHandler = foregroundWindow;
                notifyIconMenu.AddMenuItem(new TextEimer.Windows.MenuItems.Separator());
                notifyIconMenu.AddMenuItem(new TextEimer.Windows.MenuItems.Options("Optionen", settings));
                notifyIconMenu.AddMenuItem(new TextEimer.Windows.MenuItems.QuitItem("Beenden", "quit"));
                notifyIconMenu.LogWriter = log;
                #endregion

                #region NotifyIcon
                NotifyIcon notifyIcon = new NotifyIcon();
                notifyIcon.ContextMenuStrip = notifyIconMenu.contextMenuStrip;
                notifyIcon.Icon = (System.Drawing.Icon)TextEimer.Properties.Resources.ResourceManager.GetObject("bucket");
                notifyIcon.Text = "TextEimer";
                notifyIcon.Visible = true;
                notifyIcon.Click += delegate { notifyIconMenu.BuildContextMenuStrip(); };
                NotifyIconSymbol notifyIconSymbol = new NotifyIconSymbol(notifyIcon, notifyIconMenu);
                #endregion

                ClipboardHandler clipboardHandler = new ClipboardHandler(notifyIconMenu);
                clipboardHandler.LogWriter = log;

                #region global Hotkey
                Hotkey hk = new Hotkey();

                hk.KeyCode = Keys.V;
                hk.Windows = true;
                hk.Pressed += delegate {
                	notifyIconSymbol.ShowNotifyIconMenu();
                };

                hk.Register(notifyIconMenu.contextMenuStrip);

                Application.ApplicationExit += delegate {
                	if (hk.Registered)
                	{
                		hk.Unregister();
                	}
                };
                #endregion

                Application.Run();
            }
        }
    }
}
