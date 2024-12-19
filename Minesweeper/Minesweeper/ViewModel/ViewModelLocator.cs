/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ChineseChess"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using MvvmLight.CommonServiceLocator;
using MvvmLight.Ioc;

namespace Minesweeper.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    internal sealed class ViewModelLocator
    {
        static ViewModelLocator()
        {
            Instance = new ViewModelLocator();
        }

        public static ViewModelLocator Instance
        {
            get;
        }

        private ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MineCustomViewModel>();
            SimpleIoc.Default.Register<NickNameViewModel>();
            SimpleIoc.Default.Register<HeroRankViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public NickNameViewModel NickName
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NickNameViewModel>();
            }
        }

        public HeroRankViewModel Hero
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HeroRankViewModel>();
            }
        }

        public MineCustomViewModel MineCustom
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MineCustomViewModel>();
            }
        }

        public void Cleanup()
        {
            MineCustom.Cleanup();
            NickName.Cleanup();
            Hero.Cleanup();
            Main.Cleanup();
        }
    }
}