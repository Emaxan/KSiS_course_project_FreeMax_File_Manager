﻿#pragma checksum "..\..\..\Windows\SettingsWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3A786E039722BB9C607A39DC8FA3BF09"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FreeMax_File_Manager.Properties;
using FreeMax_File_Manager.Properties.UserSettings.Colors;
using FreeMax_File_Manager.Properties.UserSettings.Text.SettingsWindow;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace FreeMax_File_Manager.Windows {
    
    
    /// <summary>
    /// SettingsWindow
    /// </summary>
    public partial class SettingsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FreeMax_File_Manager.Windows.SettingsWindow WSettings;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid GMain;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TbTitle;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel SpMenu;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TbMenu1;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TbMenu2;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox LbSetting;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid GBottom;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TbSave;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\..\Windows\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TbCancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/FreeMax File Manager;component/windows/settingswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\SettingsWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.WSettings = ((FreeMax_File_Manager.Windows.SettingsWindow)(target));
            
            #line 18 "..\..\..\Windows\SettingsWindow.xaml"
            this.WSettings.KeyUp += new System.Windows.Input.KeyEventHandler(this.WSettings_KeyUp);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\Windows\SettingsWindow.xaml"
            this.WSettings.KeyDown += new System.Windows.Input.KeyEventHandler(this.WSettings_KeyDown);
            
            #line default
            #line hidden
            
            #line 20 "..\..\..\Windows\SettingsWindow.xaml"
            this.WSettings.SizeChanged += new System.Windows.SizeChangedEventHandler(this.SW_OnSizeChanged);
            
            #line default
            #line hidden
            
            #line 21 "..\..\..\Windows\SettingsWindow.xaml"
            this.WSettings.Activated += new System.EventHandler(this.SW_OnActivated);
            
            #line default
            #line hidden
            
            #line 22 "..\..\..\Windows\SettingsWindow.xaml"
            this.WSettings.Deactivated += new System.EventHandler(this.SW_OnDeactivated);
            
            #line default
            #line hidden
            return;
            case 2:
            this.GMain = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.TbTitle = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.SpMenu = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 5:
            this.TbMenu1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.TbMenu2 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.LbSetting = ((System.Windows.Controls.ListBox)(target));
            return;
            case 8:
            this.GBottom = ((System.Windows.Controls.Grid)(target));
            return;
            case 9:
            this.TbSave = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.TbCancel = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
