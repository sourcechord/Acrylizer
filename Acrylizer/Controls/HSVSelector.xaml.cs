using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Acrylizer.Controls
{
    /// <summary>
    /// HSVSelector.xaml の相互作用ロジック
    /// </summary>
    public partial class HSVSelector : UserControl
    {
        public HSVSelector()
        {
            InitializeComponent();
            this.root.DataContext = this;
        }



        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Hue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(double), typeof(HSVSelector), new PropertyMetadata(0.0));


    }
}
