﻿#pragma checksum "..\..\..\MyControl\TransControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "0414A53F24727BD2361C4F1FA200E6CC4EA1DCC9BE27C7D30C26E0EE5807B54D"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using IECSC.Trans.MyControl;
using Microsoft.Expression.Controls;
using Microsoft.Expression.Media;
using Microsoft.Expression.Shapes;
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


namespace IECSC.Trans.MyControl {
    
    
    /// <summary>
    /// TransControl
    /// </summary>
    public partial class TransControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\MyControl\TransControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox gbTitle;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\MyControl\TransControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle recAuto;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\MyControl\TransControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnLocPlcNo;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\MyControl\TransControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Expression.Shapes.BlockArrow baNorth;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\MyControl\TransControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Expression.Shapes.BlockArrow baSouth;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\MyControl\TransControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Expression.Shapes.BlockArrow baEast;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\MyControl\TransControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Expression.Shapes.BlockArrow baWest;
        
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
            System.Uri resourceLocater = new System.Uri("/IECSC.Trans.A;component/mycontrol/transcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MyControl\TransControl.xaml"
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
            this.gbTitle = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 2:
            this.recAuto = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 3:
            this.btnLocPlcNo = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\..\MyControl\TransControl.xaml"
            this.btnLocPlcNo.Click += new System.Windows.RoutedEventHandler(this.btnLocPlcNo_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.baNorth = ((Microsoft.Expression.Shapes.BlockArrow)(target));
            return;
            case 5:
            this.baSouth = ((Microsoft.Expression.Shapes.BlockArrow)(target));
            return;
            case 6:
            this.baEast = ((Microsoft.Expression.Shapes.BlockArrow)(target));
            return;
            case 7:
            this.baWest = ((Microsoft.Expression.Shapes.BlockArrow)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
